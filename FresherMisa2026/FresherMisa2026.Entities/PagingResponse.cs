using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities
{
    /// <summary>
    /// DTO chứa các thông tin trả cho client sau khi thực hiện nghiệp vụ lấy danh sách dữ liệu có phân trang
    /// , bao gồm tổng số bản ghi, kích thước trang, trang hiện tại và danh sách dữ liệu của trang đó
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagingResponse<T>
    {
        /// <summary>
        /// Tổng số bản ghi sau khi đã áp dụng các điều kiện tìm kiếm,
        /// lọc, sắp xếp (nếu có) để client biết được có bao nhiêu bản ghi
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// Kích thước trang (số bản ghi/trang) để client biết được có bao nhiêu bản ghi trên mỗi trang,
        /// từ đó có thể tính được có bao nhiêu trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Trang hiện tại để client biết được đang ở trang nào, từ đó có thể điều hướng sang các trang khác nếu muốn
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Danh sách dữ liệu của trang hiện tại sau khi đã áp dụng các điều kiện tìm kiếm,
        /// lọc, sắp xếp (nếu có) để client hiển thị ra giao diện
        /// </summary>
        public List<T> Data { get; set; }
    }
}
