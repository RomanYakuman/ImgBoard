using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;
using AppContext = MvcApp.Models.AppContext;

namespace MvcApp.Controllers
{
    public class PostsController : Controller
    {
        public IActionResult Page(int id = 1)
        {   
            using (AppContext db = new())
            {
                int pageSize = db.Posts.Count() > 30? 30 : db.Posts.Count();
                var pgn = new Paginator(id, pageSize);
                ViewData["MaxPage"] = pgn.PageCount;
                ViewData["CurPage"] = pgn.CurPage;
                ViewBag.PagArr = pgn.PagArr;
                ViewBag.Posts = db.Posts.OrderByDescending(x =>  x.id).Skip(pgn.SkipValue).Take(pageSize).ToList();
            }
            return View();
        }
        public IActionResult Post(int id)
        {
            var post = new Post().GetPostById(id);
            ViewData["path"] = post.path;
            return View();
        }
        [Authorize]
        public IActionResult Upload()
        {
            var request = HttpContext.Request;
            if (request.Method == "POST")
            {
                Post post = new();
                post.UploadToServer(request.Form.Files);
            }
            return View();
        }
    }
}