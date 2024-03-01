using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;
using AppContext = MvcApp.Models.AppContext;


namespace MvcApp.Controllers
{
     public class AuthController : Controller
     {
          public IActionResult Login()
          {
               return View();
          }

          public IActionResult SignUp()
          {
               return View();
          }
     }
}