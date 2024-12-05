using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiWorkflowHub.ContextModels;
using apiWorkflowHub.DTO.LectureAndPublisher;
using Microsoft.AspNetCore.Cors;

namespace apiWorkflowHub.Controllers.LectureAndPublisher
{
    [Route("api/[controller]")]
    [EnableCors("ALL")]//一定要加入[EnableCors("ALL")]
    [ApiController]
    public class TSopsController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TSopsController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TSops
        //列出全部SOP商品，首頁用
        [HttpGet]
        public IEnumerable<homeDTO> GetTSops()
        {

            var homeComponentList = from s in _context.TSops
                                    where s.FSopType == 2
                                    select new homeDTO
                                    {
                                        fSOPID = s.FSopid,
                                        fSopName = s.FSopName,
                                        fPubSopImagePath = s.FPubSopImagePath,
                                        fPrice = s.FPrice,
                                        fSalePoints = s.FSalePoints,
                                        fJobItemId = s.FJobItemId,
                                        fIndustryId = s.FIndustryId,
                                        fCompanySize = s.FCompanySize,
                                        fReleaseTime = s.FReleaseTime,
                                        fPubSopImageUrl = Path.Combine("D:\\iSPAN\\專題\\BackStage\\prjWorkflowHubAdmin\\wwwroot\\Workflow\\PublishImages\\",
                                        //到時候要改
                                        $"{s.FPubSopImagePath}")
                                    };

            foreach (var item in homeComponentList)
            {
                string filePath = item.fPubSopImageUrl;
                if (System.IO.File.Exists(filePath))
                {
                    var imageBytes = System.IO.File.ReadAllBytes(filePath);
                    var base64String = Convert.ToBase64String(imageBytes);
                    item.fPassImage = base64String;
                }
            }

            //if (System.IO.File.Exists(filePath))
            //{
            //    var imageBytes = System.IO.File.ReadAllBytes(filePath);
            //    var base64String = Convert.ToBase64String(imageBytes);
            //    TSop.FPubImagePath = base64String;
            //}

            //return _context.TSops.ToList();
            return homeComponentList;

        }

        // GET: api/TSops/5
        [HttpGet("{sopId}")]
        public async Task<ActionResult<workFlowProdDTO>> GetTSop(int sopId)
        {
            var sopById = (from s in _context.TSops
                           join p in _context.TPublishers
                           on s.FPublisherId equals p.FPublisherId
                           select new workFlowProdDTO
                           {
                               fSOPID = s.FSopid,
                               fPubId = p.FPublisherId,
                               fSopName = s.FSopName,
                               fPubName = p.FPubName,
                               FPubContent = s.FPubContent,
                               fPubSopImagePath = s.FPubSopImagePath,
                               fPrice = s.FPrice,
                               fSalePoints = s.FSalePoints,
                               fReleaseStatus = s.FReleaseStatus,
                               fJobItemId = s.FJobItemId,
                               fIndustryId = s.FIndustryId,
                               fCompanySize = s.FCompanySize,
                               fReleaseTime = s.FReleaseTime
                           }).First(x => x.fSOPID == sopId);
            if (sopById == null)
            {
                return NotFound();
            }

            return sopById;
        }
        //取該發布者的發布清單
        [HttpGet("[Action]/{pubId}")]
        public async Task<IQueryable<workFlowProdDTO>> GetPublisherSopList(int pubId)
        {
            var pubSopList = from list in _context.TSops
                             where list.FPublisherId == pubId && list.FSopType == 2
                             select new workFlowProdDTO
                             {
                                 fPubId = pubId,
                                 fSOPID = list.FSopid,
                                 fSopName = list.FSopName,
                                 fPrice = list.FPrice,
                                 fSalePoints = list.FSalePoints,
                                 fReleaseTime = list.FReleaseTime
                             };

            return pubSopList;
        }

        // PUT: api/TSops/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTSop(int id, TSop tSop)
        {
            if (id != tSop.FSopid)
            {
                return BadRequest();
            }

            _context.Entry(tSop).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TSopExists(id))
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

        // POST: api/TSops
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TSop>> PostTSop(TSop tSop)
        {
            _context.TSops.Add(tSop);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTSop", new { id = tSop.FSopid }, tSop);
        }

        // DELETE: api/TSops/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTSop(int id)
        {
            var tSop = await _context.TSops.FindAsync(id);
            if (tSop == null)
            {
                return NotFound();
            }

            _context.TSops.Remove(tSop);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TSopExists(int id)
        {
            return _context.TSops.Any(e => e.FSopid == id);
        }
    }
}
