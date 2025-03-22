using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Models.Entities;
using SalesDatePredictionAPI.Repositories.Interfaces;

namespace SalesDatePrediction.API.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        productid AS ProductId,
                        productname AS ProductName
                    FROM Production.Products";

                return await connection.QueryAsync<ProductDTO>(query);
            }
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        productid AS ProductId, 
                        productname AS ProductName,
                        supplierid AS SupplierId,
                        categoryid AS CategoryId,
                        unitprice AS UnitPrice,
                        discontinued AS Discontinued
                    FROM Production.Products
                    WHERE productid = @ProductId";

                return await connection.QuerySingleOrDefaultAsync<Product>(query, new { ProductId = productId });
            }
        }
    }
}
