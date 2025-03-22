using SalesDatePredictionAPI.Models.DTOs;

namespace SalesDatePredictionAPI.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync();
    }
}
