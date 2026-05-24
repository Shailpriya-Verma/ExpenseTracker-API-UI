using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.MVC.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult ReportDashboard()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if(string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login","Auth");
            }
            return View();
        }
    }
}
