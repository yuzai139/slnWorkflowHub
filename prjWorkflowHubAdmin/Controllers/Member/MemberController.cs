using Microsoft.AspNetCore.Mvc;
using prjCoreMvcDemo.Controllers;
using prjWorkflowHubAdmin.ContextModels;
using prjWorkflowHubAdmin.ViewModels.Member;

namespace prjWorkflowHubAdmin.Controllers.Member
{
    public class MemberController : SuperController
    {
        public IActionResult MemberIndex(CKeywordViewModel vm)
        {
            SOPMarketContext db = new SOPMarketContext();
            string keyword = vm.txtKeyword;
            IEnumerable<TMember> datas = null;
            if (string.IsNullOrEmpty(keyword))
                datas = from p in db.TMembers
                        select p;
            else
                datas = db.TMembers.Where(p => p.FName.Contains(keyword)
                || p.FPhone.Contains(keyword)
                || p.FEmail.Contains(keyword)
                || p.FAddress.Contains(keyword));
            return View(datas);
        }

        public IActionResult Delete(int? id)
        {
            SOPMarketContext db = new SOPMarketContext();
            TMember member = db.TMembers.FirstOrDefault(m => m.FMemberId == id);
            if (member != null)
            {
                db.TMembers.Remove(member);
                db.SaveChanges();
            }
            return RedirectToAction("MemberIndex");
        }

        public ActionResult Edit(int? id)
        {
            SOPMarketContext db = new SOPMarketContext();
            TMember member = db.TMembers.FirstOrDefault(m => m.FMemberId == id);
            if (member == null)
                return RedirectToAction("MemberIndex");
            return View(member);
        }
        [HttpPost]
        public ActionResult Edit(TMember inMeber)
        {
            SOPMarketContext db = new SOPMarketContext();
            TMember dbMember = db.TMembers.FirstOrDefault(m => m.FMemberId == inMeber.FMemberId);
            if (dbMember == null)
                return RedirectToAction("MemberIndex");

            dbMember.FName = inMeber.FName;
            dbMember.FPhone = inMeber.FPhone;
            dbMember.FEmail = inMeber.FEmail;
            dbMember.FAddress = inMeber.FAddress;
            dbMember.FPassword = inMeber.FPassword;

            db.SaveChanges();
            return RedirectToAction("MemberIndex");
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(TMember m)
        {
            SOPMarketContext db = new SOPMarketContext();
            db.TMembers.Add(m);
            db.SaveChanges();
            return RedirectToAction("MemberIndex");
        }
    }
}
