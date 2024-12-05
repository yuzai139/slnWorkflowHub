using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjWorkflowHubAdmin.ContextModels;
using prjWorkflowHubAdmin.ViewModels.LectureAndPublisher.Lecture;

namespace prjWorkflowHubAdmin.Controllers.LectureAndPublisher
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrjSopsController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public PrjSopsController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/PrjSops
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<prjSopImageDTO>>> GetTSops()
        //首頁的
        public async Task<List<prjSopImageDTO>> GetTSops()
        {
            //用DTO會動到sql套件，兩個專案版本不一，升級成一樣之後swagger會跳500
            //string imageBasePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Workflow", "PublishImages");

            var homeComponentList = (from s in _context.TSops
                                     where s.FSopType == 2
                                     select new prjSopImageDTO
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
                                         fPubSopImageUrl = $"{Request.Scheme}://{Request.Host}/Workflow/PublishImages/{s.FPubSopImagePath}"
                                     }).ToList();

            //foreach (var item in homeComponentList)
            //{
            //    // 假設你的圖片 URL 是這樣的
            //    string filePath = item.fPubSopImageUrl;
            //    item.fPassImage = filePath; // 將 URL 存儲到 fPassImage 中
            //}

            return homeComponentList;

            //if (System.IO.File.Exists(filePath))
            //{
            //    var imageBytes = System.IO.File.ReadAllBytes(filePath);
            //    var base64String = Convert.ToBase64String(imageBytes);
            //    TSop.FPubImagePath = base64String;
            //}

            //return _context.TSops.ToList();

            //return await _context.TSops.ToListAsync();
        }

        //商品頁的
        // GET: api/PrjSops/5
        [HttpGet("{id}")]
        public async Task<ActionResult<prjSopProdDTO>> GetTSop(int id)
        {
            var homeComponentList = (from s in _context.TSops
                                     where s.FSopType == 2 && s.FSopid == id
                                     select new prjSopProdDTO
                                     {
                                         fSOPID = s.FSopid,
                                         fPubId = s.FPublisherId,
                                         fSopName = s.FSopName,
                                         fPubSopImagePath = s.FPubSopImagePath,
                                         fPrice = s.FPrice,
                                         fSalePoints = s.FSalePoints,
                                         fJobItemId = s.FJobItemId,
                                         fIndustryId = s.FIndustryId,
                                         fCompanySize = s.FCompanySize,
                                         fReleaseTime = s.FReleaseTime,
                                         fPubContent = s.FPubContent,
                                         fPubSopImageUrl = $"{Request.Scheme}://{Request.Host}/Workflow/PublishImages/{s.FPubSopImagePath}"
                                     }).FirstOrDefault();
            return homeComponentList;
        }

        //發布者頁的，該發布者工作流
        // GET: api/PrjSops/5
        [HttpGet("[Action]/{id}")]
        public async Task<IQueryable<prjSopProdDTO>> GetPubSop(int id)
        {
            var homeComponentList = from s in _context.TSops
                                     where s.FSopType == 2 && s.FPublisherId == id
                                     select new prjSopProdDTO
                                     {
                                         fSOPID = s.FSopid,
                                         fPubId = s.FPublisherId,
                                         fSopName = s.FSopName,
                                         fPubSopImagePath = s.FPubSopImagePath,
                                         fPrice = s.FPrice,
                                         fSalePoints = s.FSalePoints,
                                         fPubSopImageUrl = $"{Request.Scheme}://{Request.Host}/Workflow/PublishImages/{s.FPubSopImagePath}"
                                     };
            return homeComponentList;
        }

        //搜尋用
        // GET: api/PrjSops/5
        [HttpGet("[Action]/{key}")]
        public async Task<List<prjSopProdDTO>> serchSop(string key)
        {
            var homeComponentList = (from s in _context.TSops
                                    where s.FSopType == 2 
                                    select new prjSopProdDTO
                                    {
                                        fSOPID = s.FSopid,
                                        fPubId = s.FPublisherId,
                                        fSopName = s.FSopName,
                                        fPubSopImagePath = s.FPubSopImagePath,
                                        fPrice = s.FPrice,
                                        fSalePoints = s.FSalePoints,
                                        fPubSopImageUrl = $"{Request.Scheme}://{Request.Host}/Workflow/PublishImages/{s.FPubSopImagePath}"
                                    }).Where(x=>x.fSopName.Contains(key)).ToList();
            return homeComponentList;
        }

        [HttpGet("[Action]/{jobItemId}")]
        public List<prjSopImageDTO> filterSop(int jobItemId)
        {
            var homeComponentList = (from s in _context.TSops
                                     where s.FSopType == 2 && s.FJobItemId == jobItemId
                                     select new prjSopImageDTO
                                     {
                                         fSOPID = s.FSopid,
                                         fSopName = s.FSopName,
                                         fPubSopImagePath = s.FPubSopImagePath,
                                         fPrice = s.FPrice,
                                         fSalePoints = s.FSalePoints,
                                         fPubSopImageUrl = $"{Request.Scheme}://{Request.Host}/Workflow/PublishImages/{s.FPubSopImagePath}"
                                     }).ToList();
            return homeComponentList;
        }

        // PUT: api/PrjSops/5
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

        // POST: api/PrjSops
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TSop>> PostTSop(TSop tSop)
        {
            _context.TSops.Add(tSop);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTSop", new { id = tSop.FSopid }, tSop);
        }

        // DELETE: api/PrjSops/5
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
