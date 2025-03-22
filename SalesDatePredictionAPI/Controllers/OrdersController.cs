using Microsoft.AspNetCore.Mvc;
using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Models.Entities;
using SalesDatePredictionAPI.Services.Interfaces;

namespace SalesDatePredictionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        // GET: api/orders/customer/{customerId}
        //[HttpGet("customer/{customerId}")]
        //public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByCustomer(int customerId)
        //{
        //    try
        //    {
        //        var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);
        //        return Ok(orders);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

        // GET: api/orders/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<PaginatedResponse<OrderDTO>>> GetOrdersByCustomer(
            int customerId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagedOrders = await _orderService.GetOrdersByCustomerIdAsync(customerId, pageNumber, pageSize);
                return Ok(pagedOrders);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<int>> CreateOrder([FromBody] NewOrderDTO orderCreation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var orderId = await _orderService.CreateOrderAsync(orderCreation);
                return CreatedAtAction(nameof(GetOrdersByCustomer), new { customerId = orderCreation.CustomerId }, orderId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}