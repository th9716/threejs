using Microsoft.AspNetCore.Mvc;

namespace dbCompanyTest.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult Service()
        {
            if (HttpContext.Session.Keys.Contains("Account"))
                return View();
            else
                return RedirectToAction("login", "Staff_Home");
        }
    }
}
