﻿using AppContext = MvcApp.Models.AppContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.EntityFrameworkCore;

namespace MvcApp.Controllers;
public class PostsController : Controller
{
    public IActionResult Page(int Id = 1, string tags = "")
    {
        using AppContext db = new();
        Paginator pgn;
        ViewData["tags"] = tags;
        if (tags == "")
        {
            pgn = new Paginator(Id, db.Posts.Count());
            ViewBag.Posts = db.Posts.OrderByDescending(x => x.Id).
                Skip(pgn.SkipValue).Take(pgn.PageSize).ToList();
        }
        else
        {
            var posts = Search.SearchByTags(tags, db, out int maxPosts);
            pgn = new Paginator(Id, maxPosts);
            ViewBag.Posts = posts.Skip(pgn.SkipValue).Take(pgn.PageSize).ToList();
        }
        ViewBag.Pgn = pgn;
        return View();

    }
    public IActionResult Post(int Id)
    {
        using AppContext db = new();
        if (Request.Method == "POST")
        {
            PostManager.AddComment(Request.Form["comment"],
                Id, db.Users.FirstOrDefault( u => u.Username == User.Identity.Name).Id, db);
            return Redirect($"/posts/post?Id={Id}");
        }
        try
        {
            var postPage = PostManager.GetPostPage(Id, db);
            ViewBag.PostPage = postPage;
        }
        catch (RuntimeBinderException)
        {
            return NotFound();
        }
        return View();
    }
    public IActionResult Tags(int Id = 1)
    {
        using AppContext db = new();
        var tags = db.TagsCount;
        var pgn = new Paginator(Id, tags.Count());
        ViewBag.Tags = tags.OrderByDescending(t => t.Count)
            .Skip(pgn.SkipValue).Take(pgn.PageSize).ToList();
        ViewBag.Pgn = pgn;
        return View();
    }
    [Authorize]
    public IActionResult Upload()
    {
        if (Request.Method == "POST")
        {
            using AppContext db = new();
            if (Request.Form.Files.Count < 1)
                return BadRequest();
            var post = PostManager.CreatePost(Request.Form["Description"],
                db.Users.FirstOrDefault(u => u.Username == User.Identity.Name).Id, Request.Form.Files[0]);
            PostManager.LoadPostToDb(post, Request.Form["tags"], db);
        }
        return View();
    }
    [Authorize]
    public IActionResult Edit(int Id)
    {
        using (AppContext db = new())
        {
            Post post = db.Posts.FirstOrDefault(p => p.Id == Id);
            if (post.UserId != db.Users.FirstOrDefault(u => u.Username == User.Identity.Name).Id)
                return Unauthorized();
            var tags = db.Tags.Where(t => t.PostId == post.Id).Select(t => t.TagString);
            ViewData["tags"] = string.Join(',', tags);
            ViewData["Description"] = post.Description;
            ViewData["Path"] = post.Path;
            if (Request.Method == "POST")
            {
                PostManager.EditPost(post, Request.Form["Description"], Request.Form["tags"], db);
                return Redirect($"~/posts/post?Id={post.Id}");
            }
        }
        return View();
    }
    [Authorize]
    public IActionResult Delete(int Id)
    {
        using AppContext db = new();
        var post = db.Posts.Include(p => p.Tags).FirstOrDefault(p => p.Id == Id);
        if (post.UserId != db.Users.FirstOrDefault(u => u.Username == User.Identity.Name).Id)
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