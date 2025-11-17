using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.Domain.DTOs.Accounts;
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

            // Đăng nhập thành công -> lưu session
            HttpContext.Session.SetString("Username", account.Username);
            HttpContext.Session.SetInt32("AccountId", account.AccountId);

            var roles = account.Roles ?? new List<string>();

            if (roles.Contains("Admin") || roles.Contains("Staff"))
            {
                HttpContext.Session.SetString("Role", roles.FirstOrDefault() ?? "Staff");
                return RedirectToAction("Manage", "Home");
            }
            else if (roles.Contains("Customer"))
            {
                HttpContext.Session.SetString("Role", "Customer");
                return RedirectToAction("Home", "Home");
            }

            // Trường hợp không có role
            ViewBag.Error = "Tài khoản chưa được phân quyền!";
            return View();
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

            var newAccount = new AccountCreateDto
            {
                Username = username,
                Password = password,
                Email = email,
                ContactNumber = contactNumber,
                RoleIds = new List<int> { 3 }
            };

            await _accountService.RegisterAsync(newAccount);

            TempData["Success"] = "Đăng ký thành công, vui lòng đăng nhập!";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Index()
        {
            var accounts = await _accountService.GetAllAsync();
            return View("AccountManagement", accounts);
        }

        public IActionResult Create()   
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Create(string username, string password, string email, string? contactNumber, bool isActive = true)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var dto = new AccountCreateDto
            {
                Username = username,
                Password = password,
                Email = email,
                ContactNumber = contactNumber,
                RoleIds = new List<int> { 2 }
            };

            await _accountService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var account = await _accountService.GetByIdAsync(id);
            if (account == null) return NotFound();
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int accountId, string username, string? password, string email, string? contactNumber, bool isActive = true)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));

            var dto = new AccountUpdateDto
            {
                AccountId = accountId,
                Username = username,
                Password = password,
                Email = email,
                ContactNumber = contactNumber,
                IsActive = isActive,
                RoleIds = new List<int>()
            };

            await _accountService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _accountService.SoftDeleteAsync(id);

            if (!result)
                return Json(new { success = false, message = "Account not found" });

            return Json(new { success = true, message = "Delete thành công!" });
        }
    }
}
