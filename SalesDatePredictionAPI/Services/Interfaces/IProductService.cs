using SalesDatePredictionAPI.Models.DTOs;

namespace SalesDatePredictionAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
    }
}
