namespace SalesDatePredictionAPI.Models.Entities
{
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        public PaginationInfo Pagination { get; set; }
    }
}
