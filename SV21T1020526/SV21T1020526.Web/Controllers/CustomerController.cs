using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SV21T1020526.BusinessLayers;
using SV21T1020526.DomainModels;
using SV21T1020526.Web.Appcodes;
using SV21T1020526.Web.Models;
using static SV21T1020526.Web.Models.PaginationSearchResult;

namespace SV21T1020526.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR},{WebUserRoles.EMPLOYEE}")]   
    public class CustomerController : Controller
    {
        private const int PAGE_SIZE = 30;
        private const string CUSTOMER_SERCH_CONDITION = "CustomerSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SERCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput() {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue=""
                };
            return View(condition);
        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data =CommonDataService.ListOfCustomer(out rowCount,condition.Page,condition.PageSize,condition.SearchValue??"");
            CustomerSearchResult model = new CustomerSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(CUSTOMER_SERCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            var data = new Customer()
            {
                CustomerId = 0,
            };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            var data = CommonDataService.GetCustomer(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Customer data)
        {
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName),"Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của khách hàng");
            if(string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Hãy chọn tỉnh/thành cho khách hàng");
            //dựa cào thuộc tính Isvalid của modelstate để viết rõ có lỗi tồn tại hay không
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }
            if (data.CustomerId == 0)
            {
                int id =CommonDataService.AddCustomer(data);
                if (id <= 0) {
                    ModelState.AddModelError(nameof(data.Email), "email bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
               bool result = CommonDataService.UpdateCustomer(data);
                if (result == false) {
                    
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetCustomer(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
    }
}
