using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit; // O NUnit.Framework para NUnit o Microsoft.VisualStudio.TestTools.UnitTesting para MSTest
using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Repositories.Interfaces;
using SalesDatePrediction.API.Services.Implementations;
using SalesDatePredictionAPI.Services.Interfaces;

namespace SalesDatePrediction.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            // Inicializa el mock del repositorio
            _mockRepository = new Mock<IProductRepository>();
            _service = new ProductService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllProductsAsyncReturnsProductsList()
        {
            // Arrange
            var expectedProducts = new List<ProductDTO>
            {
                new ProductDTO { ProductId = 1, ProductName = "Test Product 1" },
                new ProductDTO { ProductId = 2, ProductName = "Test Product 2" }
            };

            // Configura el mock del repositorio para devolver datos de prueba
            _mockRepository.Setup(r => r.GetAllProductsAsync())
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(expectedProducts.Count, resultList.Count);
            Assert.Equal(expectedProducts[0].ProductId, resultList[0].ProductId);
            Assert.Equal(expectedProducts[0].ProductName, resultList[0].ProductName);
            Assert.Equal(expectedProducts[1].ProductId, resultList[1].ProductId);
            Assert.Equal(expectedProducts[1].ProductName, resultList[1].ProductName);

            // Verifica que el método del repositorio fue llamado exactamente una vez
            _mockRepository.Verify(r => r.GetAllProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllProductsAsync_WhenRepositoryReturnsEmpty_ReturnsEmptyList()
        {
            // Arrange
            var emptyList = new List<ProductDTO>();

            _mockRepository.Setup(r => r.GetAllProductsAsync())
                .ReturnsAsync(emptyList);

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.Empty(result);
            _mockRepository.Verify(r => r.GetAllProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllProductsAsync_WhenExceptionOccurs_PropagatesException()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllProductsAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.GetAllProductsAsync());
            Assert.Equal("Database error", exception.Message);
        }

        //[Fact]
        //public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        //{
        //    // Act & Assert
        //    var exception = Assert.Throws<ArgumentNullException>(() => new ProductService(null));
        //    Assert.Equal("productRepository", exception.ParamName);
        //}
    }
}