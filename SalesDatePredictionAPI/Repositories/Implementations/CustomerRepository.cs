using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Models.Entities;
using SalesDatePredictionAPI.Repositories.Interfaces;

namespace SalesDatePrediction.API.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<(IEnumerable<CustomerPredictionDTO> items, int totalCount)> GetCustomerPredictionsAsync(
            string customerNameFilter,
            int pageNumber,
            int pageSize,
            string sortBy = "Customer_Name",
            bool sortAscending = true)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Obtenemos todos los resultados del SP
            var allPredictions = await connection.QueryAsync<CustomerPredictionDTO>("PredictNextOrder",
                commandType: CommandType.StoredProcedure);

            // Aplicamos filtro de nombre de cliente si es necesario
            if (!string.IsNullOrEmpty(customerNameFilter))
            {
                allPredictions = allPredictions.Where(p =>
                    p.Customer_Name.Contains(customerNameFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Contamos total de elementos para paginación
            var totalCount = allPredictions.Count();

            // Aplicamos ordenamiento
            IOrderedEnumerable<CustomerPredictionDTO> orderedPredictions;

            switch (sortBy)
            {
                case "Last_Order_Date":
                    orderedPredictions = sortAscending
                        ? allPredictions.OrderBy(p => p.Last_Order_Date)
                        : allPredictions.OrderByDescending(p => p.Last_Order_Date);
                    break;
                case "Next_Predicted_Order":
                    orderedPredictions = sortAscending
                        ? allPredictions.OrderBy(p => p.Next_Predicted_Order)
                        : allPredictions.OrderByDescending(p => p.Next_Predicted_Order);
                    break;
                case "Customer_Name":
                default:
                    orderedPredictions = sortAscending
                        ? allPredictions.OrderBy(p => p.Customer_Name)
                        : allPredictions.OrderByDescending(p => p.Customer_Name);
                    break;
            }

            // Aplicamos paginación
            var pagedResults = orderedPredictions
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (pagedResults, totalCount);
        }



        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "SELECT custid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax FROM Sales.Customers WHERE custid = @CustomerId";

                return await connection.QuerySingleOrDefaultAsync<Customer>(query, new { CustomerId = customerId });
            }
        }

        public async Task<bool> CustomerExistsAsync(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT COUNT(1) FROM Sales.Customers WHERE custid = @CustomerId";
                    command.Parameters.Add(new SqlParameter("@CustomerId", SqlDbType.Int) { Value = customerId });

                    var result = (int)await command.ExecuteScalarAsync();
                    return result > 0;
                }
            }
        }
    }
}
