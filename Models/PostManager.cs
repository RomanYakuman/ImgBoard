
namespace MvcApp.Models;

public static class PostManager
{
    public static void LoadPostToDb(Post post, string tags, AppContext db)
    {
        db.Add(post);
        db.SaveChanges();
        var newPost = db.Posts.FirstOrDefault(p => p.Path == post.Path);
        if(tags != null && tags.Length > 0 && newPost != null)
        {
            var tagArr = CreateTagArr(tags, newPost.Id);
            foreach(var tag in tagArr)
                db.Add(tag);
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
        var post = db.Posts.FirstOrDefault(p => p.Id == postId);
        var tags = db.Tags.Where(t => t.PostId == postId).Select(t => t.TagString).ToList();
        var username = db.Users.FirstOrDefault(u => u.UserId == post.UserId).Username;
        var comments = db.Comments.Join(db.Users,
                c => c.UserId, 
                u => u.UserId,
                (c, u) => new CommentWithUser
                {
                    CommentId = c.CommentId,
                    Username = u.Username,
                    PostId = c.PostId,
                    CommentString = c.CommentString,
                    TimeCreated = c.TimeCreated
                }).Where(c => c.PostId == postId).ToList();
        PostPage postPage = new()
        {
            Post = post,
            Username = username,
            Tags = tags,
            CommentSection =  comments
        };
        return postPage;
    }
    public static void DeletePost(Post post, AppContext db)
    {
        foreach (var comment in db.Comments.Where(c => c.PostId == post.Id))
            db.Comments.Remove(comment);
        db.SaveChanges();
        File.Delete($"{Directory.GetCurrentDirectory()}/wwwroot{post.Path}");
        db.Posts.Remove(post);
        db.SaveChanges();
    }
    internal static void EditPost(Post post, string Description, string tagString, AppContext db)
    {
        post.Description = Description;
        db.Posts.Update(post);
        foreach (var tag in db.Tags.Where(t => t.PostId == post.Id))
            db.Tags.Remove(tag);
        db.SaveChanges();
        var tagArr = CreateTagArr(tagString, post.Id);
        foreach(var tag in tagArr)
            db.Add(tag);
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
    public static Tag[] CreateTagArr(string tagString, int postId)
    {
        string[] tags = tagString.Split(",", StringSplitOptions.TrimEntries);
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
}
public struct PostPage
{
    public List<CommentWithUser> CommentSection {get;set;}
    public string Username {get; set;}
    public List<string> Tags {get;set;}
    public Post Post {get;set;}

}