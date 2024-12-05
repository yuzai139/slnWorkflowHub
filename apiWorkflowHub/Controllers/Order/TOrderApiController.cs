using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace apiWorkflowHub.Controllers.Order
{
    [Route("api/[controller]")]
    [ApiController]
    public class TOrderApiController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTOrders()
        {
            // 執行某些操作
            return Ok();
        }
    }
}
