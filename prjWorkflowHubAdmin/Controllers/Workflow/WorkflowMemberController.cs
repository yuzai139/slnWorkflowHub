using Microsoft.AspNetCore.Mvc;
using prjWorkflowHubAdmin.Models.Workflow;
using prjWorkflowHubAdmin.ContextModels;
using prjWorkflowHubAdmin.ViewModels.Workflow;
using prjWorkflowHubAdmin.Models;
using prjCoreMvcDemo.Controllers;

namespace prjWorkflowHubAdmin.Controllers.Workflow
{
    public class WorkflowMemberController : Controller
    {
        string? _sopImagePath {  get; set; }
        CMemberSopBoxViewModel _MmbSopBox {  get; set; }


        public IActionResult WorkflowMemberCanvas()
        {
            return View();
        }
        public IActionResult SOPMemberTest()
        {
            return View();
        }

        // [會員列表]
        public IActionResult WorkflowMemberIndex(CSopKeywordViewModels vm)
        {
            // 創建資料庫上下文物件
            SOPMarketContext db = new SOPMarketContext();

            // 從前端傳入的模型中取得使用者輸入的關鍵字
            string keyword = vm.txtKeyword;

            // 初始化客戶資料列表為 null，準備存放查詢結果
            //IEnumerable<TMember> datas = null;
            List<CMemberViewModel> dataList = null;

            // 如果關鍵字為空或為 null，則查詢所有客戶資料
            if (string.IsNullOrEmpty(keyword))
            {
                // 查詢所有會員資料，並映射到 MemberViewModel
                dataList = db.TMembers.Select(p => new CMemberViewModel
                {
                    MemberId = p.FMemberId,
                    Name = p.FName,
                    Email = p.FEmail
                }).ToList();
            }
            // 如果有輸入關鍵字，則根據關鍵字篩選出客戶資料
            else
            {
                // 根據名稱或 Email 查詢
                dataList = db.TMembers
                    .Where(p => p.FName.Contains(keyword) || p.FEmail.Contains(keyword))
                    .Select(p => new CMemberViewModel
                    {
                        MemberId = p.FMemberId,
                        Name = p.FName,
                        Email = p.FEmail
                    }).ToList();
            }

            // 返回篩選後的客戶資料列表到 View 中，顯示結果
            return View(dataList);
        }

        //[ Member SOP List ]
        public IActionResult WorkflowMemberList(CSopKeywordViewModels kword, int? MemberId, string MemberName)
        {
            // 創建資料庫上下文物件
            SOPMarketContext db = new SOPMarketContext();

            // 從前端傳入的模型中取得使用者輸入的關鍵字
            string keyword = kword.txtKeyword;
     

            // 初始化客戶資料列表為 null，準備存放查詢結果
            List<CMemberSOPListViewModel> dataList = null;

            // 如果關鍵字為空或為 null，則查詢所有資料
            if (string.IsNullOrEmpty(keyword))
            {
                // 查詢所有會員資料，並映射到 MemberViewModel
                dataList = db.TSops
                    .Where(p => p.FMemberId == MemberId && p.FSopType== CSopDictionary.MemberSopType)
                    .Select(p => new CMemberSOPListViewModel
                {
                    FSopid = p.FSopid,
                    FMemberId = p.FMemberId,
                    MemberName = MemberName, //傳入的MemberName
                    FSopType = p.FSopType,
                    FSopName = p.FSopName,
                    FJobItemId = p.FJobItemId,
                    FIndustryId = p.FIndustryId,
                    FFileStatus = p.FFileStatus

                }).ToList();
            }
            // 如果有輸入關鍵字，則根據關鍵字篩選出客戶資料
            else
            {
                // LINQ查詢
                dataList = db.TSops
                    .Where(p => p.FMemberId == MemberId && p.FSopType == CSopDictionary.MemberSopType && p.FSopName.Contains(keyword))
                    .Select(p => new CMemberSOPListViewModel
                    {
                        FSopid = p.FSopid,
                        FMemberId = p.FMemberId,
                        MemberName = MemberName, //傳入的MemberName
                        FSopType = p.FSopType,
                        FSopName = p.FSopName,
                        FJobItemId = p.FJobItemId,
                        FIndustryId = p.FIndustryId,
                        FFileStatus = p.FFileStatus
                    }).ToList();
            }

            // 將資料放入
            var memberSopListBox = new CMemberSOPListBoxViewModel
            {
                MemberName = MemberName,  // 將傳入值MemberName放入
                MemberId = MemberId, // 將傳入值MemberId放入
                MemberSOPList = dataList        // 將資料列表放入
            };

            return View(memberSopListBox);
        }

