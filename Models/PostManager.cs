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
}