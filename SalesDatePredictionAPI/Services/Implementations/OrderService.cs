using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Models.Entities;
using SalesDatePredictionAPI.Repositories.Interfaces;
using SalesDatePredictionAPI.Services.Interfaces;

namespace SalesDatePredictionAPI.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        //public async Task<IEnumerable<OrderDTO>> GetOrdersByCustomerIdAsync(int customerId)
        //{
        //    // Verificar si el cliente existe
        //    var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
        //    if (customer == null)
        //    {
        //        throw new KeyNotFoundException($"Cliente con ID {customerId} no encontrado");
        //    }

        //    return await _orderRepository.GetOrdersByCustomerIdAsync(customerId);
        //}

        public async Task<PaginatedResponse<OrderDTO>> GetOrdersByCustomerIdAsync(int customerId, int pageNumber, int pageSize)
        {
            // Verificar si el cliente existe
            var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Cliente con ID {customerId} no encontrado");
            }

            // Validar parámetros de paginación
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10; // Valor predeterminado

            return await _orderRepository.GetOrdersByCustomerIdAsync(customerId, pageNumber, pageSize);
        }

        public async Task<int> CreateOrderAsync(NewOrderDTO newOrderDTO)
        {
            if (newOrderDTO == null)
            {
                throw new ArgumentNullException(nameof(newOrderDTO));
            }

            // Verificar si el cliente existe
            var customer = await _customerRepository.GetCustomerByIdAsync(newOrderDTO.CustomerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Cliente con ID {newOrderDTO.CustomerId} no encontrado");
            }

            // Verificar si el producto existe
            var product = await _productRepository.GetProductByIdAsync(newOrderDTO.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Producto con ID {newOrderDTO.ProductId} no encontrado");
            }

            // Crear la orden
            var order = new Order
            {
                CustomerId = newOrderDTO.CustomerId,
                EmployeeId = newOrderDTO.EmployeeId,
                ShipperId = newOrderDTO.ShipperId,
                OrderDate = newOrderDTO.OrderDate,
                RequiredDate = newOrderDTO.RequiredDate,
                ShippedDate = newOrderDTO.ShippedDate,
                Freight = newOrderDTO.Freight,
                ShipName = newOrderDTO.ShipName,
                ShipAddress = newOrderDTO.ShipAddress,
                ShipCity = newOrderDTO.ShipCity,
                ShipCountry = newOrderDTO.ShipCountry
            };

            // Crear el detalle de la orden
            var orderDetail = new OrderDetail
            {
                ProductId = newOrderDTO.ProductId,
                UnitPrice = newOrderDTO.UnitPrice,
                Quantity = newOrderDTO.Quantity,
                Discount = newOrderDTO.Discount
            };

            // Guardar la orden y obtener el ID generado
            return await _orderRepository.AddOrderAsync(order, orderDetail);
        }
    }
}
