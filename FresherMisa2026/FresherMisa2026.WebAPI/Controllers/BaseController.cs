using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using static Dapper.SqlMapper;

namespace FresherMisa2026.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BaseController<TEntity> : ControllerBase
    {

        private readonly IBaseService<TEntity> _baseService;

        public BaseController(IBaseService<TEntity> baseService)
        {
            _baseService = baseService;
        }


        /// <summary>
        /// Danh sách
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<ServiceResponse> Get()
        {
            var response = new ServiceResponse();
            response.Data = await _baseService.GetEntities();
            response.IsSuccess = true;
            return response;
        }

        /// <summary>
        /// Một phần tử
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet("{ID}")]
        public async Task<ServiceResponse> GetByID(Guid ID)
        {
            var response = new ServiceResponse();
            response.Data = await _baseService.GetEntityByID(ID);
            response.IsSuccess = true;
            return response;
        }

        /// <summary>
        /// Xóa một phần tử
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpDelete("{ID}")]
        public async Task<ServiceResponse> DeleteByID(Guid ID)
        {
            var response = new ServiceResponse();
            bool success = await _baseService.DeleteByID(ID);
            response.IsSuccess = success;
            response.Data = ID;
            return response;
        }

        /// <summary>
        /// Thêm một thực thể mới
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Sô bản ghi bị ảnh hưởng</returns>
        /// CreatedBy: DVHAI 07/07/2026
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TEntity entity)
        {
            var serviceResult = new ServiceResponse();
            try
            {
                serviceResult = await _baseService.Insert(entity);
                if (!serviceResult.IsSuccess)
                    return BadRequest(serviceResult);

                return StatusCode((int)ResponseCode.Created, serviceResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Sửa một thực thể
        /// </summary>
        /// <param name="id">id của bản ghi</param>
        /// <param name="entity">thông tin của bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CreatedBy: DVHAI 07/07/2021
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] string id, [FromBody] TEntity entity)
        {
            var serviceResult = await _baseService.Update(Guid.Parse(id), entity);

            if (!serviceResult.IsSuccess)
                return StatusCode(StatusCodes.Status400BadRequest, serviceResult);
            else if (serviceResult.Code == (int)ResponseCode.InternalServerError)
                return StatusCode(StatusCodes.Status500InternalServerError, serviceResult);
            else if (serviceResult.Code == (int)ResponseCode.NotFound)
                return StatusCode(StatusCodes.Status404NotFound, serviceResult);

            return Ok(serviceResult);
        }
    }
}
