using SalesDatePredictionAPI.Models.DTOs;

namespace SalesDatePredictionAPI.Services.Interfaces
{
    public interface IShipperService
    {
        Task<IEnumerable<ShipperDTO>> GetAllShippersAsync();
    }
}
