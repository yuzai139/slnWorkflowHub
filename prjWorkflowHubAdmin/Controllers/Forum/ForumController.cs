using Microsoft.AspNetCore.Mvc;
using prjCoreMvcDemo.Controllers;
using prjWorkflowHubAdmin.ContextModels;
using prjWorkflowHubAdmin.ViewModels.Forum;

namespace prjWorkflowHubAdmin.Controllers.Forum
{
    public class ForumController : SuperController
    {
        private readonly SOPMarketContext _sopcnt;

        public ForumController(SOPMarketContext context)
        {
            _sopcnt = context;
        }


        public IActionResult ForumIndex(CForumKeywordViewModel vm)
        {

            string keyword = vm.txtKeyword;
            IEnumerable<TCategory> datas = null;
            if (string.IsNullOrEmpty(keyword))
                datas = from c in _sopcnt.TCategories
                        select c;
            else
                datas = _sopcnt.TCategories.Where(c => c.FCategoryName.Contains(keyword));
            return View(datas);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TCategory p)
        {

            _sopcnt.TCategories.Add(p);
            _sopcnt.SaveChanges();
            return RedirectToAction("ForumIndex");
        }

        public ActionResult Delete(int? id)
        {

            TCategory category = _sopcnt.TCategories.FirstOrDefault(c => c.FCategoryNumber == id);
            if (category != null)
            {
                _sopcnt.TCategories.Remove(category);
                _sopcnt.SaveChanges();
            }
            return RedirectToAction("ForumIndex");
        }

        public ActionResult Edit(int? id)
        {

            TCategory category = _sopcnt.TCategories.FirstOrDefault(c => c.FCategoryNumber == id);
            if (category == null)
                return RedirectToAction("ForumIndex");
            return View(category);
        }
        [HttpPost]
        public ActionResult Edit(TCategory inCategory)
        {

            TCategory dbcategory = _sopcnt.TCategories.FirstOrDefault(c => c.FCategoryNumber == inCategory.FCategoryNumber);
            if (dbcategory == null)
                return RedirectToAction("ForumIndex");
            dbcategory.FCategoryName = inCategory.FCategoryName;
            _sopcnt.SaveChanges();
            return RedirectToAction("ForumIndex");

        }
    }
}
