using AppContext = MvcApp.Models.AppContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;

namespace MvcApp.Controllers;
public class PostsController : Controller
{
    public IActionResult Page(int id = 1, string tags = "")
    {
        using AppContext db = new();
        Paginator pgn;
        ViewData["tags"] = tags;
        if (tags == "")
        {
            pgn = new Paginator(id, db.Posts.Count());
            ViewBag.Posts = db.Posts.OrderByDescending(x => x.id).
                Skip(pgn.SkipValue).Take(pgn.PageSize).ToList();
        }
        else
        {
            var posts = Search.SearchByTags(tags, db, out int maxPosts);
            pgn = new Paginator(id, maxPosts);
            ViewBag.Posts = posts.Skip(pgn.SkipValue).Take(pgn.PageSize).ToList();
        }
        ViewBag.Pgn = pgn;
        return View();

    }
    public IActionResult Post(int id)
    {
        using AppContext db = new();
        if (Request.Method == "POST")
        {
            PostManager.AddComment(Request.Form["comment"],
                id, db.Users.FirstOrDefault( u => u.username == User.Identity.Name).user_id, db);
            return Redirect($"/posts/post?id={id}");
        }
        ViewBag.PostPage = PostManager.GetPostPage(id, db);
        return View();
    }
    [Authorize]
    public IActionResult Upload(int id)
    {
        if (Request.Method == "POST")
        {
            using AppContext db = new();
            if (Request.Form.Files.Count < 1)
                return BadRequest();
            var post = PostManager.CreatePost(Request.Form["description"],
                db.Users.FirstOrDefault(u => u.username == User.Identity.Name).user_id, Request.Form.Files[0]);
            PostManager.LoadPostToDb(post, Request.Form["tags"], db);
        }
        return View();
    }
    public IActionResult Edit(int id)
    {
        using (AppContext db = new())
        {
            Post post = db.Posts.FirstOrDefault(p => p.id == id);
            if (post.user_id != db.Users.FirstOrDefault(u => u.username == User.Identity.Name).user_id)
                return Unauthorized();
            var tags = db.Tags.Where(t => t.post_id == post.id).Select(t => t.tag);
            ViewData["tags"] = string.Join(',', tags);
            ViewData["description"] = post.description;
            ViewData["path"] = post.path;
            if (Request.Method == "POST")
            {
                PostManager.EditPost(post, Request.Form["description"], Request.Form["tags"], db);
                return Redirect($"~/posts/post?id={post.id}");
            }
        }
        return View();
    }

    public IActionResult Delete(int id)
    {
        using AppContext db = new();
        var post = db.Posts.FirstOrDefault(p => p.id == id);
        if (post.user_id != db.Users.FirstOrDefault(u => u.username == User.Identity.Name).user_id)
            return Unauthorized();
        else if (post != null && Request.Method == "POST")
        {
            PostManager.DeletePost(post, db);
            return View();
        }
        else
            return NotFound();
    }
}