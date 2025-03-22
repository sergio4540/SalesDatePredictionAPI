namespace SalesDatePredictionAPI.Models.DTOs
{
    public class CustomerPredictionDTO
    {
        public string Customer_Name { get; set; }
        public DateTime Last_Order_Date { get; set; }
        public DateTime Next_Predicted_Order { get; set; }
        public int Customer_Id { get; set; }
    }
}
