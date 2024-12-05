namespace apiWorkflowHub.DTO.Order
{
    public class OrderWithDetailsViewModel
    {
        public OrderDTO Order { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}

