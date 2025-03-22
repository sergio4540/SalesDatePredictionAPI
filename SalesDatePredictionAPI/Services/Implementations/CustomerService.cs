using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Models.Entities;
using SalesDatePredictionAPI.Repositories.Interfaces;
using SalesDatePredictionAPI.Services.Interfaces;

namespace SalesDatePredictionAPI.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }


        public async Task<PaginatedResponse<CustomerPredictionDTO>> GetCustomerPredictionsAsync(
           string customerNameFilter,
           int pageNumber,
           int pageSize,
           string sortBy = "Customer_Name",
           bool sortAscending = true)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var (items, totalCount) = await _customerRepository.GetCustomerPredictionsAsync(
                customerNameFilter,
                pageNumber,
                pageSize,
                sortBy,
                sortAscending);

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PaginatedResponse<CustomerPredictionDTO>
            {
                Data = items,
                Pagination = new PaginationInfo
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalItems = totalCount
                }
            };
        }


        //public async Task<IEnumerable<CustomerPredictionDTO>> GetCustomerPredictionsAsync()
        //{
        //    return await _customerRepository.GetCustomerPredictionsAsync();
        //}

        public async Task<bool> CustomerExistsAsync(int customerId)
        {
            return await _customerRepository.CustomerExistsAsync(customerId);
        }
    }
}
