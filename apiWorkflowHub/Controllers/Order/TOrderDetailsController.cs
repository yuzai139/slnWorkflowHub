using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiWorkflowHub.ContextModels;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using System.Web.Http.Cors;
using apiWorkflowHub.DTO.Order;

namespace apiWorkflowHub.Controllers.Order
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
    public class TOrderDetailsController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TOrderDetailsController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TOrderDetails
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetailDTO>>> GetTOrderDetails()
        {
            var orderdetails = await _context.TOrderDetails.ToListAsync();
            
            var orderdetailDTO = orderdetails.Select(e => new OrderDetailDTO
            {
                FOrderId =  e.FOrderId,
                
                FSubtotal = e.FSubtotal,
            }).ToList();

            return orderdetailDTO;
        }

        // GET: api/TOrderDetails/5
        [HttpGet("{OrderID}")]
        public async Task<ActionResult<OrderDetailDTO>> GetTOrderDetail(int OrderID)
        {
            var orderDetail = from s in _context.TOrderDetails
                              join t in _context.TSopProducts
                              on s.FOrderId equals t.FOrderId
                              where s.FOrderId == OrderID
                              select new OrderDetailDTO
                              {
                                  FOrderId = s.FOrderId,
                                  FSopid = s.FSopid,
                                  FSubtotal = s.FSubtotal
                              };
                         


            //.FirstOrDefaultAsync(e => e.FOrderId == OrderID);

            if (orderDetail == null)
            {
                return NotFound();
            }
            return Ok(orderDetail);
            // 轉換為 DTO
            //var orderDetailDTO = new OrderDetailDTO
            //{
            //    FOrderId = orderDetail.FOrderId,
            //    flsCopy = orderDetail.
            //    FSubtotal = orderDetail.FSubtotal,
            //};

            //return Ok(orderDetailDTO);
        }

        // PUT: api/TOrderDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{OrderID}")]
        public async Task<IActionResult> PutTOrderDetail(int OrderID, TOrderDetail tOrderDetail)
        {
            if (OrderID != tOrderDetail.FOrderId)
            {
                return BadRequest();
            }

            _context.Entry(tOrderDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TOrderDetailExists(OrderID))
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

        // POST: api/TOrderDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<ActionResult<TOrderDetail>> PostTOrderDetail(TOrderDetail tOrderDetail)
        {
            _context.TOrderDetails.Add(tOrderDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTOrderDetail", new { OrderID = tOrderDetail.FOrderId }, tOrderDetail);
        }

        // DELETE: api/TOrderDetails/5
        [HttpDelete("{OrderID}")]
        public async Task<IActionResult> DeleteTOrderDetail(int OrderID)
        {
            var tOrderDetail = await _context.TOrderDetails.FindAsync(OrderID);
            if (tOrderDetail == null)
            {
                return NotFound();
            }

            _context.TOrderDetails.Remove(tOrderDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TOrderDetailExists(int OrderID)
        {
            return _context.TOrderDetails.Any(e => e.FOrderId == OrderID);
        }
    }
}
