using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for Department entity
    /// </summary>
    /// Created By: dvhai (09/04/2026)
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(IConfiguration configuration) : base(configuration)
        {

        }

        /// <summary>
        /// Lấy department theo code
        /// </summary>
        /// <param name="code">Mã department</param>
        /// <returns>Department tìm thấy hoặc null</returns>
        /// CREATED BY: dvhai (09/04/2026)
        public async Task<Department> GetDepartmentByCode(string code)
        {
            string query = SQLExtension.GetQuery("Department.GetByCode");
            var @param = new Dictionary<string, object>
            {
                {"@DepartmentCode", code }
            };
            return await _dbConnection.QueryFirstOrDefaultAsync<Department>(query, @param, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Repo lấy tổng số nhân viên trong phòng ban theo mã phòng ban
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// Created By: nvdoan (18/04/2026)
        public async Task<int> GetEmployeeCountByDepartmentCodeAsync(string code)
        {
            var param = new DynamicParameters();
            param.Add("@v_DepartmentCode", code);
            return await _dbConnection.ExecuteScalarAsync<int>("Proc_GetEmployeeCountByDepartmentCode", param, commandType: System.Data.CommandType.StoredProcedure);
        }

        /// <summary>
        /// Repo lấy danh sách nhân viên theo mã phòng ban
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// Created By: nvdoan (18/04/2026)
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentCodeAsync(string code)
        {
            var param = new DynamicParameters();
            param.Add("@v_DepartmentCode", code);
            return await _dbConnection.QueryAsync<Employee>("Proc_GetEmployeesByDepartmentCode", param, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
