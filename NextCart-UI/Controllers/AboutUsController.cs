using Microsoft.AspNetCore.Mvc;

namespace NextCart_UI.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "About Us"; // Sets the title for the browser tab and for navbar active state
            return View();
        }
    }
}
