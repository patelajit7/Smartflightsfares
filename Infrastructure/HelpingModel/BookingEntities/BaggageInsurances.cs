namespace Infrastructure.HelpingModel.BookingEntities
{
    public class BaggageInsurances
    {
        public int BookingId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal TotalPrice { get; set; }
        public string ServiceNumber { get; set; }
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
