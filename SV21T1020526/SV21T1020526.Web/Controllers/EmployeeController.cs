using Microsoft.AspNetCore.Mvc;
using SV21T1020526.BusinessLayers;
using SV21T1020526.DomainModels;
using SV21T1020526.Web.Appcodes;
using static SV21T1020526.Web.Models.PaginationSearchResult;
using SV21T1020526.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace SV21T1020526.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR}")]
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 9;
        private const string EMPLOYEE_SERCH_CONDITION = "EmployeeSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SERCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);
        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfEmployee(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            EmployeeSearchResult model = new EmployeeSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(EMPLOYEE_SERCH_CONDITION, condition);
            return View(model);
        }
       

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên mới";
            var data = new Employee()
            {
                EmployeeID = 0,
                IsWorking = true,
                Photo = "nophoto.jpg"
            };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            var data = CommonDataService.GetEmployee(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Employee data, string _BirthDate, IFormFile? _Photo)
        {
            ViewBag.Title = data.EmployeeID == 0 ? "Thêm nhân viên" : "Cập nhật thông tin nhân viên";
            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Tên nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(_BirthDate))
                ModelState.AddModelError(nameof(data.BirthDate), "Vui lòng nhập ngày sinh của nhân viên");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện của nhân viên");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email của nhân viên");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của nhân viên");


            //dựa cào thuộc tính Isvalid của modelstate để viết rõ có lỗi tồn tại hay không
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }

            //Xử lý ngày sinh 
            DateTime? d = _BirthDate.ToDateTime();
            if (d.HasValue)  //(d != null)
            {
                int year = d.Value.Year;
                
                if(year<1000)
                {
                    ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh không hợp lệ. Vui lòng nhập lại.");
                    return View("Edit", data);
                }
                if (DateTime.Now.Year - year >= 65)
                {
                    ModelState.AddModelError(nameof(data.BirthDate), "Nhân viên quá độ tuổi lao động.");
                    return View("Edit", data);
                }
                // Kiểm tra ngày sinh không được là ngày trong tương lai
                if (d.Value > DateTime.Now)
                {
                    ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh không thể là ngày trong tương lai.");
                    return View("Edit", data);
                }

                
                data.BirthDate = d.Value;
            }
            // Xử lý ảnh 
            if (_Photo != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images/employees", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    _Photo.CopyTo(stream);
                }
                data.Photo= fileName;
            }
            /*if (!string.IsNullOrWhiteSpace(_BirthDate))
            {
                DateTime parsedDate;
                if (!DateTime.TryParse(_BirthDate, out parsedDate))
                {
                    ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh không hợp lệ. Vui lòng nhập lại.");
                    return View("Edit", data);
                }

                // Kiểm tra ngày sinh không được là ngày trong tương lai
                if (parsedDate > DateTime.Now)
                {
                    ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh không thể là ngày trong tương lai.");
                    return View("Edit", data);
                }

                data.BirthDate = parsedDate;
            }*/
            
            if (data.EmployeeID == 0)
            {
                int id =CommonDataService.AddEmployee(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.Email), "email bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateEmployee(data);
                if (result == false)
                {
                    ModelState.AddModelError(nameof(data.Email), "email bị trùng");
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }

            var data = CommonDataService.GetEmployee(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
    }
}
