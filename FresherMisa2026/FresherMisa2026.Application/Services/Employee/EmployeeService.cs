using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;

namespace FresherMisa2026.Application.Services
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(
            IBaseRepository<Employee> baseRepository,
            IEmployeeRepository employeeRepository
            ) : base(baseRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Lấy dữ liệu nhân viên theo mã nhân viên
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// Created by: nvdoan (18/04/2026)
        public async Task<Employee> GetEmployeeByCodeAsync(string code)
        {
            var employee = await _employeeRepository.GetEmployeeByCode(code);
            if (employee == null)
                throw new Exception("Employee not found");

            return employee;
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo phòng ban
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        /// Created by: nvdoan (18/04/2026)
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentIdAsync(Guid departmentId)
        {
            return await _employeeRepository.GetEmployeesByDepartmentId(departmentId);
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo vị trí công việc
        /// </summary>
        /// <param name="positionId"></param>
        /// <returns></returns>
        /// Created by: nvdoan (18/04/2026)
        public async Task<IEnumerable<Employee>> GetEmployeesByPositionIdAsync(Guid positionId)
        {
            return await _employeeRepository.GetEmployeesByPositionId(positionId);
        }

        /// <summary>
        /// Validate cho Employee entity
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        /// Created by: nvdoan (18/04/2026)
        protected override List<ValidationError> ValidateCustom(Employee employee)
        {
            var errors = new List<ValidationError>();

            if (!string.IsNullOrEmpty(employee.EmployeeCode) && employee.EmployeeCode.Length > 20)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự"));
            }

            if (string.IsNullOrEmpty(employee.EmployeeName))
            {
                errors.Add(new ValidationError("EmployeeName", "Tên nhân viên không được để trống"));
            }

            // 1. Kiểm tra mã nhân viên không được trùng lặp
            if (!string.IsNullOrEmpty(employee.EmployeeCode))
            {
                // Vì ValidateCustom là hàm đồng bộ (sync), dùng GetAwaiter().GetResult() để gọi qua Repository
                var duplicate = _employeeRepository.GetEmployeeByCode(employee.EmployeeCode).GetAwaiter().GetResult();

                // Nếu tìm thấy một người khác cũng có mã tương tự trong CSDL
                if (duplicate != null && duplicate.EmployeeID != employee.EmployeeID)
                {
                    errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại"));
                }
            }

            // 2. Email đúng định dạng
            if (!string.IsNullOrEmpty(employee.Email))
            {
                var emailregex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailregex.IsMatch(employee.Email))
                {
                    errors.Add(new ValidationError("Email", "Email không đúng định dạng"));
                }
            }

            // 3. Số điện thoại đúng định dạng (10 số và bắt đầu bằng số 0)
            if (!string.IsNullOrEmpty(employee.PhoneNumber))
            {
                var phoneregex = new System.Text.RegularExpressions.Regex(@"^0\d{9}$");
                if (!phoneregex.IsMatch(employee.PhoneNumber))
                {
                    errors.Add(new ValidationError("PhoneNumber", "Số điện thoại không đúng định dạng"));
                }
            }

            // 4. Ngày sinh không được lớn hơn ngày hiện tại
            if (employee.DateOfBirth.HasValue)
            {
                if (employee.DateOfBirth.Value.Date >= DateTime.Now.Date)
                {
                    errors.Add(new ValidationError("DateOfBirth", "Ngày sinh phải nhỏ hơn ngày hiện tại"));
                }
            }

            return errors;
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
            return await _employeeRepository.GetFilterEmployeesAsync(pageSize, pageIndex, departmentId, positionId, salaryFrom, salaryTo, gender, hireDateFrom, hireDateTo);
        }
    }
}