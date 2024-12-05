using Microsoft.AspNetCore.Mvc;
using prjWorkflowHubAdmin.ContextModels;
using prjWorkflowHubAdmin.ViewModels.CartAndOrder;

namespace prjWorkflowHubAdmin.Controllers.CartAndOrder
{
    public class OrderController : Controller
    {
        private IWebHostEnvironment _enviorment;

        //public IActionResult List(CKeywordViewModel vm)
        //{
        //    SOPMarketContext db = new SOPMarketContext();
        //    string keyword = vm.txtKeyword;
        //    IEnumerable<TOrder> datas = null;
        //    if (string.IsNullOrEmpty(keyword))
        //        datas = from p in db.TOrders
        //                select p;
        //    else
        //        datas = db.TOrders.Where(p => p.FOrderId.ToString().Contains(keyword)
        //        || p.FMemberId.ToString().Contains(keyword) 
        //        || p.FOrderStatus.Contains(keyword));

        //    return View(datas);

        //}
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost] //跟系統說下面的是用Http的Post方法
        public ActionResult Create(TOrder p) //參數作為物件資料的存取區域
        {
            SOPMarketContext db = new SOPMarketContext();
            db.TOrders.Add(p); //Add()+SaveChange() 插入並將資料存於資料庫中
            db.SaveChanges();
            return RedirectToAction("OrderIndex");
        }

        public ActionResult Delete(int? id)
        {
            SOPMarketContext db = new SOPMarketContext();
            TOrder ord = db.TOrders.FirstOrDefault(p => p.FOrderId == id); //Lambda函式 鎖定哪一個ID的資料
            if (ord != null)
            {
                db.TOrders.Remove(ord);
                db.SaveChanges();
            }
            return RedirectToAction("OrderIndex");
        }
        public IActionResult Edit(int? id)
        {
            SOPMarketContext db = new SOPMarketContext();
            TOrder ord = db.TOrders.FirstOrDefault(p => p.FOrderId == id);
            if (ord == null)
                return RedirectToAction("OrderIndex");
            return View(ord);
        }
        [HttpPost]

        public IActionResult Edit(TOrder InOrder)
        {
            SOPMarketContext db = new SOPMarketContext();
            TOrder sopOrder = db.TOrders.FirstOrDefault(p => p.FOrderId == InOrder.FOrderId);
            if (sopOrder == null)
                return RedirectToAction("OrderIndex");
            
            sopOrder.FOrderId = InOrder.FOrderId;
            sopOrder.FMemberId = InOrder.FMemberId;
            sopOrder.FTotalPrice = InOrder.FTotalPrice;
            sopOrder.FOrderDate = InOrder.FOrderDate;
            sopOrder.FOrderStatus = InOrder.FOrderStatus;
            sopOrder.FPayment = InOrder.FPayment;

            db.SaveChanges();
            return RedirectToAction("OrderIndex");
        }

        public IActionResult Details(int? id)
        {
            SOPMarketContext sop = new SOPMarketContext();
            TOrderDetail ord = sop.TOrderDetails.FirstOrDefault(p => p.FOrderId == id);
            if (ord == null)
                return RedirectToAction("OrderIndex");
            return View(ord);

        }

        public IActionResult OrderIndex(CKeywordViewModel vm)
        {
            {
                SOPMarketContext db = new SOPMarketContext();
                string keyword = vm.txtKeyword;
                IEnumerable<TOrder> datas = null;
                if (string.IsNullOrEmpty(keyword))
                    datas = from p in db.TOrders
                            select p;
                else
                    datas = db.TOrders.Where(p => p.FOrderId.ToString().Contains(keyword)
                    || p.FMemberId.ToString().Contains(keyword)
                    || p.FOrderStatus.ToString().Contains(keyword));

                return View(datas);

            }
        }


    }
}
