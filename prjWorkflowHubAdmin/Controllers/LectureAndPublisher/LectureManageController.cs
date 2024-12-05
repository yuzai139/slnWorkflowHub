using Microsoft.AspNetCore.Mvc;
using prjCoreMvcDemo.Controllers;
using prjWorkflowHubAdmin.ContextModels;
using prjWorkflowHubAdmin.ViewModels.LectureAndPublisher;

namespace prjWorkflowHubAdmin.Controllers.LectureAndPublisher
{
    public class LectureManageController : SuperController
    {
        SOPMarketContext _db;
        public LectureManageController(SOPMarketContext db)
        {
            _db = db;
        }
        public IActionResult LectureIndex()
        {
            return View();
        }

        public IActionResult List(txtKeywordViewModel vm)
        {
            string keyword = vm.txtKeyword;
            IEnumerable<LectureManageViewModel> datas = null;

            datas = from lec in _db.TLectureRecords
                    join mem in _db.TMembers
                    on lec.FMemberId equals mem.FMemberId
                    join l in _db.TLectures
                    on lec.FLectureId equals l.FLectureId
                    orderby lec.FLectureId
                    select new LectureManageViewModel
                    {
                        FLecRecordId = lec.FLecRecordId,
                        FLectureId = lec.FLectureId,
                        FMemberId = lec.FMemberId,
                        FLecName = l.FLecName,
                        FName = mem.FName,
                        FPhone = mem.FPhone,
                        FEmail = mem.FEmail,
                    };
            if (string.IsNullOrEmpty(keyword))
            {
                datas = datas;
            }
            else
            {
                datas = datas.Where(l =>
                l.FName.Contains(keyword) ||
                l.FPhone.Contains(keyword) ||
                l.FEmail.Contains(keyword) ||
                l.FLecName.Contains(keyword)
                );
            }
            return View(datas);
        }
        public ActionResult Delete(int? id)
        {
            TLectureRecord lec = _db.TLectureRecords.FirstOrDefault(l => l.FLecRecordId == id);
            if (lec != null)
            {
                _db.TLectureRecords.Remove(lec);
                _db.SaveChanges();
            }
            return RedirectToAction("List");
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TLectureRecord l)
        {
            var dbRecord = _db.TLectureRecords.Where(x=>x.FLectureId == l.FLectureId).ToList();
            foreach(var items in dbRecord)
            {
                if(items.FMemberId == l.FMemberId)
                {
                    ViewBag.Message="該講座已有此報名會員";
                    //l = null;
                    return View();
                }                
            }
            _db.TLectureRecords.Add(l);
            _db.SaveChanges();
            return RedirectToAction("List");
        }
        public IActionResult Edit(int? id)
        {
            var queryDbLecRe = from lec in _db.TLectureRecords
                               join mem in _db.TMembers
                               on lec.FMemberId equals mem.FMemberId
                               join l in _db.TLectures
                               on lec.FLectureId equals l.FLectureId
                               where lec.FLecRecordId == id
                               select new LectureManageViewModel
                               {
                                   FLecRecordId = lec.FLecRecordId,
                                   FLectureId = lec.FLectureId,
                                   FMemberId = lec.FMemberId,
                                   FLecName = l.FLecName,
                                   FName = mem.FName,
                                   FPhone = mem.FPhone,
                                   FEmail = mem.FEmail,
                               };
            var vm = queryDbLecRe.FirstOrDefault();
            if (vm == null)
                return RedirectToAction("List");
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(LectureManageViewModel inLec)
        {
            TLectureRecord dbLecRecord = _db.TLectureRecords.FirstOrDefault(l => l.FLecRecordId == inLec.FLecRecordId);
            if (dbLecRecord == null)
                return RedirectToAction("List");
            dbLecRecord.FMemberId = inLec.FMemberId;
            dbLecRecord.FLectureId = inLec.FLectureId;
            dbLecRecord.FEmail = inLec.FEmail;
            dbLecRecord.FPhone = inLec.FPhone;
            _db.SaveChanges();
            return RedirectToAction("List");
        }

    }
}
