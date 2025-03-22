using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Models.Entities;

namespace SalesDatePredictionAPI.Services.Interfaces
{
    public interface IOrderService
    {
        //Task<IEnumerable<OrderDTO>> GetOrdersByCustomerIdAsync(int customerId);

        Task<PaginatedResponse<OrderDTO>> GetOrdersByCustomerIdAsync(int customerId, int pageNumber, int pageSize);
        Task<int> CreateOrderAsync(NewOrderDTO newOrderDTO);
    }
}
