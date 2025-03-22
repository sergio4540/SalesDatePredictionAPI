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
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        empid AS EmpId,
                        firstname + ' ' + lastname AS FullName
                    FROM HR.Employees";

                return await connection.QueryAsync<EmployeeDTO>(query);
            }
        }
    }
}
