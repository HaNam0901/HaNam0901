using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SV21T1020526.BusinessLayers;
using SV21T1020526.DomainModels;
using SV21T1020526.Shop;
using SV21T1020526.Shop.Appcodes;
using SV21T1020526.Shop.Models;
using static SV21T1020526.Shop.Models.PaginationSearchResult;

namespace SV21T1020526.Shop.Controllers
{
	[Authorize(Roles = WebUserRoles.CUSTOMER)]
	public class CustomerController : Controller
    {
		
		public IActionResult Index()
        {
			if (Request.Method == "POST")
			{				
				return RedirectToAction("Edit");
			}
            int UserId = int.Parse( User.GetUserData().UserId);
			var data = CommonDataService.GetCustomer(UserId);
            if( data == null)
            {
                data = new Customer();
            }
			
			return View(data);
		}
        
       
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin cá nhân ";
            var data = CommonDataService.GetCustomer(id);
            if (data == null)
            {
                data = new Customer();
            }    
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
                    ModelState.AddModelError(nameof(data.Email), "email bị đã tồn tại");
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

        
    }
}