        //[新增會員工作流程]
        public CMemberSopViewModel SopMemberCreate(int memberId)
        {

            // 新增工作流程的邏輯
            TSop newSop = new TSop
            {
                // 初始化需要的欄位
                FMemberId = memberId,
                FSopType = CSopDictionary.MemberSopType,
                FSopName = "未命名工作流程",
                FFileStatus = "啟用中",
                FEditTime = "-",
                FJobItemId = 0,
                FIndustryId = 0
            };

            using (SOPMarketContext db = new SOPMarketContext())
            {
                db.TSops.Add(newSop);
                db.SaveChanges();  // 儲存到資料庫
            }

            CMemberSopViewModel sopViewModel = new CMemberSopViewModel//建立SopViewModel
            {
                FSopid = newSop.FSopid,
                FMemberId = newSop.FMemberId,
                FSopType = CSopDictionary.MemberSopType,
                FSopName = newSop.FSopName,
                FFileStatus = newSop.FFileStatus,
                FEditTime = newSop.FEditTime,
                FJobItemId = newSop.FJobItemId,
                FIndustryId = newSop.FIndustryId
            };


            return sopViewModel;  // 返回 SOP ID
        }

        //[編輯會員工作流程 - Get]
        public IActionResult SopMemberEdit(int sopId, int memberId, string memberName)
        {
            TSop tsop = new TSop();
            TSopIndustry tsopIngustry = new TSopIndustry();
            TSopJobItem tsopJobItem = new TSopJobItem();
            using (SOPMarketContext db = new SOPMarketContext())
            {
                // 查詢資料，傳遞到 View 中進行編輯
                tsop = db.TSops.FirstOrDefault(s => s.FSopid == sopId);
                if (tsop == null)
                {
                    return RedirectToAction("WorkflowMemberList", "WorkflowMember", new {MemberId = memberId, MemberName = memberName }); 
                }
                if(tsop.FIndustryId!=null)
                {
                    tsopIngustry = db.TSopIndustries.FirstOrDefault(s => s.FIndustryId == tsop.FIndustryId);
                }
                if (tsop.FJobItemId != null)
                {
                    tsopJobItem = db.TSopJobItems.FirstOrDefault(s => s.FJobItemId == tsop.FJobItemId);
                }

            }
            CMemberSopViewModel tsopViewModel = new CMemberSopViewModel
            {
                FSopid = tsop.FSopid,
                FMemberId = tsop.FMemberId,
                FSopType = tsop.FSopType,
                FSopName = tsop.FSopName,
                FSopDescription = tsop.FSopDescription,
                FSopFlowImagePath = tsop.FSopFlowImagePath,
                FJobItemId = tsop.FJobItemId,
                JobItem = tsopJobItem.FJobItem,
                FIndustryId = tsop.FIndustryId,
                Industry = tsopIngustry.FIndustry,
                FBusiness = tsop.FBusiness,
                FCustomer = tsop.FCustomer,
                FCompanySize = tsop.FCompanySize,
                FDepartment = tsop.FDepartment,
                FShareUrl = tsop.FShareUrl,
                FEditTime = tsop.FEditTime,
                FSharePermission = tsop.FSharePermission,
                FFileStatus = tsop.FFileStatus
            };

            CMemberSopBoxViewModel MemberSopBox = new CMemberSopBoxViewModel
            {
                MemberName = memberName,
                MemberSopViewModel = tsopViewModel,
            };

			// 返回編輯頁面，傳遞資料
			return View(MemberSopBox);
        }

