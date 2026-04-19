using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IConfiguration configuration, IMemoryCache cache) : base(configuration, cache)
        {
        }

        /// <summary>
        /// Repo lấy dữ liệu nhân viên theo mã nhân viên
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// Created by: nvdoan (18/04/2026)
        public async Task<Employee> GetEmployeeByCode(string code)
        {
            string query = SQLExtension.GetQuery("Employee.GetByCode");
            var param = new Dictionary<string, object>
            {
                {"@EmployeeCode", code }
            };
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Repo lấy danh sách nhân viên theo phòng ban
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        /// Created by: nvdoan (18/04/2026)
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByDepartmentId");
            var param = new Dictionary<string, object>
            {
                {"@DepartmentID", departmentId }
            };
            using var connection = CreateConnection();
            return await connection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Repo lấy danh sách nhân viên theo vị trí công việc
        /// </summary>
        /// <param name="positionId"></param>
        /// <returns></returns>
        /// Created by: nvdoan (18/04/2026)
        public async Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByPositionId");
            var param = new Dictionary<string, object>
            {
                {"@PositionID", positionId }
            };
            using var connection = CreateConnection();
            return await connection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Lọc nhân viên theo các tiêu chí: phòng ban, vị trí công việc, mức lương, giới tính, ngày tuyển dụng
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="positionId"></param>
        /// <param name="salaryFrom"></param>
        /// <param name="salaryTo"></param>
        /// <param name="gender"></param>
        /// <param name="hireDateFrom"></param>
        /// <param name="hireDateTo"></param>
        /// <returns></returns>
        /// Created by: nvdoan (18/04/2026)
        public async Task<PagingResponse<Employee>> GetFilterEmployeesAsync(int pageSize, int pageIndex, Guid? departmentId, Guid? positionId, decimal? salaryFrom, decimal? salaryTo, int? gender, DateTime? hireDateFrom, DateTime? hireDateTo)
        {
            var param = new DynamicParameters();
            param.Add("@v_pageSize", pageSize);
            param.Add("@v_pageIndex", pageIndex);
            param.Add("@v_DepartmentID", departmentId);
            param.Add("@v_PositionID", positionId);
            param.Add("@v_SalaryFrom", salaryFrom);
            param.Add("@v_SalaryTo", salaryTo);
            param.Add("@v_Gender", gender);
            param.Add("@v_HireDateFrom", hireDateFrom);
            param.Add("@v_HireDateTo", hireDateTo);

            using var connection = CreateConnection();
            using var reader = await connection.QueryMultipleAsync("Proc_GetFilterEmployeesPaging", param, commandType: System.Data.CommandType.StoredProcedure);

            var data = (await reader.ReadAsync<Employee>()).ToList();
            var total = await reader.ReadFirstAsync<long>();

            return new PagingResponse<Employee>
            {
                Total = total,
                PageSize = pageSize,
                PageIndex = pageIndex,
                Data = data
            };
        }
    }
}