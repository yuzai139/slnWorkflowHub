using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiWorkflowHub.ContextModels;

namespace apiWorkflowHub.DTO.Order
{
    [Route("api/[controller]")]
    [ApiController]
    public class TShoppingCartsController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TShoppingCartsController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TShoppingCarts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TShoppingCart>>> GetTShoppingCarts()
        {
            return await _context.TShoppingCarts.ToListAsync();
        }

        // GET: api/TShoppingCarts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TShoppingCart>> GetTShoppingCart(int id)
        {
            var tShoppingCart = await _context.TShoppingCarts.FindAsync(id);

            if (tShoppingCart == null)
            {
                return NotFound();
            }

            return tShoppingCart;
        }

        // PUT: api/TShoppingCarts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTShoppingCart(int id, TShoppingCart tShoppingCart)
        {
            if (id != tShoppingCart.FCartId)
            {
                return BadRequest();
            }

            _context.Entry(tShoppingCart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TShoppingCartExists(id))
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

        // POST: api/TShoppingCarts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TShoppingCart>> PostTShoppingCart(TShoppingCart tShoppingCart)
        {
            _context.TShoppingCarts.Add(tShoppingCart);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTShoppingCart", new { id = tShoppingCart.FCartId }, tShoppingCart);
        }

        // DELETE: api/TShoppingCarts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTShoppingCart(int id)
        {
            var tShoppingCart = await _context.TShoppingCarts.FindAsync(id);
            if (tShoppingCart == null)
            {
                return NotFound();
            }

            _context.TShoppingCarts.Remove(tShoppingCart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TShoppingCartExists(int id)
        {
            return _context.TShoppingCarts.Any(e => e.FCartId == id);
        }
    }
}