        //[新增並編輯會員工作流程]
        public IActionResult SopMemberCreateAndEdit(int memberId, string memberName)
        {
            // 1. 調用 CreateController 來新增工作流程
            CMemberSopViewModel SopDto = SopMemberCreate(memberId);

            int? newSopId = SopDto.FSopid;
            int? newSopMemberid = SopDto.FMemberId;

            // 2. 重定向到 EditController 來編輯工作流程
            return RedirectToAction("SopMemberEdit", "WorkflowMember", new { sopId = newSopId, MemberId = memberId, MemberName = memberName });
        }

        [HttpPost]//儲存SOP資料
        public async Task<IActionResult> SaveSopMemberData(CMemberSopBoxViewModel viewModelBox)
        {
            if (!ModelState.IsValid)
            {
                // 檢查是否有錯誤訊息並返回原頁面
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }

                TempData["ErrorMessage"] = "表單資料不合法，請檢查輸入內容";
                return View(viewModelBox);
            }

            // 提取 CMemberSopViewModel
            CMemberSopViewModel sopViewModel = viewModelBox.MemberSopViewModel;

            using (SOPMarketContext db = new SOPMarketContext())
            {
                // 查找對應的 TSop 資料
                TSop existingSop = db.TSops.FirstOrDefault(s => s.FSopid == sopViewModel.FSopid);
                if (existingSop == null)
                {
                    TempData["ErrorMessage"] = "找不到對應的 SOP";
                    return View(viewModelBox);
                }

                // 更新資料
                existingSop.FSopType = sopViewModel.FSopType;
                existingSop.FSopName = sopViewModel.FSopName;
                existingSop.FSopDescription = sopViewModel.FSopDescription;
                existingSop.FSopFlowImagePath = sopViewModel.FSopFlowImagePath;
                existingSop.FJobItemId = sopViewModel.FJobItemId;
                existingSop.FIndustryId = sopViewModel.FIndustryId;
                existingSop.FBusiness = sopViewModel.FBusiness;
                existingSop.FCustomer = sopViewModel.FCustomer;
                existingSop.FCompanySize = sopViewModel.FCompanySize;
                existingSop.FDepartment = sopViewModel.FDepartment;
                existingSop.FShareUrl = sopViewModel.FShareUrl;
                existingSop.FSharePermission = sopViewModel.FSharePermission;
                existingSop.FFileStatus = sopViewModel.FFileStatus;

                // 儲存更改
                await db.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "資料已成功儲存";
            return View(viewModelBox);
        }



        // 圖檔儲存的 Action (接收 PNG 圖片並調用儲存邏輯)
        [HttpPost]
        public async Task<IActionResult> SaveDiagram(IFormFile diagramPng, int sopId)
        {
            if (diagramPng != null)
            {
                // 儲存圖檔邏輯（儲存到 wwwroot/Workflow/SopImages 資料夾）
                string imagePath = await SaveDiagramAsync(diagramPng);

                string oldImagePath = "";

                // 更新資料庫中的 TSop 圖檔路徑
                using (SOPMarketContext db = new SOPMarketContext())
                {
                    TSop existingSop = db.TSops.FirstOrDefault(s => s.FSopid == sopId);
                    if (existingSop != null)
                    {
                        oldImagePath = existingSop.FSopFlowImagePath;

                        existingSop.FSopFlowImagePath = imagePath;
                        await db.SaveChangesAsync();
                    }
                }

                // 呼叫刪除舊圖檔的方法
                DeleteOldImage(oldImagePath);

                // 回傳儲存成功與圖片路徑
                return Json(new { success = true, path = imagePath });
            }

            return Json(new { success = false, message = "圖檔未上傳" });
        }


