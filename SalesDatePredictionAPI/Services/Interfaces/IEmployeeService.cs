using SalesDatePredictionAPI.Models.DTOs;

namespace SalesDatePredictionAPI.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync();
    }
}
