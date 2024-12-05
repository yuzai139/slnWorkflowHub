using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiWorkflowHub.ContextModels;
using System.Web.Http;
using System.Web.Http.Cors;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using apiWorkflowHub.DTO.Order;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpDeleteAttribute = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;

namespace apiWorkflowHub.Controllers.Order
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]

    public class TOrdersController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TOrdersController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetTOrders()
        {
            var orders = await _context.TOrders.ToListAsync();

            var orderDTOs = orders.Select(e => new OrderDTO
            {

                fOrderId = e.FOrderId,
                fMemberId = e.FMemberId,
                fTotalPrice = e.FTotalPrice,
                //fOrderDate = e.FOrderDate,
                OrderDateDisplay = e.FOrderDate.Value.ToString("yyyy/MM/dd"),
                fOrderStatus = e.FOrderStatus,

                fPayment = e.FPayment
            }).ToList();

            return orderDTOs;
        }

        // GET: api/TOrders/5
        [HttpGet("{fOrderID}")]
        public async Task<ActionResult<TOrder>> GetTOrder(int fOrderID)
        {
            var tOrder = await _context.TOrders.FindAsync(fOrderID);

            if (tOrder == null)
            {
                return NotFound();
            }

            return tOrder;
        }

        // PUT: api/TOrders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{fOrderID}")]
        public async Task<IActionResult> PutTOrder(int fOrderID, TOrder tOrder)
        {
            if (fOrderID != tOrder.FOrderId)
            {
                return BadRequest();
            }

            _context.Entry(tOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TOrderExists(fOrderID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TOrders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TOrder>> PostTOrder(TOrder tOrder)
        {
            _context.TOrders.Add(tOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTOrder", new { fOrderID = tOrder.FOrderId }, tOrder);
        }

        [HttpPost("createWithDetails")]
        public async Task<ActionResult> CreateWithDetails([FromBody] OrderWithDetailsViewModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 建立訂單主檔
                    var order = new TOrder
                    {
                        FMemberId = model.Order.fMemberId,
                        FTotalPrice = model.Order.fTotalPrice,
                        FOrderDate = DateTime.Now,
                        FOrderStatus = true,
                        FPayment = model.Order.fPayment
                    };
                    _context.TOrders.Add(order);
                    await _context.SaveChangesAsync();

                    // 建立訂單明細和對應的 SOP 產品
                    foreach (var detail in model.OrderDetails)
                    {
                        // 建立訂單明細
                        var orderDetail = new TOrderDetail
                        {
                            FOrderId = order.FOrderId,
                            FSubtotal = detail.FSubtotal,
                            FIsCopy = false,
                            FSopid = detail.FSopid
                        };
                        _context.TOrderDetails.Add(orderDetail);
                        await _context.SaveChangesAsync();

                        // 建立 TSopProduct 記錄，並關聯到訂單明細
                        var sopProduct = new TSopProduct
                        {
                            FOrderId = order.FOrderId,
                            FSopid = detail.FSopid,
                            FPrice = detail.FSubtotal

                        };
                        _context.TSopProducts.Add(sopProduct);
                    }
                    await _context.SaveChangesAsync();

                    // 如果需要取得包含 FSopid 的訂單明細，可以使用以下查詢
                    var orderDetails = await _context.TOrderDetails
                        .Join(_context.TSopProducts,
                            od => od.FOrderId,
                            sp => sp.FOrderId,
                            (od, sp) => new
                            {
                                OrderDetailId = od.FOrdRecordId,
                                OrderId = od.FOrderId,
                                SopId = od.FSopid,
                                //FSOPID= od.FSopid,
                                Subtotal = od.FSubtotal ,
                                IsCopy = od.FIsCopy
                            })
                        .Where(x => x.OrderId == order.FOrderId).Distinct()
                        .ToListAsync();

                    await transaction.CommitAsync();
                    return Ok(new { 
                        orderId = order.FOrderId,
                        orderDetails = orderDetails
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { message = ex.Message });
                }
            }
        }

        // DELETE: api/TOrders/5
        [HttpDelete("{fOrderID}")]
        public async Task<IActionResult> DeleteTOrder(int fOrderID)
        {
            var tOrder = await _context.TOrders.FindAsync(fOrderID);
            if (tOrder == null)
            {
                return NotFound();
            }

            _context.TOrders.Remove(tOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TOrderExists(int fOrderID)
        {
            return _context.TOrders.Any(e => e.FOrderId == fOrderID);
        }

        // GET: api/TOrders/GetOrdersByMember/{memberId}
        [HttpGet("GetOrdersByMember/{fMemberID}")]
        public async Task<ActionResult<IEnumerable<TOrder>>> GetOrdersByMember(int fMemberID)
        {
            var orders = await _context.TOrders
                .Where(o => o.FMemberId == fMemberID)
                .OrderByDescending(o => o.FOrderDate)
                .ToListAsync();

            if (!orders.Any())
            {
                return NotFound($"找不到會員 ID {fMemberID} 的訂單");
            }

            return orders;
        }

        // GET: api/TOrders/GetOrdersByDateRange
        [HttpGet("GetOrdersByDateRange")]
        public async Task<ActionResult<IEnumerable<TOrder>>> GetOrdersByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var orders = await _context.TOrders
                .Where(o => o.FOrderDate >= startDate && o.FOrderDate <= endDate)
                .OrderByDescending(o => o.FOrderDate)
                .ToListAsync();

            return orders;
        }

        // PUT: api/TOrders/UpdateOrderStatus/5
        [HttpPut("UpdateOrderStatus/{fOrderID}")]
        public async Task<IActionResult> UpdateOrderStatus(int fOrderID, [FromBody] bool status)
        {
            var order = await _context.TOrders.FindAsync(fOrderID);

            if (order == null)
            {
                return NotFound();
            }

            order.FOrderStatus = status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "訂單狀態更新成功" });
        }

        
    }
}

