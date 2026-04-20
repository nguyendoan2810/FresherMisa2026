using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities
{
    /// <summary>
    /// DTO nhận các tham số phân trang, tìm kiếm, sắp xếp từ client gửi xuống khi có yêu cầu lấy danh sách dữ liệu có phân trang
    /// </summary>
    public class PagingRequest
    {
        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Kích thước trang (số bản ghi/trang)
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Từ khóa tìm kiếm
        /// </summary>
        public string Search { get; set; }

        /// <summary>
        /// Sắp xếp theo trường nào (có sắp xếp theo tăng dần và giảm dần)
        /// </summary>
        public string Sort { get; set; } //vd: +ModifiedDate

        /// <summary>
        /// Timf kiếm theo trường nào (có thể tìm kiếm nhiều trường cùng lúc, cách nhau bởi dấu phẩy)
        /// </summary>
        public string SearchFields { get; set; }
    }

}
