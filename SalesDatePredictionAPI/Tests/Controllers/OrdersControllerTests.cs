using Microsoft.AspNetCore.Mvc;
using Moq;
using SalesDatePredictionAPI.Controllers;
using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Models.Entities;
using SalesDatePredictionAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SalesDatePredictionAPI.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _controller = new OrdersController(_mockOrderService.Object);
        }

        [Fact]
        public async Task GetOrdersByCustomer_ReturnsOkResult_WithPaginatedOrders()
        {
            // Arrange
            int customerId = 1;
            int pageNumber = 1;
            int pageSize = 10;

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

            _mockOrderService.Setup(service => service.GetOrdersByCustomerIdAsync(customerId, pageNumber, pageSize))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetOrdersByCustomer(customerId, pageNumber, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PaginatedResponse<OrderDTO>>(okResult.Value);
            Assert.NotNull(returnValue.Data);
            Assert.NotNull(returnValue.Pagination);
            Assert.Equal(expectedResponse.Pagination.CurrentPage, returnValue.Pagination.CurrentPage);
            Assert.Equal(expectedResponse.Pagination.TotalItems, returnValue.Pagination.TotalItems);
        }

        [Fact]
        public async Task GetOrdersByCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            int customerId = 999;
            _mockOrderService.Setup(service => service.GetOrdersByCustomerIdAsync(customerId, It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException($"Customer with ID {customerId} not found"));

            // Act
            var result = await _controller.GetOrdersByCustomer(customerId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetOrdersByCustomer_ReturnsServerError_WhenExceptionOccurs()
        {
            // Arrange
            int customerId = 1;
            _mockOrderService.Setup(service => service.GetOrdersByCustomerIdAsync(customerId, It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetOrdersByCustomer(customerId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Internal server error", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreatedAtAction_WithOrderId()
        {
            // Arrange
            var newOrder = new NewOrderDTO
            {
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2
            };
            int expectedOrderId = 1;

            _mockOrderService.Setup(service => service.CreateOrderAsync(newOrder))
                .ReturnsAsync(expectedOrderId);

            // Act
            var result = await _controller.CreateOrder(newOrder);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(OrdersController.GetOrdersByCustomer), createdAtActionResult.ActionName);
            Assert.Equal(expectedOrderId, createdAtActionResult.Value);
            Assert.Equal(newOrder.CustomerId, ((object[])createdAtActionResult.RouteValues.Values)[0]);
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var newOrder = new NewOrderDTO();
            _controller.ModelState.AddModelError("CustomerId", "Required");

            // Act
            var result = await _controller.CreateOrder(newOrder);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateOrder_ReturnsServerError_WhenExceptionOccurs()
        {
            // Arrange
            var newOrder = new NewOrderDTO
            {
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2
            };

            _mockOrderService.Setup(service => service.CreateOrderAsync(newOrder))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.CreateOrder(newOrder);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Internal server error", statusCodeResult.Value.ToString());
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenOrderServiceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new OrdersController(null));
        }
    }
}