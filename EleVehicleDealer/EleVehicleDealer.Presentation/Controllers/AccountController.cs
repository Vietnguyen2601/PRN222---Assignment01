using EleVehicleDealer.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EleVehicleDealer.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var account = await _accountService.LoginAsync(username, password);

            if (account == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
                return View();
            }

            // ✅ Đăng nhập thành công, có thể lưu session
            HttpContext.Session.SetString("Username", account.Username);
            HttpContext.Session.SetInt32("AccountId", account.AccountId);

            return RedirectToAction("Index", "Vehicle");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
