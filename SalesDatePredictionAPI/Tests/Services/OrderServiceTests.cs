using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Models.Entities;
using SalesDatePredictionAPI.Repositories.Interfaces;
using SalesDatePredictionAPI.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

namespace SalesDatePredictionAPI.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockProductRepository = new Mock<IProductRepository>();

            _orderService = new OrderService(
                _mockOrderRepository.Object,
                _mockCustomerRepository.Object,
                _mockProductRepository.Object
            );
        }

        [Fact]
        public async Task GetOrdersByCustomerId_ReturnsOrders_WhenCustomerExists()
        {
            // Arrange
            int customerId = 1;
            int pageNumber = 1;
            int pageSize = 10;

            var customer = new Customer
            {
                CustId = customerId,
                CompanyName = "Test Company",
                ContactName = "John Doe",
                ContactTitle = "Owner",
                Address = "123 Test St",
                City = "Test City",
                Country = "Test Country",
                Phone = "555-1234"
            };

            var expectedResponse = new PaginatedResponse<OrderDTO>
            {
                Data = new List<OrderDTO>
                {
                    new OrderDTO
                    {
                        OrderId = 1,
                        RequiredDate = DateTime.Now.AddDays(5),
                        ShippedDate = null,
                        ShipName = "Test Customer",
                        ShipAddress = "123 Test St",
                        ShipCity = "Test City"
                    }
                },
                Pagination = new PaginationInfo
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalPages = 1,
                    TotalItems = 1
                }
            };

            _mockCustomerRepository.Setup(repo => repo.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(customer);

            _mockOrderRepository.Setup(repo => repo.GetOrdersByCustomerIdAsync(customerId, pageNumber, pageSize))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _orderService.GetOrdersByCustomerIdAsync(customerId, pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Pagination);
            Assert.Equal(expectedResponse.Pagination.CurrentPage, result.Pagination.CurrentPage);
            Assert.Equal(expectedResponse.Pagination.TotalItems, result.Pagination.TotalItems);

            _mockCustomerRepository.Verify(repo => repo.GetCustomerByIdAsync(customerId), Times.Once);
            _mockOrderRepository.Verify(repo => repo.GetOrdersByCustomerIdAsync(customerId, pageNumber, pageSize), Times.Once);
        }

        [Fact]
        public async Task GetOrdersByCustomerId_ThrowsKeyNotFoundException_WhenCustomerDoesNotExist()
        {
            // Arrange
            int customerId = 999;
            int pageNumber = 1;
            int pageSize = 10;

            _mockCustomerRepository.Setup(repo => repo.GetCustomerByIdAsync(customerId))
                .ReturnsAsync((Customer)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _orderService.GetOrdersByCustomerIdAsync(customerId, pageNumber, pageSize));

            _mockCustomerRepository.Verify(repo => repo.GetCustomerByIdAsync(customerId), Times.Once);
            _mockOrderRepository.Verify(repo =>
                repo.GetOrdersByCustomerIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [InlineData(0, 10, 1, 10)]
        [InlineData(1, 0, 1, 10)]
        [InlineData(0, 0, 1, 10)]
        [InlineData(-1, 5, 1, 10)]
        public async Task GetOrdersByCustomerId_NormalizesInvalidPaginationParameters(
            int inputPageNumber, int inputPageSize,
            int expectedPageNumber, int expectedPageSize)
        {
            // Arrange
            int customerId = 1;
            var customer = new Customer
            {
                CustId = customerId,
                CompanyName = "Test Company",
                ContactName = "John Doe"
            };

            _mockCustomerRepository.Setup(repo => repo.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(customer);

            _mockOrderRepository.Setup(repo =>
                repo.GetOrdersByCustomerIdAsync(customerId, expectedPageNumber, expectedPageSize))
                .ReturnsAsync(new PaginatedResponse<OrderDTO>());

            // Act
            await _orderService.GetOrdersByCustomerIdAsync(customerId, inputPageNumber, inputPageSize);

            // Assert
            _mockOrderRepository.Verify(repo =>
                repo.GetOrdersByCustomerIdAsync(customerId, expectedPageNumber, expectedPageSize), Times.Once);
        }

        [Fact]
        public async Task CreateOrder_ReturnsOrderId_WhenSuccessful()
        {
            // Arrange
            int customerId = 1;
            int productId = 10;
            int expectedOrderId = 100;

            var newOrderDTO = new NewOrderDTO
            {
                CustomerId = customerId,
                ProductId = productId,
                EmployeeId = 5,
                OrderDate = DateTime.Now,
                RequiredDate = DateTime.Now.AddDays(3),
                ShipName = "Test Customer",
                ShipAddress = "123 Test St",
                ShipCity = "Test City",
                Quantity = 2,
                UnitPrice = 10.99m
            };

            var customer = new Customer
            {
                CustId = customerId,
                CompanyName = "Test Company",
                ContactName = "John Doe"
            };
            var product = new Product { ProductId = productId, ProductName = "Test Product" };

            _mockCustomerRepository.Setup(repo => repo.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(customer);

            _mockProductRepository.Setup(repo => repo.GetProductByIdAsync(productId))
                .ReturnsAsync(product);

            _mockOrderRepository.Setup(repo => repo.AddOrderAsync(It.IsAny<Order>(), It.IsAny<OrderDetail>()))
                .ReturnsAsync(expectedOrderId);

            // Act
            var result = await _orderService.CreateOrderAsync(newOrderDTO);

            // Assert
            Assert.Equal(expectedOrderId, result);

            _mockCustomerRepository.Verify(repo => repo.GetCustomerByIdAsync(customerId), Times.Once);
            _mockProductRepository.Verify(repo => repo.GetProductByIdAsync(productId), Times.Once);
            _mockOrderRepository.Verify(repo => repo.AddOrderAsync(
                It.Is<Order>(o => o.CustomerId == customerId),
                It.Is<OrderDetail>(od => od.ProductId == productId && od.Quantity == newOrderDTO.Quantity)),
                Times.Once);
        }

        [Fact]
        public async Task CreateOrder_ThrowsKeyNotFoundException_WhenCustomerDoesNotExist()
        {
            // Arrange
            int customerId = 999;
            var newOrderDTO = new NewOrderDTO { CustomerId = customerId, ProductId = 1 };

            _mockCustomerRepository.Setup(repo => repo.GetCustomerByIdAsync(customerId))
                .ReturnsAsync((Customer)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _orderService.CreateOrderAsync(newOrderDTO));

            _mockCustomerRepository.Verify(repo => repo.GetCustomerByIdAsync(customerId), Times.Once);
            _mockProductRepository.Verify(repo => repo.GetProductByIdAsync(It.IsAny<int>()), Times.Never);
            _mockOrderRepository.Verify(repo =>
                repo.AddOrderAsync(It.IsAny<Order>(), It.IsAny<OrderDetail>()), Times.Never);
        }

        [Fact]
        public async Task CreateOrder_ThrowsKeyNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            int customerId = 1;
            int productId = 999;
            var newOrderDTO = new NewOrderDTO { CustomerId = customerId, ProductId = productId };

            var customer = new Customer
            {
                CustId = customerId,
                CompanyName = "Test Company",
                ContactName = "John Doe"
            };

            _mockCustomerRepository.Setup(repo => repo.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(customer);

            _mockProductRepository.Setup(repo => repo.GetProductByIdAsync(productId))
                .ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _orderService.CreateOrderAsync(newOrderDTO));

            _mockCustomerRepository.Verify(repo => repo.GetCustomerByIdAsync(customerId), Times.Once);
            _mockProductRepository.Verify(repo => repo.GetProductByIdAsync(productId), Times.Once);
            _mockOrderRepository.Verify(repo =>
                repo.AddOrderAsync(It.IsAny<Order>(), It.IsAny<OrderDetail>()), Times.Never);
        }

        [Fact]
        public async Task CreateOrder_ThrowsArgumentNullException_WhenOrderDTOIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _orderService.CreateOrderAsync(null));

            _mockCustomerRepository.Verify(repo => repo.GetCustomerByIdAsync(It.IsAny<int>()), Times.Never);
            _mockProductRepository.Verify(repo => repo.GetProductByIdAsync(It.IsAny<int>()), Times.Never);
            _mockOrderRepository.Verify(repo =>
                repo.AddOrderAsync(It.IsAny<Order>(), It.IsAny<OrderDetail>()), Times.Never);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenOrderRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new OrderService(
                null,
                _mockCustomerRepository.Object,
                _mockProductRepository.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenCustomerRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new OrderService(
                _mockOrderRepository.Object,
                null,
                _mockProductRepository.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenProductRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new OrderService(
                _mockOrderRepository.Object,
                _mockCustomerRepository.Object,
                null));
        }
    }
}