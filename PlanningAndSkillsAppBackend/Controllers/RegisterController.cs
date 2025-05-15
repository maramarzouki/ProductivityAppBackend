using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Admin;

namespace PlanningAndSkillsAppBackend.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IAdminService _adminService;
        public RegisterController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // GET: /Register
        [HttpGet]
        public IActionResult Index()
        {
            return View();  // Views/Register/Index.cshtml
        }

        // POST: /Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View();
            }

            var success = await _adminService.CreateAsync(username, password);
            if (success)
            {
                // Optionally, auto-login or redirect to login page
                return RedirectToAction("Index", "Login");
            }

            ModelState.AddModelError("", "That username is already taken.");
            return View();
        }
    }
}
