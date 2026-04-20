using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities
{
    public class ServiceResponse
    {
        /// <summary>
        /// Trạng thái thành công hay thất bại của nghiệp vụ
        /// </summary>
        public bool IsSuccess { get; set; }
        
        /// <summary>
        /// Mã trạng thái của nghiệp vụ
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Dữ liệu trả về cho client sau khi thực hiện nghiệp vụ
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Thông báo lỗi cho client
        /// </summary>
        public object UserMessage { get; set; }

        /// <summary>
        /// Thông báo lỗi cho Dev
        /// </summary>
        public object DevMessage { get; set; }
    }
}
