using Microsoft.AspNetCore.Mvc;
using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Services.Interfaces;

namespace SalesDatePredictionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippersController : ControllerBase
    {
        private readonly IShipperService _shipperService;

        public ShippersController(IShipperService shipperService)
        {
            _shipperService = shipperService ?? throw new ArgumentNullException(nameof(shipperService));
        }

        // GET: api/shippers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipperDTO>>> GetShippers()
        {
            try
            {
                var shippers = await _shipperService.GetAllShippersAsync();
                return Ok(shippers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}