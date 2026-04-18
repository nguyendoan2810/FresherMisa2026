using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Interfaces.Services
{
    public interface IDepartmentSerice : IBaseService<Department>
    {
        /// <summary>
        /// Lấy department theo code
        /// </summary>
        /// <returns></returns>
        /// Created By: dvhai (10/04/2026)
        Task<Department> GetDepartmentByCodeAsync(string code);

        /// <summary>
        /// Lấy danh sách nhân viên theo mã phòng ban
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// Created By: nvdoan (18/04/2026)
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentCodeAsync(string code);

        /// <summary>
        /// Tính tổng số nhân viên trong phòng ban theo mã phòng ban
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// Created By: nvdoan (18/04/2026)
        Task<int> GetEmployeeCountByDepartmentCodeAsync(string code);
    }
}
