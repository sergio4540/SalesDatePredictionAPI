using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SalesDatePredictionAPI.Models.DTOs;
using SalesDatePredictionAPI.Repositories.Interfaces;
using SalesDatePredictionAPI.Services.Interfaces;

namespace SalesDatePrediction.API.Services.Implementations
{
    public class ShipperService : IShipperService
    {
        private readonly IShipperRepository _shipperRepository;

        public ShipperService(IShipperRepository shipperRepository)
        {
            _shipperRepository = shipperRepository ?? throw new ArgumentNullException(nameof(shipperRepository));
        }

        public async Task<IEnumerable<ShipperDTO>> GetAllShippersAsync()
        {
            return await _shipperRepository.GetAllShippersAsync();
        }
    }
}

