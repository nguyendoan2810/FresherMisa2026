using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FresherMisa2026.Entities
{
    public class ValidateException : Exception
    {
        /// <summary>
        /// Hàm xử lý lỗi validate dữ liệu đầu vào
        /// </summary>
        /// <param name="message"></param>
        /// Created by: nvdoan (19/04/2026)
        public ValidateException(string message) : base(message) { }
    }
}
