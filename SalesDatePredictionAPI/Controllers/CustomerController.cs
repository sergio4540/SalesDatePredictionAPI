using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Services.Interfaces;

namespace SalesDatePredictionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Gets all customers with their last order date and predicted next order date
        /// </summary>
        /// 
        [HttpGet]
        public async Task<IActionResult> GetCustomerPredictions(
           [FromQuery] string? customerName = null,
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10,
           [FromQuery] string sortBy = "CustomerName",
           [FromQuery] bool sortAscending = true)
        {
            var response = await _customerService.GetCustomerPredictionsAsync(
                customerName,
                pageNumber,
                pageSize,
                sortBy,
                sortAscending);

            return Ok(response);
        }
    }
}