        // 刪除舊圖檔的方法
        private void DeleteOldImage(string ImagePath)
        {
            if (string.IsNullOrEmpty(ImagePath))
            {
                return;
            }

            // 將相對路徑轉換為伺服器上的實際絕對路徑
            string fullImagePath = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot/Workflow/SopImages", Path.GetFileName(ImagePath));

            // 檢查檔案是否存在，如果存在則刪除
            if (System.IO.File.Exists(fullImagePath))
            {
                try
                {
                    System.IO.File.Delete(fullImagePath); // 刪除舊檔案
                    Console.WriteLine($"圖檔 {fullImagePath} 已被刪除。");
                }
                catch (Exception ex)
                {
                    // 處理刪除過程中的錯誤，例如：日誌記錄或返回錯誤信息
                    Console.WriteLine($"刪除檔案失敗: {ex.Message}");
                }
            }
        }


        // 圖檔儲存邏輯 (將圖檔存入伺服器)
        public static async Task<string> SaveDiagramAsync(IFormFile diagramPng)
        {
            // 檢查上傳的圖檔是否為 null，如果是，則拋出 ArgumentNullException 異常
            if (diagramPng == null)
                throw new ArgumentNullException(nameof(diagramPng));

            // 定義圖片存儲的目錄，將圖片存入 wwwroot/Workflow/SopImages 資料夾
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Workflow/SopImages");

            // 檢查資料夾是否存在，如果不存在，則創建該資料夾
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir); // 創建資料夾
            }

            // 生成唯一的圖檔名稱，使用 GUID 確保每個檔案名都是唯一的，並且副檔名為 .png
            var fileName = $"{Guid.NewGuid()}.png";

            // 圖檔的完整儲存路徑 (目錄 + 檔案名稱)
            var filePath = Path.Combine(uploadsDir, fileName);

            // 使用 FileStream 將圖檔寫入到伺服器上的指定位置
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                // 將接收到的圖檔內容異步寫入文件流中
                await diagramPng.CopyToAsync(fileStream);
            }

            // 返回圖檔的相對路徑，這可以存入資料庫中，用於日後檢索圖檔
            //return $"/Workflow/SopImages/{fileName}";

            return fileName;
        }


        ////整合方法
        //[HttpPost]
        //public async Task<IActionResult> SaveSopMemberAndDiagram(CMemberSopBoxViewModel viewModelBox)
        //{
            
        //    // 1. 先儲存會員資料
        //    IActionResult memberSaveResult = await SaveSopMemberData(viewModelBox);

        //    // 檢查會員資料是否儲存成功，如果有錯誤則直接返回
        //    if (!(memberSaveResult is ViewResult))
        //    {
        //        return memberSaveResult; // 如果會員資料儲存失敗，返回錯誤訊息
        //    }

        //    return RedirectToAction("SopMemberEdit", "WorkflowMember", 
        //        new { sopId = viewModelBox.MemberSopViewModel.FSopid , memberid = viewModelBox.MemberSopViewModel.FMemberId,
        //            memberName = viewModelBox.MemberName});
        //}

        //重新整理
        //public IActionResult RefreshCanva(int sopId)
        //{
        //    return View();
        //}


        //刪除工作流程
        public IActionResult MemberSopDelete(int sopId)
        {
            using (SOPMarketContext db = new SOPMarketContext())
            {
                TSop tsop = db.TSops.FirstOrDefault(t => t.FSopid == sopId);

                if(tsop != null)
                {
                    db.TSops.Remove(tsop);
                    db.SaveChanges();
                    string deleteImagePath = tsop.FSopFlowImagePath;
                    DeleteOldImage(deleteImagePath);
                }

                int? memberId = tsop.FMemberId;

                TMember tmember = db.TMembers.FirstOrDefault(t => t.FMemberId == memberId);

                string memberName = tmember.FName;


                return RedirectToAction("WorkflowMemberList", new { MemberId = memberId, MemberName = memberName });
            };
        }

    }
}
