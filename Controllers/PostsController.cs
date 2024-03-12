using AppContext = MvcApp.Models.AppContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;

namespace MvcApp.Controllers;
public class PostsController : Controller
{
    public IActionResult Page(int id = 1)
    {
        using AppContext db = new();
        var pgn = new Paginator(id, db);
        ViewBag.Pgn = pgn;
        ViewBag.Posts = db.Posts.OrderByDescending(x => x.id).
            Skip(pgn.SkipValue).Take(pgn.PageSize).ToList();
        return View();
    }
    public IActionResult Post(int id)
    {
        using AppContext db = new();
        if (Request.Method == "POST")
        {
            PostManager.AddComment(Request.Form["comment"],
                id, User.Identity.Name, db);
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
                User.Identity.Name, Request.Form.Files[0]);
            PostManager.LoadPostToDb(post, Request.Form["tags"], db);
        }
        return View();
    }
    public IActionResult Edit(int postId)
    {
        using(AppContext db = new())
        {
            Post post = db.Posts.FirstOrDefault(p => p.id == postId);
            if(post.user != User.Identity.Name)
                return Unauthorized();
            ViewData["path"] = post.path;
            if(Request.Method == "POST")
            {
                post.description = Request.Form["description"].ToString() ?? post.description;
                db.Posts.Update(post);
                db.SaveChanges();
                return Redirect($"~/posts/post?id={post.id}");
            }
        }
        return View();
    } 
    
    public IActionResult Delete(int postId)
    {
        using AppContext db = new();
        var post = db.Posts.FirstOrDefault(p => p.id == postId);
        if (post.user != User.Identity.Name)
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