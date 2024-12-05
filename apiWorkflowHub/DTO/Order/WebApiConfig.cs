namespace apiWorkflowHub.DTO.Order
{
    public class WebApiConfig
    {

        public int FId { get; set; }
        public int fOrderID { get; set; }
        public int fMemberID { get; set; }
        public Decimal fTotalPrice { get; set; }

        public DateTime fOrderDate { get; set; }
        public bool fOrderStatus { get; set; }
        public string fPayment { get; set; }
    }
}
