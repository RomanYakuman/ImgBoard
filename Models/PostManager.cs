namespace MvcApp.Models;

public static class PostManager
{
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
    public static Post GetPostById(int id)
    {
        using (AppContext db = new())
        {
            return db.Posts.FirstOrDefault(p => p.id == id);
        }
    }
    public static void DeletePost(Post post, AppContext db)
    {
        File.Delete($"{Directory.GetCurrentDirectory()}/wwwroot{post.path}");
        db.Posts.Remove(post);
        db.SaveChangesAsync();
    }
    public static void AddComment(string comment, int postId, string user)
    {
        Comment comm = new();
        comm.user = user;
        comm.comment = comment;
        comm.post_id = postId;
        comm.time_created = DateTime.Now;
        using (AppContext db = new())
        {
            db.Add(comm);
            db.SaveChanges();
        }
    }
    public static List<Comment> GetCommentSection(int postId)
    {
        using(AppContext db = new())
        {
            return db.Comments.Where(c => c.post_id == postId)
                .OrderByDescending(c =>  c.time_created).ToList();
        }
    }
}