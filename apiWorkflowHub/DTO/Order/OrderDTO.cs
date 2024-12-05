using Microsoft.AspNetCore.Cors;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Routing;

public class OrderDTO
{
    public int? fOrderId { get; set; }
    public int? fMemberId { get; set; }
    public decimal? fTotalPrice { get; set; }
    public DateTime? fOrderDate { get; set; }
    public string? OrderDateDisplay { get; set; }
    public bool? fOrderStatus { get; set; }
    public string? fPayment { get; set; }

    // 訂單項目列表

}
