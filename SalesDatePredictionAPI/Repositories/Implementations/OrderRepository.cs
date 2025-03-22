using System.Data;
using Dapper;
using System.Data.SqlClient;
using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Models.Entities;
using SalesDatePredictionAPI.Repositories.Interfaces;

namespace SalesDatePrediction.API.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        //public async Task<IEnumerable<OrderDTO>> GetOrdersByCustomerIdAsync(int customerId)
        //{
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();

        //        var query = @"
        //            SELECT 
        //                orderid,
        //                requireddate,
        //                shippeddate,
        //                shipname,
        //                shipaddress,
        //                shipcity
        //            FROM Sales.Orders
        //            WHERE custid = @CustomerId
        //            ORDER BY orderdate DESC";

        //        return await connection.QueryAsync<OrderDTO>(query, new { CustomerId = customerId });
        //    }
        //}

        public async Task<PaginatedResponse<OrderDTO>> GetOrdersByCustomerIdAsync(int customerId, int page, int pageSize)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Consulta para obtener el total de registros
                var countQuery = "SELECT COUNT(*) FROM Sales.Orders WHERE custid = @CustomerId";
                int totalItems = await connection.ExecuteScalarAsync<int>(countQuery, new { CustomerId = customerId });

                // Consulta paginada
                var query = @"
            SELECT 
                orderid,
                requireddate,
                shippeddate,
                shipname,
                shipaddress,
                shipcity
            FROM Sales.Orders
            WHERE custid = @CustomerId
            ORDER BY orderdate DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

                int offset = (page - 1) * pageSize;

                var items = await connection.QueryAsync<OrderDTO>(query,
                    new
                    {
                        CustomerId = customerId,
                        Offset = offset,
                        PageSize = pageSize
                    });

                // Calcular el total de páginas
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                return new PaginatedResponse<OrderDTO>
                {
                    Data = items,
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalItems = totalItems,
                        TotalPages = totalPages
                    }
                };
            }
        }

        public async Task<int> AddOrderAsync(Order order, OrderDetail orderDetail)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Crear una transacción para garantizar la integridad
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insertar la orden
                        var orderInsertQuery = @"
                            INSERT INTO Sales.Orders (
                                custid, empid, shipperid, orderdate, requireddate, shippeddate,
                                freight, shipname, shipaddress, shipcity, shipcountry
                            ) 
                            VALUES (
                                @CustomerId, @EmployeeId, @ShipperId, @OrderDate, @RequiredDate, @ShippedDate,
                                @Freight, @ShipName, @ShipAddress, @ShipCity, @ShipCountry
                            );
                            SELECT CAST(SCOPE_IDENTITY() as int);";

                        int orderId = await connection.QuerySingleAsync<int>(orderInsertQuery, order, transaction);

                        // Asignar el ID de la orden al detalle
                        orderDetail.OrderId = orderId;

                        // Insertar el detalle de la orden
                        var orderDetailInsertQuery = @"
                            INSERT INTO Sales.OrderDetails (
                                orderid, productid, unitprice, qty, discount
                            ) 
                            VALUES (
                                @OrderId, @ProductId, @UnitPrice, @Quantity, @Discount
                            );";

                        await connection.ExecuteAsync(orderDetailInsertQuery, orderDetail, transaction);

                        // Confirmar la transacción
                        transaction.Commit();

                        return orderId;
                    }
                    catch
                    {
                        // Revertir la transacción en caso de error
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
