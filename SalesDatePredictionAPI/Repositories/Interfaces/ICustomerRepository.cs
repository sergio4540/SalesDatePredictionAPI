using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Models.Entities;

namespace SalesDatePredictionAPI.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<(IEnumerable<CustomerPredictionDTO> items, int totalCount)> GetCustomerPredictionsAsync(
           string customerNameFilter,
           int pageNumber,
           int pageSize,
           string sortBy = "CustomerName",
           bool sortAscending = true);


        Task<Customer> GetCustomerByIdAsync(int customerId);

        Task<bool> CustomerExistsAsync(int customerId);
    }
}
