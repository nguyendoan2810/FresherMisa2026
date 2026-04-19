using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using Microsoft.AspNetCore.Mvc;

namespace FresherMisa2026.WebAPI.Controllers
{
    [ApiController]
    public class EmployeesController : BaseController<Employee>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(
            IEmployeeService employeeService) : base(employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Lấy dữ liệu nhân viên theo mã nhân viên
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// Created by: nvdoan (18/04/2026)
        [HttpGet("Code/{code}")]
        public async Task<ActionResult<ServiceResponse>> GetByCode(string code)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeeByCodeAsync(code);
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Lâý danh sách nhân viên theo phòng ban
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        /// Created by: nvdoan (18/04/2026)
        [HttpGet("Department/{departmentId}")]
        public async Task<ActionResult<ServiceResponse>> GetByDepartmentId(Guid departmentId)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeesByDepartmentIdAsync(departmentId);
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo vị trí công việc
        /// </summary>
        /// <param name="positionId"></param>
        /// <returns></returns>
        /// Created by: nvdoan (18/04/2026)
        [HttpGet("Position/{positionId}")]
        public async Task<ActionResult<ServiceResponse>> GetByPositionId(Guid positionId)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeesByPositionIdAsync(positionId);
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// API lọc nhân viên theo các tiêu chí: phòng ban, vị trí công việc, mức lương, giới tính, ngày tuyển dụng
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
        [HttpGet("Filter")]
        public async Task<ActionResult<ServiceResponse>> GetFilterEmployee(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 1,
            [FromQuery] Guid? departmentId = null,
            [FromQuery] Guid? positionId = null,
            [FromQuery] decimal? salaryFrom = null,
            [FromQuery] decimal? salaryTo = null,
            [FromQuery] int? gender = null,
            [FromQuery] DateTime? hireDateFrom = null,
            [FromQuery] DateTime? hireDateTo = null)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetFilterEmployeesAsync(pageSize, pageIndex, departmentId, positionId, salaryFrom, salaryTo, gender, hireDateFrom, hireDateTo);
            response.IsSuccess = true;
            response.Code = 200;

            return Ok(response);
        }
    }
}