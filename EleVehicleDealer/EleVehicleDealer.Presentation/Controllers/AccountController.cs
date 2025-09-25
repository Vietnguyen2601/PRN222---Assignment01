using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.EntityModels;
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

            // Đăng nhập thành công, có thể lưu session
            HttpContext.Session.SetString("Username", account.Username);
            HttpContext.Session.SetInt32("AccountId", account.AccountId);

            return RedirectToAction("Index", "Vehicle");
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string email, string? contactNumber)
        {
            // Kiểm tra username đã tồn tại
            var exists = await _accountService.ExistsAsync(username);
            if (exists)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View();
            }

            var emailExists = await _accountService.ExistsEmailAsync(email);
            if (emailExists)
            {
                ViewBag.Error = "Email đã tồn tại!";
                return View();
            }

            var newAccount = new Account
            {
                Username = username,
                Password = password, //Demo: nên hash trong production
                Email = email,
                ContactNumber = contactNumber,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            // Gán mặc định role "Customer"
            newAccount.AccountRoles.Add(new AccountRole
            {
                RoleId = 3, // giả sử trong DB RoleId = 2 là Customer
                IsActive = true
            });

            await _accountService.RegisterAsync(newAccount);

            TempData["Success"] = "Đăng ký thành công, vui lòng đăng nhập!";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
