using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Models.Entities;

namespace SalesDatePredictionAPI.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        //Task<IEnumerable<OrderDTO>> GetOrdersByCustomerIdAsync(int customerId);

        Task<PaginatedResponse<OrderDTO>> GetOrdersByCustomerIdAsync(int customerId, int pageNumber, int pageSize);

        Task<int> AddOrderAsync(Order order, OrderDetail orderDetail);
    }
}
