using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SV21T1020526.Web
{
    /// <summary>
    /// lưu giữ thông tin của người dùng lưu trong cooki
    /// </summary>
    public class WebUserData
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Photo { get; set; } = "";
        public List<string>? Roles { get; set; }
        /// <summary>
        /// Danh sách các claim
        /// </summary>
        private List<Claim> Claims
        {
            get
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(nameof(UserId), UserId),
                    new Claim(nameof(UserName), UserName),
                    new Claim(nameof(DisplayName), DisplayName),
                    new Claim(nameof(Photo), Photo)
                };
                if (Roles != null)
                    foreach (var role in Roles)

                        claims.Add(new Claim(ClaimTypes.Role, role));

                return claims;
            }
        }

        public ClaimsPrincipal CreatePrincipal()
        {
            var identity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }

    }
}

