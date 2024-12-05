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
    public class TPublishersController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TPublishersController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TPublishers
        //全拿
        [HttpGet]
        public async Task<IEnumerable<TPublisher>> GetTPublishers()
        {
            return await _context.TPublishers.ToListAsync();
        }

        // GET: api/TPublishers/5
        //slideBar選發布者
        [HttpGet("{id}")]
        public IQueryable<TPublisher> GetTPublisherList(int id)
        {
            var tPublisherList = from m in _context.TPublishers
                                 where m.FMemberId == id
                                 select m;


            if (tPublisherList == null)
            {
                return (IQueryable<TPublisher>)NotFound();
            }

            return tPublisherList;
        }
        //這才是取得發布者資料
        [HttpGet("[Action]/{pubId}")]
        //public IQueryable<TPublisher> GetTPublisher(int pubId)
        public ActionResult<forGetPubData> GetTPublisher(int pubId)
        {
            var tPublisher = (from p in _context.TPublishers
                              where p.FPublisherId == pubId
                              select new forGetPubData
                              {
                                  FPubName = p.FPubName,
                                  FPublisherId = p.FPublisherId,
                                  FMemberId = p.FMemberId,
                                  FPubDescription = p.FPubDescription,
                                  FPubImagePath = p.FPubImagePath,
                                  FPubLink = p.FPubLink,
                              }).FirstOrDefault();
            if (tPublisher == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(tPublisher.FPubImagePath))
            {
                //實際路徑，Directory.GetCurrentDirectory()是此專案路徑
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", tPublisher.FPubImagePath);

                if (System.IO.File.Exists(filePath))
                {
                    //url路徑$"{Request.Scheme}://{Request.Host}/images/ {{path}}
                    //var filePath = Path.Combine($"{Request.Scheme}://{Request.Host}/images/", tPublisher.FPubImagePath);
                    //var imageBytes = System.IO.File.ReadAllBytes(filePath);
                    //var base64String = Convert.ToBase64String(imageBytes);
                    tPublisher.FPubImageUrl = $"{Request.Scheme}://{Request.Host}/images/{tPublisher.FPubImagePath}";
                }
            }
                else
                {
                    tPublisher.FPubImageUrl = $"{Request.Scheme}://{Request.Host}/images/default.jpg";
                }

            return tPublisher;
        }
        //新增發布者資料
        [HttpPost]
        public async Task<ActionResult<TPublisher>> PostTPublisher([FromForm] forCreatePublisher inPub)
        {
            TPublisher saveData = new TPublisher
            {
                FPubName = inPub.fPubName,
                FMemberId = inPub.fMemberId,
                FPubDescription = inPub.fPubDescription,
                FPubLink = inPub.fPubLink,
                FPubStatus = inPub.fPubStatus,
                //FPubCreateTime = inPub.fPubCreateTime,
            };


            if (inPub.fPubImage != null)
            {
                string photoName = Guid.NewGuid().ToString() + ".jpg";
                //inPub.fPubImage.CopyTo(new FileStream("D:\\iSPAN\\專題\\BackStage\\apiWorkflowHub\\image\\" + photoName, FileMode.Create));
                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath); // 創建目錄
                }

                string filePath = Path.Combine(directoryPath, photoName); // 使用 wwwroot/images 的路徑

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await inPub.fPubImage.CopyToAsync(stream); // 使用異步方法
                }

                saveData.FPubImagePath = photoName;
            }

            _context.TPublishers.Add(saveData);
            _context.SaveChanges();

            return Ok();
            //return CreatedAtAction("GetTPublisher", new { id = saveData.FMemberId }, saveData);
        }

        // PUT: api/TPublishers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")] //
        public async Task<IActionResult> PutTPublisher(int id, [FromForm] forEditPublisherDTO inPub)
        {
            //if (id != inPub.fPublisherId)
            //{
            //    return BadRequest("ID mismatch.");
            //}
            var editData = _context.TPublishers.FirstOrDefault(x=>x.FPublisherId == id);

            if (editData == null)
                return NotFound();

            editData.FPublisherId = id;
            editData.FMemberId = inPub.fMemberId;
            editData.FPubName = inPub.fPubName;
            editData.FPubDescription = inPub.fPubDescription;
            editData.FPubLink = inPub.fPubLink;

            //修改new是在衝三小，低能兒是不是
            //TPublisher editData = new TPublisher
            //{
            //    FPublisherId = id,
            //    FMemberId = inPub.fMemberId,
            //    FPubName = inPub.fPubName,
            //    FPubDescription = inPub.fPubDescription,
            //    FPubLink = inPub.fPubLink
            //};

            try
            {
                if (inPub.fPubImage != null)
                {
                    string photoName = Guid.NewGuid().ToString() + ".jpg";
                    //inPub.fPubImage.CopyTo(new FileStream("D:\\iSPAN\\專題\\BackStage\\apiWorkflowHub\\image\\" + photoName, FileMode.Create));
                    string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath); // 創建目錄
                    }

                    string filePath = Path.Combine(directoryPath, photoName); // 使用 wwwroot/images 的路徑

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await inPub.fPubImage.CopyToAsync(stream); // 使用異步方法
                    }

                    editData.FPubImagePath = photoName;
                }
                else
                {
                    editData.FPubImagePath = editData.FPubImagePath;
                }

                //_context.Entry(editData).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TPublisherExists(inPub.fPublisherId))
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

        //TPublisher pubSelected = null;
        //if (pubId > 0)
        //{
        //    pubSelected = (from p in _context.TPublishers
        //                   select p).First(x => x.FPublisherId == pubId);
        //}
        //if (pubSelected == null)
        //{
        //    return NotFound();
        //}

        //return pubSelected;
        // POST: api/TPublishers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        //public class Test
        //{
        //    public int Id { get; set; }
        //    public string Name { get; set; }
        //}

        //[HttpPost]
        //public ActionResult<TPublisher> PostTPublisher([FromBody] Test test)
        //{
        //    TPublisher pubSelected = null;
        //    if (test.Id > 0)
        //    {
        //        pubSelected = (from p in _context.TPublishers

        //                       select p).First(x => x.FPublisherId == test.Id);
        //    }
        //    if (pubSelected == null)
        //    {
        //        return NotFound();
        //    }

        //    return pubSelected;
        //}


        //[HttpPost]
        //public ActionResult<TPublisher> PostTPublisher([FromBody] int pubId)
        //{

        //    //_context.TPublishers.Add(tPublisher);
        //    //await _context.SaveChangesAsync();

        //    //return CreatedAtAction("GetTPublisher", new { id = tPublisher.FPublisherId }, tPublisher);

        //    TPublisher pubSelected = null;
        //    if (pubId > 0)
        //    {
        //        pubSelected = (from p in _context.TPublishers
        //                       select p).First(x => x.FPublisherId == pubId);
        //    }
        //    if (pubSelected == null)
        //    {
        //        return NotFound();
        //    }

        //    return pubSelected;
        //}



        // DELETE: api/TPublishers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTPublisher(int id)
        {
            var tPublisher = await _context.TPublishers.FindAsync(id);
            if (tPublisher == null)
            {
                return NotFound();
            }

            _context.TPublishers.Remove(tPublisher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TPublisherExists(int id)
        {
            return _context.TPublishers.Any(e => e.FPublisherId == id);
        }
    }
}
