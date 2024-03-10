using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;
using AppContext = MvcApp.Models.AppContext;

namespace MvcApp.Controllers;
public class PostsController : Controller
{
    public IActionResult Page(int id = 1)
    {   
        using (AppContext db = new())
        {
            int pageSize = db.Posts.Count() > 18? 18 : db.Posts.Count();
            var pgn = new Paginator(id, pageSize);
            ViewData["MaxPage"] = pgn.PageCount;
            ViewData["CurPage"] = pgn.CurPage;
            ViewBag.PagArr = pgn.PagArr;
            ViewBag.Posts = db.Posts.OrderByDescending(x =>  x.id)
                .Skip(pgn.SkipValue).Take(pageSize).ToList();
        }
        return View();
    }
    public IActionResult Page(string tags, int id = 1)
    {   
        using (AppContext db = new())
        {
            int pageSize = db.Tags.Where(t => t.tag == tags).Count() > 18? 18 : db.Tags.Where(t => t.tag == tags).Count();
            var pgn = new Paginator(id, pageSize);
            ViewData["MaxPage"] = pgn.PageCount;
            ViewData["CurPage"] = pgn.CurPage;
            ViewBag.PagArr = pgn.PagArr;
            ViewBag.Posts = db.Posts.OrderByDescending(x =>  x.id)
                .Skip(pgn.SkipValue).Take(pageSize).ToList();
        }
        return View();
    }
    public IActionResult Post(int id)
    {
        Post post = PostManager.GetPostById(id);
        ViewBag.comments = PostManager.GetCommentSection(id);
        ViewBag.tags = PostManager.GetTagsById(id);
        ViewBag.post = post;
        if(Request.Method == "POST")
        {
            PostManager.AddComment(Request.Form["comment"]
            , post.id, User.Identity.Name);
            return Redirect($"/posts/post?id={post.id}");
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
    public IActionResult Edit(int id)
    {
        using(AppContext db = new())
        {
            Post post = PostManager.GetPostById(id);
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
    
    public IActionResult Delete(int id)
    {
        using(AppContext db = new())
        {
            var post = PostManager.GetPostById(id);
            if(post.user != User.Identity.Name)
                return Unauthorized();
            if(post != null && Request.Method == "POST")
            {
                PostManager.DeletePost(post, db);
                return View();
            }
            else
                return NotFound();
        }
    }
}