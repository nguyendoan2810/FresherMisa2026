using Dapper;
using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FresherMisa2026.Infrastructure.Repositories
{
    /// <summary>
    /// Base repository (Refactored: Tối ưu Connection Pooling & Caching)
    /// </summary>
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>, IDisposable where TEntity : BaseModel
    {
        protected string _connectionString = string.Empty;
        protected IConfiguration _configuration;
        public Type _modelType = null;
        protected string _tableName;

        protected readonly IMemoryCache _cache;

        protected MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        // Constructor
        public BaseRepository(IConfiguration configuration, IMemoryCache cache)
        {
            _configuration = configuration;
            _cache = cache;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            _modelType = typeof(TEntity);
            _tableName = _modelType.GetTableName();
        }

        public void Dispose() { }

        #region Method Get (Có Caching)
        /// <summary>
        /// Repo lấy danh sách thực thể (Có Caching, tự động làm mới sau 5 phút hoặc khi có thay đổi dữ liệu)
        /// </summary>
        /// <returns></returns>
        /// Created by: nvdoan (10/04/2026)
        public async Task<IEnumerable<BaseModel>> GetEntitiesAsync()
        {
            // Đặt Cache Key theo tên bảng và hành động để dễ quản lý, hất bỏ khi có thay đổi dữ liệu hoặc sau 5 phút
            var cacheKey = $"{_tableName}_Action_GetAll";

            // Nếu trong máy không có sẵn Cache mới lôi đầu thằng DB lên làm
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<BaseModel> entities))
            {
                entities = await GetEntitiesUsingCommandTextAsync();
                var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(cacheKey, entities, cacheOptions);
            }
            return entities;
        }

        /// <summary>
        /// Repo lấy danh sách thực thể bằng CommandText
        /// </summary>
        /// <returns></returns>
        /// Created by: nvdoan (19/04/2026)
        private async Task<IEnumerable<TEntity>> GetEntitiesUsingCommandTextAsync()
        {
            var query = new StringBuilder($"select * from {_tableName}");

            if (_modelType.GetHasDeletedColumn())
            {
                query.Append($" where IsDeleted = FALSE");
            }

            // Gọi using linh hoạt (đọc xong dữ liệu là TỰ ĐÓNG LUÔN KẾT NỐI trả về Pool)
            using var connection = CreateConnection();
            var entities = await connection.QueryAsync<TEntity>(query.ToString(), commandType: CommandType.Text);

            return entities.ToList();
        }

        /// <summary>
        /// Repo lấy bản ghi theo id (Có Caching, tự động làm mới sau 5 phút hoặc khi có thay đổi dữ liệu)
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        /// Created by: nvdoan (19/04/2026)
        public async Task<TEntity> GetEntityByIDAsync(Guid entityId)
        {
            var cacheKey = $"{_tableName}_Action_GetById_{entityId}";

            if (!_cache.TryGetValue(cacheKey, out TEntity entity))
            {
                entity = await GetEntitieByIdUsingCommandTextAsync(entityId.ToString());
                if (entity != null)
                {
                    _cache.Set(cacheKey, entity, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }
            return entity;
        }

        /// <summary>
        /// Repo lấy bản ghi theo id bằng CommandText
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Created by: nvdoan (19/04/2026)
        private async Task<TEntity> GetEntitieByIdUsingCommandTextAsync(string id)
        {
            var query = new StringBuilder($"select * from {_tableName}");
            var primaryKey = _modelType.GetKeyName();
            // Đếm số điều kiện where để biết thêm "where" hay "and" cho đúng cú pháp SQL khi có thêm điều kiện IsDeleted = FALSE ở dưới
            int whereCount = 0; 

            if (primaryKey != null)
            {
                query.Append($" where {primaryKey} = @Id");
                whereCount++;
            }

            if (_modelType.GetHasDeletedColumn())
            {
                query.Append(whereCount == 0 ? " where " : " and ");
                query.Append("IsDeleted = FALSE");
            }

            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<TEntity>(query.ToString(), new { Id = id }, commandType: CommandType.Text);
        }
        #endregion

        #region Method Thực chiến (Tự động hất Cache rác sau khi sửa DB)
        /// <summary>
        /// Repo xóa bản ghi
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        /// Created by: nvdoan (19/04/2026)
        public async Task<int> DeleteAsync(Guid entityId)
        {
            var rowAffects = 0;
            using var connection = CreateConnection();
            await connection.OpenAsync(); // Bắt buộc mở cho Transaction

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var keyName = _modelType.GetKeyName();
                    var dynamicParams = new DynamicParameters();
                    dynamicParams.Add($"@v_{keyName}", entityId);

                    rowAffects = await connection.ExecuteAsync($"Proc_Delete{_tableName}ById", param: dynamicParams, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();

                    // Vừa Xóa thành công phải đập bỏ sạch tàn dư Cache 5 phút của thằng đó
                    if (rowAffects > 0)
                    {
                        _cache.Remove($"{_tableName}_Action_GetAll");
                        _cache.Remove($"{_tableName}_Action_GetById_{entityId}");
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return rowAffects;
        }

        /// <summary>
        /// Repo thêm mới bản ghi
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// Created by: nvdoan (19/04/2026)
        public async Task<int> InsertAsync(TEntity entity)
        {
            var rowAffects = 0;
            using var connection = CreateConnection();
            await connection.OpenAsync();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Code tự sinh ID
                    var keyName = _modelType.GetKeyName();
                    var keyProp = entity.GetType().GetProperty(keyName);
                    if (keyProp != null && keyProp.PropertyType == typeof(Guid))
                    {
                        var currentId = (Guid)(keyProp.GetValue(entity) ?? Guid.Empty);
                        if (currentId == Guid.Empty)
                        {
                            keyProp.SetValue(entity, Guid.NewGuid());
                        }
                    }

                    var parameters = MappingDbType(entity);

                    rowAffects = await connection.ExecuteAsync($"Proc_Insert{_tableName}", param: parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();

                    if (rowAffects > 0)
                    {
                        _cache.Remove($"{_tableName}_Action_GetAll");
                    }
                }
                catch (MySqlException ex) when (ex.Number == 1062)
                {
                    transaction.Rollback();
                    string msg = _tableName.ToLower() == "employee" ? "Mã nhân viên đã tồn tại" : "Mã đã tồn tại trong hệ thống";
                    throw new ValidateException(msg);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return rowAffects;
        }

        /// <summary>
        /// Repo cập nhật bản ghi
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// Created by: nvdoan (19/04/2026)
        public async Task<int> UpdateAsync(Guid entityId, TEntity entity)
        {
            var rowAffects = 0;
            using var connection = CreateConnection();
            await connection.OpenAsync();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var keyName = _modelType.GetKeyName();
                    entity.GetType().GetProperty(keyName).SetValue(entity, entityId);

                    var parameters = MappingDbType(entity);

                    rowAffects = await connection.ExecuteAsync($"Proc_Update{_tableName}", param: parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();

                    // Thu dọn chiến trường sau cập nhật
                    if (rowAffects > 0)
                    {
                        _cache.Remove($"{_tableName}_Action_GetAll");
                        _cache.Remove($"{_tableName}_Action_GetById_{entityId}");
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return rowAffects;
        }
        #endregion

        #region Method Paging & Mapping (Tạo dựng linh hoạt)
        /// <summary>
        /// Repo lấy danh sách thực thể theo paging, search, sort
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="search"></param>
        /// <param name="searchFields"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        /// Created by: nvdoan (19/04/2026)
        public async Task<(long Total, IEnumerable<TEntity> Data)> GetFilterPagingAsync(int pageSize, int pageIndex, string search, List<string> searchFields, string sort)
        {
            long total = 0;
            var data = Enumerable.Empty<TEntity>();

            string store = string.Format("Proc_{0}_FilterPaging", _tableName);
            var parameters = new DynamicParameters();
            parameters.Add("@v_pageIndex", pageIndex);
            parameters.Add("@v_pageSize", pageSize);
            parameters.Add("@v_search", search);
            parameters.Add("@v_sort", sort);
            parameters.Add("@v_searchFields", JsonSerializer.Serialize(searchFields));

            using var connection = CreateConnection();
            using var reader = await connection.QueryMultipleAsync(new CommandDefinition(store, parameters, commandType: CommandType.StoredProcedure));

            data = (await reader.ReadAsync<TEntity>()).ToList();
            total = await reader.ReadFirstAsync<long>();

            return (total, data);
        }

        /// <summary>
        /// Repo mapping DbType cho Dapper
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// Created by: nvdoan (19/04/2026)
        private DynamicParameters MappingDbType(TEntity entity)
        {
            var parameters = new DynamicParameters();
            try
            {
                var properties = entity.GetType().GetProperties().Where(p => p.DeclaringType != typeof(BaseModel));

                foreach (var property in properties)
                {
                    var propertyName = property.Name;
                    var propertyValue = property.GetValue(entity);
                    var propertyType = property.PropertyType;

                    if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                        parameters.Add($"@v_{propertyName}", propertyValue, DbType.String);
                    else
                        parameters.Add($"@v_{propertyName}", propertyValue);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi Mapping Db: {ex.Message}");
            }
            return parameters;
        }
        #endregion
    }
}
