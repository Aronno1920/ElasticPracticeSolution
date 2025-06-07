using Microsoft.AspNetCore.Mvc;

namespace SalesApi.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
