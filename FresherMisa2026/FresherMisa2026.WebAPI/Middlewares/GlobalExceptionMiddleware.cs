using FresherMisa2026.Entities;
using System.Net;
using System.Text.Json;

namespace FresherMisa2026.WebAPI.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                Console.WriteLine("Before run middleware");
                // Pass the request to the next middleware/component
                await _next(context);
                Console.WriteLine("After run middleware");
            }
            catch (Exception ex)
            {
                // Handle the exception globally
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Hàm xử lý lỗi toàn cục
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        /// Created by: nvdoan (19/04/2026)
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = new ServiceResponse();

            // Thiết lập header trả về là JSON để client có thể hiểu được định dạng dữ liệu trả về
            context.Response.ContentType = "application/json";

            // Nếu lỗi validate dữ liệu đầu vào thì báo lỗi 400,
            // còn các lỗi khác sẽ báo lỗi Server 500 để khách hàng liên hệ với Misa xử lý
            if (exception is ValidateException)
            {
                context.Response.StatusCode = 400; // 400 (Bad Request - Lỗi do người dùng)
                response.IsSuccess = false;
                response.Code = 400;
                response.UserMessage = exception.Message;
            }
            else
            {
                // Còn vấp các lỗi đứt cáp khác thì báo lỗi Server 500
                context.Response.StatusCode = 500;
                response.IsSuccess = false;
                response.Code = 500;
                response.UserMessage = "Có lỗi xảy ra vui lòng liên hệ Misa!";
                response.DevMessage = exception.Message;
            }
            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}

