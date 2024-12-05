using Microsoft.AspNetCore.Cors;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Routing;

namespace apiWorkflowHub.DTO.Order
{
    public class OrderDetailDTO
    {
       

        public int? FOrderId { get; set; }

        public bool? flsCopy { get; set; }

        public decimal? FSubtotal { get; set; }
        // SOP 產品相關資訊

        public int? FSopid { get; set; }
    }
}

