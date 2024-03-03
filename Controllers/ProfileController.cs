using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;
using AppContext = MvcApp.Models.AppContext;

namespace MvcApp.Controllers
{

    public class ProfileController : Controller
    {
        public IActionResult Page()
        {
            return View();
        }
        public IActionResult Settings()
        {
            return View();
        }
    }
}