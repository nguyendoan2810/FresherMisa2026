using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;

namespace FresherMisa2026.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Task<Employee> GetEmployeeByCode(string code);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId);
        Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId);

        // Task 2.2
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
        Task<IEnumerable<Employee>> GetFilterEmployeesAsync(
            Guid? departmentId, Guid? positionId, decimal? salaryFrom, decimal? salaryTo,
            int? gender, DateTime? hireDateFrom, DateTime? hireDateTo);
    }
}
