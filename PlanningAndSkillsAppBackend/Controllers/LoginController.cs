using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Admin;

namespace PlanningAndSkillsAppBackend.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAdminService _adminService;
        public LoginController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        // GET: LoginController
        public ActionResult Index()
        {
            return View();
        }

        // POST: /Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string username, string password)
        {
            if (await _adminService.ValidateLoginAsync(username, password))
            {
                HttpContext.Session.SetString("AdminUser", username);
                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View();
        }

        // GET: /Login/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminUser");
            return RedirectToAction("Index");
        }
    }
}
