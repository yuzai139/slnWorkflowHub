using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using prjCoreMvcDemo.Controllers;
using prjWorkflowHubAdmin.ContextModels;
using prjWorkflowHubAdmin.ViewModels.LectureAndPublisher;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace prjWorkflowHubAdmin.Controllers.LectureAndPublisher
{
    public class LectureController : SuperController
    {
        SOPMarketContext _db;
        public LectureController(SOPMarketContext db)
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
            //IEnumerable<TLecture> datas = null;
            IEnumerable<LectureViewModel> datas = null;
            datas = from l in _db.TLectures
                    join p in _db.TPublishers
                    on l.FPublisherId equals p.FPublisherId
                    select new LectureViewModel
                    {
                        FLectureId = l.FLectureId,
                        FLecName = l.FLecName,
                        FPublisherId = l.FPublisherId,
                        FPubName = p.FPubName,
                        FLecPrice = l.FLecPrice,
                        FLecPoints = l.FLecPoints,
                        FLecDate = l.FLecDate,
                        FLecDescription = l.FLecDescription,
                        FOnline = l.FOnline,
                        FLecLimit = l.FLecLimit,
                        FLink = l.FLink,
                        FLecLocation = l.FLecLocation,
                        //FLecImage = l.FLecImage,
                    };
            if (string.IsNullOrEmpty(keyword))
            {
                datas = datas;
            }
            else
            {
                datas = datas.Where(l =>
                    l.FLecName.Contains(keyword) ||
                    l.FLecDescription.Contains(keyword) ||
                    l.FPubName.Contains(keyword) ||
                    l.FLecDate.Contains(keyword)
                );
            }
            return View(datas);
        }
        public ActionResult Delete(int? id)
        {
            TLecture lec = _db.TLectures.FirstOrDefault(l => l.FLectureId == id);
            if (lec != null)
            {
                _db.TLectures.Remove(lec);
                _db.SaveChanges();
            }
            return RedirectToAction("List");
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TLecture l)
        {
            _db.TLectures.Add(l);
            _db.SaveChanges();
            return RedirectToAction("List");
        }
        public IActionResult Edit(int? id)
        {
            var queryDbLec = from l in _db.TLectures
                             join p in _db.TPublishers
                             on l.FPublisherId equals p.FPublisherId
                             where l.FLectureId == id
                             select new LectureViewModel
                             {
                                 FLectureId = l.FLectureId,
                                 FLecName = l.FLecName,
                                 FPubName = p.FPubName,
                                 FPublisherId = p.FPublisherId,
                                 FLecPrice = l.FLecPrice,
                                 FLecPoints = l.FLecPoints,
                                 FLecDate = l.FLecDate,
                                 FLecImage = l.FLecImage,
                                 FLecDescription = l.FLecDescription,
                                 FOnline = l.FOnline,
                                 FLecLimit = l.FLecLimit,
                                 FLink = l.FLink,
                                 FLecLocation = l.FLecLocation,
                             };
            var vm = queryDbLec.FirstOrDefault();
            if (vm == null)
            {
                return RedirectToAction("List");
            }
            return View(vm);
        }
        [HttpPost]
        public IActionResult Edit(LectureViewModel inLec)
        {
            TLecture dbLec = _db.TLectures.FirstOrDefault(l => l.FLectureId == inLec.FLectureId);
            if (dbLec == null)
                return RedirectToAction("List");
            dbLec.FLecName = inLec.FLecName;
            //dbPub.FPubName = inLec.FPubName;
            dbLec.FPublisherId = inLec.FPublisherId;
            dbLec.FLecPrice = inLec.FLecPrice;
            dbLec.FLecPoints = inLec.FLecPoints;
            dbLec.FLecDate = inLec.FLecDate;
            dbLec.FLecImage = inLec.FLecImage;
            dbLec.FLecDescription = inLec.FLecDescription;
            dbLec.FOnline = inLec.FOnline;
            dbLec.FLecLimit = inLec.FLecLimit;
            dbLec.FLecLocation = inLec.FLecLocation;
            dbLec.FLink = inLec.FLink;
            _db.SaveChanges();
            return RedirectToAction("List");
        }

    }
}
