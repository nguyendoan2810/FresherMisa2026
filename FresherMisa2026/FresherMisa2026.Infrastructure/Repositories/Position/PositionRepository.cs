using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Position;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class PositionRepository : BaseRepository<Position>, IPositionRepository
    {
        public PositionRepository(IConfiguration configuration, IMemoryCache cache) : base(configuration, cache)
        {
        }

        /// <summary>
        /// Lấy position theo code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// Created by: nvdoan (19/04/2026)
        public async Task<Position> GetPositionByCode(string code)
        {
            string query = SQLExtension.GetQuery("Position.GetByCode");
            var param = new Dictionary<string, object>
            {
                {"@PositionCode", code }
            };
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Position>(query, param, commandType: System.Data.CommandType.Text);
        }
    }
}