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
    public class CustomerControllerTests
    {
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly CustomerController _controller;

        public CustomerControllerTests()
        {
            _mockCustomerService = new Mock<ICustomerService>();
            _controller = new CustomerController(_mockCustomerService.Object);
        }

        [Fact]
        public async Task GetCustomerPredictions_ReturnsOkResult_WithCustomerPredictions()
        {
            // Arrange
            var mockResponse = new PaginatedResponse<CustomerPredictionDTO>
            {
                Data = new List<CustomerPredictionDTO>
                {
                    new CustomerPredictionDTO
                    {
                        Customer_Id = 1,
                        Customer_Name = "Test Customer",
                        Last_Order_Date = DateTime.Now.AddDays(-30),
                        Next_Predicted_Order = DateTime.Now.AddDays(15)
                    }
                },
                Pagination = new PaginationInfo
                {
                    CurrentPage = 1,
                    PageSize = 10,
                    TotalItems = 1,
                    TotalPages = 1
                }
            };

            _mockCustomerService.Setup(service => service.GetCustomerPredictionsAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetCustomerPredictions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PaginatedResponse<CustomerPredictionDTO>>(okResult.Value);
            Assert.Single(returnValue.Data);
            Assert.Equal(mockResponse.Data.First().Customer_Id, returnValue.Data.First().Customer_Id);
            Assert.Equal(mockResponse.Data.First().Customer_Name, returnValue.Data.First().Customer_Name);
        }

        [Fact]
        public async Task GetCustomerPredictions_CallsService_WithCorrectParameters()
        {
            // Arrange
            string customerName = "Test";
            int pageNumber = 2;
            int pageSize = 15;
            string sortBy = "Last_Order_Date";
            bool sortAscending = false;

            var mockResponse = new PaginatedResponse<CustomerPredictionDTO>
            {
                Data = new List<CustomerPredictionDTO>(),
                Pagination = new PaginationInfo
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalItems = 0,
                    TotalPages = 0
                }
            };

            _mockCustomerService.Setup(service => service.GetCustomerPredictionsAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
                .ReturnsAsync(mockResponse);

            // Act
            await _controller.GetCustomerPredictions(customerName, pageNumber, pageSize, sortBy, sortAscending);

            // Assert
            _mockCustomerService.Verify(service => service.GetCustomerPredictionsAsync(
                customerName,
                pageNumber,
                pageSize,
                sortBy,
                sortAscending), Times.Once);
        }

        [Fact]
        public async Task GetCustomerPredictions_UsesDefaultParameters_WhenNotProvided()
        {
            // Arrange
            var mockResponse = new PaginatedResponse<CustomerPredictionDTO>
            {
                Data = new List<CustomerPredictionDTO>(),
                Pagination = new PaginationInfo
                {
                    CurrentPage = 1,
                    PageSize = 10,
                    TotalItems = 0,
                    TotalPages = 0
                }
            };

            _mockCustomerService.Setup(service => service.GetCustomerPredictionsAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
                .ReturnsAsync(mockResponse);

            // Act
            await _controller.GetCustomerPredictions();

            // Assert
            _mockCustomerService.Verify(service => service.GetCustomerPredictionsAsync(
                null,
                1,
                10,
                "CustomerName",
                true), Times.Once);
        }

        [Fact]
        public async Task GetCustomerPredictions_ReturnsCorrectStatusCode_WhenSuccessful()
        {
            // Arrange
            var mockResponse = new PaginatedResponse<CustomerPredictionDTO>
            {
                Data = new List<CustomerPredictionDTO>(),
                Pagination = new PaginationInfo
                {
                    CurrentPage = 1,
                    PageSize = 10,
                    TotalItems = 0,
                    TotalPages = 0
                }
            };

            _mockCustomerService.Setup(service => service.GetCustomerPredictionsAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetCustomerPredictions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}