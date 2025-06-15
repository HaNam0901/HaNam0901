using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020526.BusinessLayers;
using SV21T1020526.DomainModels;
using SV21T1020526.Web;
using System.Data;

namespace SV21T1020526.Web.Controllers
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
            UserAccount userAccount = UserAccountService.Authorize(UserTypes.employee, username, password);
            

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

        public IActionResult AccessDenined()
        {
            return View();
        }
    }
}
