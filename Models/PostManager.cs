
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
    public static Post CreatePost(string description, string user, IFormFile file)
    {
        Post post = new()
        {
            time_created = DateTime.Now,
            user = user,
            description = description,
            path = @$"/server/{Path.GetRandomFileName()}{Path.GetRandomFileName()}.
                {file.ContentType.Split("/")[1]}"
        };
        using FileStream fileStream = new("wwwroot/" + post.path, FileMode.Create);
            file.CopyTo(fileStream);
        return post;
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
    public static PostPage GetPostPage(int postId, AppContext db)
    {
        PostPage postPage = new();
        postPage.post = db.Posts.FirstOrDefault(p => p.id == postId);
        postPage.tags =  db.Tags.Where(t => t.post_id == postId).ToList();
        postPage.commentSection = db.Comments.Where(c => c.post_id == postId)
            .OrderByDescending(c =>  c.time_created).ToList();
        return postPage;
    }
    public static void DeletePost(Post post, AppContext db)
    {
        File.Delete($"{Directory.GetCurrentDirectory()}/wwwroot{post.path}");
        db.Posts.Remove(post);
        db.SaveChangesAsync();
    }
    public static void AddComment(string comment, int postId, string user, AppContext db)
    {
        Comment comm = new()
        {
            user = user,
            comment = comment,
            post_id = postId,
            time_created = DateTime.Now
        };
        db.Add(comm);
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
}
public struct PostPage
{
    public List<Comment> commentSection {get;set;}
    public List<Tag> tags {get;set;}
    public Post post {get;set;}

}