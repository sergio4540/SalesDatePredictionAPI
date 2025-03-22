using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Repositories.Interfaces;
using SalesDatePredictionAPI.Services.Interfaces;

namespace SalesDatePrediction.API.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }
    }
}

