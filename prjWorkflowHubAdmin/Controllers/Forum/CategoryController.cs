using Microsoft.AspNetCore.Mvc;
using prjCoreMvcDemo.Controllers;
using prjWorkflowHubAdmin.ContextModels;
using prjWorkflowHubAdmin.ViewModels.Forum;
using System.Formats.Tar;

namespace prjWorkflowHubAdmin.Controllers.Forum
{
    public class CategoryController : SuperController
    {
        private readonly SOPMarketContext _sopcnt;

        public CategoryController(SOPMarketContext context)
        {
            _sopcnt = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int? id, CForumKeywordViewModel vm)
        {
            var category = _sopcnt.TCategories.FirstOrDefault(c => c.FCategoryNumber == id);

            if (category == null)
            {
                return RedirectToAction("ForumIndex");
            }

            var articles = _sopcnt.TArticles.Where(a => a.FCategoryNumber == category.FCategoryNumber);

            string keyword = vm?.txtKeyword;
            if (!string.IsNullOrEmpty(keyword))
            {
                articles = articles.Where(a => a.FArticleName.Contains(keyword) || a.FArticleContent.Contains(keyword));
            }

            ViewBag.Articles = articles.ToList();
            ViewBag.Keyword = keyword;

            return View(category);
        }
        public IActionResult Edit(int? id)
        {

            TArticle article = _sopcnt.TArticles.FirstOrDefault(a => a.FArticleId == id);
            if (article == null)
                return RedirectToAction("Details");

            return View(article);
        }

        [HttpPost]
        public IActionResult Edit(TArticle inArticle)
        {
            if (string.IsNullOrWhiteSpace(inArticle.FArticleName))
            {
                TempData["ErrorMessage"] = "文章名稱不能為空。";
            }
            if (string.IsNullOrWhiteSpace(inArticle.FArticleContent))
            {
                TempData["ErrorMessage"] = "文章內容不能為空。";
            }
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.FCategoryNumber = inArticle.FCategoryNumber;
                return View(inArticle);
            }



            TArticle dbArticle = _sopcnt.TArticles.FirstOrDefault(a => a.FArticleId == inArticle.FArticleId);
            if (dbArticle == null)
                return RedirectToAction("Details");


            dbArticle.FArticleContent = inArticle.FArticleContent;
            dbArticle.FArticleName = inArticle.FArticleName;


            _sopcnt.SaveChanges();
            return RedirectToAction("Details", "Category", new { id = dbArticle.FCategoryNumber });
        }
        public IActionResult Delete(int? id)
        {

            TArticle article = _sopcnt.TArticles.FirstOrDefault(a => a.FArticleId == id);
            if (article == null)
            {
                return RedirectToAction("Details", "Category", new { id = article.FCategoryNumber });
            }
            _sopcnt.TArticles.Remove(article);
            _sopcnt.SaveChanges();
            return RedirectToAction("Details", "Category", new { id = article.FCategoryNumber });
        }
        public IActionResult Create(int? fCategoryNumber)
        {
            ViewBag.FCategoryNumber = fCategoryNumber;
            return View();
        }

        [HttpPost]
        public IActionResult Create(TArticle newArticle)
        {
            if (string.IsNullOrWhiteSpace(newArticle.FArticleName))
            {
                TempData["ErrorMessage"] = "文章名稱不能為空。";
            }
            if (string.IsNullOrWhiteSpace(newArticle.FArticleContent))
            {
                TempData["ErrorMessage"] = "文章內容不能為空。";
            }
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.FCategoryNumber = newArticle.FCategoryNumber;
                return View(newArticle);
            }
            _sopcnt.TArticles.Add(newArticle);
            _sopcnt.SaveChanges();
            return RedirectToAction("Details", new { id = newArticle.FCategoryNumber });
        }
    }
}
