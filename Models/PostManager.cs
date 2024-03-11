namespace MvcApp.Models;

public static class PostManager
{
    public static void CreatePost(IFormFileCollection file, string tags, string user, string description)
    {
        var post = new Post
        {
            time_created = DateTime.Now,
            user = user,
            description = description,
            path = @$"/server/{Path.GetRandomFileName()}{Path.GetRandomFileName()}.{file[0].ContentType.Split("/")[1]}"
        };
        using (AppContext db = new())
        {
            db.Add(post);
            db.SaveChanges();
            using FileStream fileStream = new("wwwroot/" + post.path, FileMode.Create);
                file[0].CopyTo(fileStream);
            post = db.Posts.FirstOrDefault(p => p.path == post.path);
            if(tags != null && tags.Length > 0 && post != null)
            {
                var tagArr = CreateTagArr(tags, post.id);
                foreach(var tag in tagArr)
                    db.Add(tag);
                db.SaveChanges();
            }
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
    public static PostPage GetPostPage(int postId)
    {
        PostPage postPage = new();
        using(AppContext db = new())
        {
            postPage.post = db.Posts.FirstOrDefault(p => p.id == postId);
            postPage.tags =  db.Tags.Where(t => t.post_id == postId).ToList();
            postPage.commentSection = db.Comments.Where(c => c.post_id == postId)
                .OrderByDescending(c =>  c.time_created).ToList();
        }
        return postPage;
    }
    public static void DeletePost(Post post, AppContext db)
    {
        File.Delete($"{Directory.GetCurrentDirectory()}/wwwroot{post.path}");
        db.Posts.Remove(post);
        db.SaveChangesAsync();
    }
    public static void AddComment(string comment, int postId, string user)
    {
        Comment comm = new()
        {
            user = user,
            comment = comment,
            post_id = postId,
            time_created = DateTime.Now
        };
        using (AppContext db = new())
        {
            db.Add(comm);
            db.SaveChanges();
        }
    }
}
public struct PostPage
{
    public List<Comment> commentSection {get;set;}
    public List<Tag> tags {get;set;}
    public Post post {get;set;}

}