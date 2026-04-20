using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities.Extensions
{
    public class ConfigTable : Attribute
    {
        /// <summary>
        /// Xóa mềm (soft delete) - có cột IsDeleted hay không,
        /// nếu có thì khi xóa sẽ chỉ đánh dấu là đã xóa mà không xóa bản ghi khỏi database,
        /// giúp bảo vệ dữ liệu và dễ dàng khôi phục khi cần thiết.
        /// </summary>
        public bool HasDeletedColumn { get; set; } = false;

        /// <summary>
        /// Ràng buộc duy nhất (không được trùng dữ liệu)
        /// </summary>
        public string UniqueColumns { get; set; } = string.Empty;

        /// <summary>
        /// Tên bảng trong database
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        public ConfigTable(string tableName = "", bool hasDeletedColumn = false, string uniqueColumns = "")
        {
            TableName = tableName;

            HasDeletedColumn = hasDeletedColumn;

            UniqueColumns = uniqueColumns;
        }
    }
}
