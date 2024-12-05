using Humanizer;
using Microsoft.AspNetCore.Mvc;
using prjCoreMvcDemo.Controllers;
using prjWorkflowHubAdmin.ContextModels;
using prjWorkflowHubAdmin.ViewModels.LectureAndPublisher;

namespace prjWorkflowHubAdmin.Controllers.LectureAndPublisher
{
    public class PublisherController : SuperController
    {
        SOPMarketContext _db;
        public PublisherController(SOPMarketContext db)
        {
            _db = db;
        }
        public IActionResult PublisherIndex()
        {
            return View();
        }
        public IActionResult List(txtKeywordViewModel vm)
        {
            string keyword = vm.txtKeyword;
            IEnumerable<PublisherViewModel> datas = null;
                datas = from p in _db.TPublishers
                        join m in _db.TMembers
                        on p.FMemberId equals m.FMemberId
                        orderby m.FMemberId
                        select new PublisherViewModel
                        {
                            FPublisherId = p.FPublisherId,
                            FMemberId = p.FMemberId,
                            FPubName = p.FPubName,
                            fMemberName = m.FName,
                            FPubDescription = p.FPubDescription
                        };
            if (string.IsNullOrEmpty(keyword))
            {
                datas = datas;
            }
            else
            {
                datas = datas.Where(p =>
                p.fMemberName.ToUpperInvariant().Contains(keyword) ||
                p.FPubName.Contains(keyword) ||
                p.FPubDescription.Contains(keyword));
            }
            return View(datas);
        }
        public ActionResult Delete(int? id)
        {

            TPublisher pub = _db.TPublishers.FirstOrDefault(p => p.FPublisherId == id);
            if (pub != null)
            {
                _db.TPublishers.Remove(pub);
                _db.SaveChanges();
            }
            return RedirectToAction("List");
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TPublisher p)
        {
            _db.TPublishers.Add(p);
            _db.SaveChanges();
            return RedirectToAction("List");
        }
        public IActionResult Edit(int? id)
        {
            var queryDbPub = from p in _db.TPublishers
                             join m in _db.TMembers
                             on p.FMemberId equals m.FMemberId
                             where p.FPublisherId == id
                             select new PublisherViewModel
                             {
                                 FPublisherId = p.FPublisherId,
                                 FPubName = p.FPubName,
                                 FMemberId = p.FMemberId,
                                 fMemberName = m.FName,
                                 FPubDescription = p.FPubDescription,
                             };
            if (queryDbPub == null)
                return RedirectToAction("List");
            var vm = queryDbPub.FirstOrDefault();
            return View(vm);
        }
        [HttpPost]
        public IActionResult Edit(PublisherViewModel inPub)
        {
            TPublisher dbPub = _db.TPublishers.FirstOrDefault(l => l.FPublisherId == inPub.FPublisherId);
            if (dbPub == null)
                return RedirectToAction("List");
            dbPub.FPubName = inPub.FPubName;
            dbPub.FMemberId = inPub.FMemberId;
            dbPub.FPubDescription = inPub.FPubDescription;
            _db.SaveChanges();
            return RedirectToAction("List");
        }

    }
}
