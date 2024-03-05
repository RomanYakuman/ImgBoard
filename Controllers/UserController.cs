using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;
using AppContext = MvcApp.Models.AppContext;

namespace MvcApp.Controllers
{

    public class UserController : Controller
    {
        public IActionResult Profile(string username)
        {
            ViewData["username"] = username;
            return View();
        }
        public IActionResult Settings()
        {
            return View();
        }
    }
}