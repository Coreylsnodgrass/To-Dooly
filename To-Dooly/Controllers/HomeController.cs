using Microsoft.AspNetCore.Mvc;

namespace ToDooly.Controllers
{
    public class HomeController : Controller
    {
        // GET: /
        public IActionResult Index()
        {
            // You can redirect to Dashboard if you don't want a separate home page
            // return RedirectToAction("Index", "Dashboard");
            return View();
        }

        // GET: /Home/Privacy
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
