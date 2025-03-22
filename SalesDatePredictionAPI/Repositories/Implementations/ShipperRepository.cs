using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Repositories.Interfaces;

namespace SalesDatePrediction.API.Repositories.Implementations
{
    public class ShipperRepository : IShipperRepository
    {
        private readonly string _connectionString;

        public ShipperRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IEnumerable<ShipperDTO>> GetAllShippersAsync()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        shipperid AS ShipperId,
                        companyname AS CompanyName
                    FROM Sales.Shippers";

                return await connection.QueryAsync<ShipperDTO>(query);
            }
        }
    }
}
