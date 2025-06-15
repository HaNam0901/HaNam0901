using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020526.BusinessLayers;
using SV21T1020526.DomainModels;
using SV21T1020526.Shop;

using System.Data;

namespace SV21T1020526.Shop.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{

		[AllowAnonymous]
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public async Task<IActionResult> Login(string username, string password)
		{
			// Kiểm tra thông tin đầu vào
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
			{
				ModelState.AddModelError("Error", "Vui lòng nhập đầy đủ tên tài khoản và mật khẩu.");
				return View();
			}

			// Kiểm tra tài khoản người dùng
			UserAccount userAccount = UserAccountService.Authorize(UserTypes.customer, username, password);


			if (userAccount == null)
			{
				ModelState.AddModelError("Error", "Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.");
				return View();
			}

			// Đăng nhập thành công
			WebUserData userData = new WebUserData()
			{
				UserId = userAccount.UserId,
				UserName = userAccount.UserName,
				DisplayName = userAccount.DisplayName,
				Photo = userAccount.Photo,
				Roles = userAccount.RoleNames.Split(',').ToList()
			};

			// Tạo giấy chứng nhận và ghi nhận trạng thái đăng nhập
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());

			// Quay về trang chủ
			return RedirectToAction("Index", "Home");
		}


		public async Task<IActionResult> Logout()
		{
			HttpContext.Session.Clear();
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login");
		}
		public IActionResult ChangePassword()
		{

			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
		{
			string message = string.Empty;
			// Kiểm tra mật khẩu xác nhận có khớp với mật khẩu mới không
			if (newPassword != confirmPassword)
			{
				ModelState.AddModelError("Error", "Mật khẩu xác nhận không khớp.");
				return View();
			}

			var username = User.GetUserData().UserName;  // Lấy tên người dùng hiện tại          
														 // Gọi phương thức UpdateUserPassword để thực hiện thay đổi mật khẩu
			bool isPasswordUpdated = UserAccountService.UpdateUserPassword(username, oldPassword, newPassword);

			// Nếu việc thay đổi mật khẩu thành công
			if (isPasswordUpdated)
			{

				message = "Đổi mật khẩu thành công";
				SetAlert(message, isPasswordUpdated);
				return View();
			}

			// Nếu không thành công, hiển thị thông báo lỗi
			ModelState.AddModelError("Err", "Sai mật khẩu . Vui lòng nhập lại mật khẩu.");
			return View();
		}

		protected void SetAlert(string message, bool type)
		{
			TempData["AlertMessage"] = message;
			if (true)
				TempData["AlertType"] = "alert-success";


		}
		[AllowAnonymous]
		public IActionResult Register(int id = 0)
		{
			ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
			
			// Lấy dữ liệu từ service
			var data = CommonDataService.GetCustomer(id);
			
			// Nếu không có dữ liệu, khởi tạo một đối tượng rỗng để tránh null
			if (data == null)
			{
				data = new Customer(); // Khởi tạo dữ liệu mặc định
			}

			return View(data); // Truyền dữ liệu tới view
		}
		[AllowAnonymous]
		[HttpPost]
		public IActionResult Register(Customer data)
		{
			if (string.IsNullOrWhiteSpace(data.CustomerName))
				ModelState.AddModelError(nameof(data.CustomerName), "Tên người dùng không được để trống");
			if (string.IsNullOrWhiteSpace(data.ContactName))
				ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
			if (string.IsNullOrWhiteSpace(data.Phone))
				ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại của bạn");
			if (string.IsNullOrWhiteSpace(data.Email))
				ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email của bạn");
			if (string.IsNullOrWhiteSpace(data.Address))
				ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của bạn");
			if (string.IsNullOrWhiteSpace(data.Address))
				ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập mật khẩu");
			if (string.IsNullOrWhiteSpace(data.Province))
				ModelState.AddModelError(nameof(data.Province), "Hãy chọn tỉnh/thành bạn đang sống");
			//dựa cào thuộc tính Isvalid của modelstate để viết rõ có lỗi tồn tại hay không
			if (ModelState.IsValid == false)
			{
				return View("Register", data);
			}
			if (data.CustomerId == 0)
			{
				int id = UserAccountService.Register(data);
				if (id <= 0)
				{
					ModelState.AddModelError(nameof(data.Email), "email đã tồn tại");
					return View("Register", data);
				}
			}
			else
			{
				bool result = CommonDataService.UpdateCustomer(data);
				if (result == false)
				{

					return View("Login", data);
				}
			}
			return RedirectToAction("Login");
		}
	}
}
