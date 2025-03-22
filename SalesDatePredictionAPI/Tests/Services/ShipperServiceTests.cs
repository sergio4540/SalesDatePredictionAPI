using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit; // O NUnit.Framework para NUnit o Microsoft.VisualStudio.TestTools.UnitTesting para MSTest
using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Repositories.Interfaces;
using SalesDatePrediction.API.Services.Implementations;
using SalesDatePredictionAPI.Services.Interfaces;

namespace SalesDatePrediction.Tests.Services
{
    public class ShipperServiceTests
    {
        private readonly Mock<IShipperRepository> _mockRepository;
        private readonly ShipperService _service;

        public ShipperServiceTests()
        {
            // Inicializa el mock del repositorio en lugar del DbConnectionFactory
            _mockRepository = new Mock<IShipperRepository>();
            _service = new ShipperService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllShippersAsync_ReturnsShippersList()
        {
            // Arrange
            var expectedShippers = new List<ShipperDTO>
            {
                new ShipperDTO { ShipperId = 1, CompanyName = "Test Shipper" }
            };

            // Configura el mock del repositorio para devolver datos de prueba
            _mockRepository.Setup(r => r.GetAllShippersAsync())
                .ReturnsAsync(expectedShippers);

            // Act
            var result = await _service.GetAllShippersAsync();

            // Assert
            Assert.Equal(expectedShippers.Count, result.Count());
            Assert.Equal(expectedShippers[0].CompanyName, result.First().CompanyName);

            // Verifica que el método del repositorio fue llamado exactamente una vez
            _mockRepository.Verify(r => r.GetAllShippersAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllShippersAsync_WhenExceptionOccurs_PropagatesException()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllShippersAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetAllShippersAsync());
        }

        //[Fact]
        //public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        //{
        //    // Act & Assert
        //    Assert.Throws<ArgumentNullException>(() => new ShipperService(null));
        //}
    }
}