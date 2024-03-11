using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppContext = MvcApp.Models.AppContext;
using MvcApp.Models;

namespace MvcApp.Controllers;
public class PostsController : Controller
{
    public IActionResult Page(int id = 1)
    {   
        using (AppContext db = new())
        {
            int pageSize = db.Posts.Count() > 18? 18 : db.Posts.Count();
            var pgn = new Paginator(id, pageSize);
            ViewBag.pgn = pgn;
            ViewBag.Posts = db.Posts.OrderByDescending(x =>  x.id)
                .Skip(pgn.SkipValue).Take(pageSize).ToList();
        }
        return View();
    }
    public IActionResult Post(int id)
    {
        var postPage = PostManager.GetPostPage(id);
        ViewBag.postPage = postPage;
        if(Request.Method == "POST")
        {
            PostManager.AddComment(Request.Form["comment"]
            , id, User.Identity.Name);
            return Redirect($"/posts/post?id={id}");
        }   
        return View();
    }
    [Authorize]
    public IActionResult Upload(int id)
    {
        var request = HttpContext.Request;
        if (request.Method == "POST")
        {
            PostManager.CreatePost(request.Form.Files, request.Form["tags"]
            , User.Identity.Name, Request.Form["description"]);
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
            ViewBag.post = post;
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
        using(AppContext db = new())
        {
            var post = db.Posts.FirstOrDefault(p => p.id == postId);
            if(post.user != User.Identity.Name)
                return Unauthorized();
            else if(post != null && Request.Method == "POST")
            {
                PostManager.DeletePost(post, db);
                return View();
            }
            else
                return NotFound();
        }
    }
}