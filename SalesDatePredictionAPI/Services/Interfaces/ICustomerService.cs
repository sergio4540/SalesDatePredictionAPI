using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Models.Entities;

namespace SalesDatePredictionAPI.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<PaginatedResponse<CustomerPredictionDTO>> GetCustomerPredictionsAsync(
            string customerNameFilter,
            int pageNumber,
            int pageSize,
            string sortBy = "CustomerName",
            bool sortAscending = true);


        Task<bool> CustomerExistsAsync(int customerId);
    }
}
