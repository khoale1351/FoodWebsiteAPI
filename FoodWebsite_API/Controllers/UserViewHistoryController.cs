using Microsoft.AspNetCore.Mvc;

namespace FoodWebsite_API.Controllers
{
    public class UserViewHistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
