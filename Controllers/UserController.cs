using Microsoft.AspNetCore.Mvc;
using AppContext = MvcApp.Models.AppContext;

namespace MvcApp.Controllers
{

    public class UserController : Controller
    {
        public IActionResult Profile(string username)
        {
            if(HttpContext.Request.Method == "POST")
            {
                Response.Cookies.Delete("auth");
                return Redirect("~/");
            }
            using (AppContext db = new())
            {
                ViewBag.User = db.Users.FirstOrDefault(u => u.username == username);
            }
            return View();
        }
        public IActionResult Settings()
        {
            return View();
        }
    }
}