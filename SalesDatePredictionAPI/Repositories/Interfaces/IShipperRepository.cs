using SalesDatePredictionAPI.Models.DTOs;

namespace SalesDatePredictionAPI.Repositories.Interfaces
{
    public interface IShipperRepository
    {
        Task<IEnumerable<ShipperDTO>> GetAllShippersAsync();
    }
}
