using Microsoft.EntityFrameworkCore;

namespace MvcApp.Models;

public static class PostManager
{
    public static void LoadPostToDb(Post post, string tagString, AppContext db)
    {
        db.Add(post);
        db.SaveChanges();
        var newPost = db.Posts.FirstOrDefault(p => p.Path == post.Path);
        string[] tags = tagString.Split(",", StringSplitOptions.TrimEntries);
        if(tags.Length > 0 && newPost != null)
        {
            var tagArr = CreateTagArr(tags, newPost.Id);
            foreach(var tag in tagArr)
            {
                db.Add(tag);
                TagCountIncrease(tag.TagString, db);
            }
            db.SaveChanges();
        }
    }
    public static Post CreatePost(string Description, int userId, IFormFile file)
    {
        Post post = new()
        {
            TimeCreated = DateTime.Now,
            UserId = userId,
            Description = Description,
            Path = @$"/server/{Path.GetRandomFileName()}{Path.GetRandomFileName()}.{file.ContentType.Split("/")[1]}"
        };
        using FileStream fileStream = new("wwwroot/" + post.Path, FileMode.Create);
            file.CopyTo(fileStream);
        return post;
    }
    public static PostPage GetPostPage(int postId, AppContext db)
    {
        var post = db.Posts.Include(p => p.Tags).FirstOrDefault(p => p.Id == postId);
        var temp = post.Tags.Select(t => t.TagString).ToList();
        var tags = new List<TagCount>(temp.Count);
        for (int i = 0; i < temp.Count; i++)
        {
            tags.Add(db.TagsCount.Where(tc => tc.TagString == temp[i]).First());
        }
        var username = db.Users.First(u => u.Id == post.UserId).Username;
        var commentSection = db.Comments.Include(c => c.User)
            .Where(c => c.PostId == postId).ToList();
        PostPage postPage = new()
        {
            Post = post,
            Username = username,
            Tags = tags,
            CommentSection = commentSection
        };
        return postPage;
    }
    public static void DeletePost(Post post, AppContext db)
    {
        foreach (var tag in post.Tags)
            TagCountDecrease(tag.TagString, db);
        File.Delete($"{Directory.GetCurrentDirectory()}/wwwroot{post.Path}");
        db.Posts.Remove(post);
        db.SaveChanges();
    }
    internal static void EditPost(Post post, string Description, string tagString, AppContext db)
    {
        post.Description = Description;
        db.Posts.Update(post);
        foreach (var tag in db.Tags.Where(t => t.PostId == post.Id).ToList())
        {
            TagCountDecrease(tag.TagString, db);
            db.Tags.Remove(tag);
        }
        db.SaveChanges();
        var tagArr = CreateTagArr(tagString.Split(',', StringSplitOptions.RemoveEmptyEntries), post.Id).ToList();
        foreach(var tag in tagArr)
        {
            TagCountIncrease(tag.TagString, db);
            db.Add(tag);
        }
        db.SaveChanges();
    }
    public static int GetRandomPostId()
    {
        using (AppContext db = new())
        {
            var count = db.Posts.Count();
            if(count == 0)
                return 0;
            var number = new Random().Next(0, count);
            return db.Posts.Skip(number).First().Id;
        }
    }
    public static Tag[] CreateTagArr(string[] tags, int postId)
    {
        Tag[] tagArr = new Tag[tags.Length];
        for(int i = 0; i < tagArr.Length; i++)
        {
            tagArr[i] = new Tag
            {
                PostId = postId,
                TagString = tags[i]
            };
        }
        return tagArr;
    }
    public static void AddComment(string comment, int postId, int userId, AppContext db)
    {
        Comment comm = new()
        {
            UserId = userId,
            CommentString = comment,
            PostId = postId,
            TimeCreated = DateTime.Now
        };
        db.Add(comm);
        db.SaveChanges();
    }
    public static void TagCountIncrease(string tagString, AppContext db)
    {
        var tagCount = db.TagsCount.FirstOrDefault(t => t.TagString == tagString);
        if(tagCount == null)
        {
            tagCount = new TagCount(){TagString = tagString, Count = 1};
            db.TagsCount.Add(tagCount);
        }
        else
        {
            tagCount.Count++;
            db.TagsCount.Update(tagCount);
        }
    }
    public static void TagCountDecrease(string tagString, AppContext db)
    {
        var tagCount = db.TagsCount.FirstOrDefault(t => t.TagString == tagString);
        if(--tagCount.Count < 1)
            db.TagsCount.Remove(tagCount);
        else
            db.TagsCount.Update(tagCount);
    }
}
public struct PostPage
{
    public IEnumerable<Comment> CommentSection {get;set;}
    public string Username {get; set;}
    public IEnumerable<TagCount> Tags {get;set;}
    public Post Post {get;set;}

}