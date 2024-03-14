
namespace MvcApp.Models;

public static class PostManager
{
    public static void LoadPostToDb(Post post, string tags, AppContext db)
    {
        db.Add(post);
        db.SaveChanges();
        post = db.Posts.FirstOrDefault(p => p.path == post.path);
        if(tags != null && tags.Length > 0 && post != null)
        {
            var tagArr = CreateTagArr(tags, post.id);
            foreach(var tag in tagArr)
                db.Add(tag);
            db.SaveChanges();
        }
    }
    public static Post CreatePost(string description, int userId, IFormFile file)
    {
        Post post = new()
        {
            time_created = DateTime.Now,
            user_id = userId,
            description = description,
            path = @$"/server/{Path.GetRandomFileName()}{Path.GetRandomFileName()}.{file.ContentType.Split("/")[1]}"
        };
        using FileStream fileStream = new("wwwroot/" + post.path, FileMode.Create);
            file.CopyTo(fileStream);
        return post;
    }
    public static PostPage GetPostPage(int postId, AppContext db)
    {
        var post = db.Posts.FirstOrDefault(p => p.id == postId);
        var tags = db.Tags.Where(t => t.post_id == postId).Select(t => t.tag).ToList();
        var username = db.Users.FirstOrDefault(u => u.user_id == post.user_id).username;
        var comments = db.Comments.Join(db.Users,
                c => c.user_id, 
                u => u.user_id,
                (c, u) => new CommentWithUser
                {
                    comment_id = c.comment_id,
                    username = u.username,
                    post_id = c.post_id,
                    comment = c.comment,
                    time_created = c.time_created
                }).Where(c => c.post_id == postId).ToList();
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
        foreach (var tag in db.Tags.Where(t => t.post_id == post.id))
            db.Tags.Remove(tag);
        foreach (var comment in db.Comments.Where(c => c.post_id == post.id))
            db.Comments.Remove(comment);
        db.SaveChanges();
        File.Delete($"{Directory.GetCurrentDirectory()}/wwwroot{post.path}");
        db.Posts.Remove(post);
        db.SaveChanges();
    }
    internal static void EditPost(Post post, string description, string tagString, AppContext db)
    {
        post.description = description;
        db.Posts.Update(post);
        foreach (var tag in db.Tags.Where(t => t.post_id == post.id))
            db.Tags.Remove(tag);
        db.SaveChanges();
        var tagArr = CreateTagArr(tagString, post.id);
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
            return db.Posts.Skip(number).First().id;
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
                post_id = postId,
                tag = tags[i]
            };
        }
        return tagArr;
    }
    public static void AddComment(string comment, int postId, int userId, AppContext db)
    {
        Comment comm = new()
        {
            user_id = userId,
            comment = comment,
            post_id = postId,
            time_created = DateTime.Now
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