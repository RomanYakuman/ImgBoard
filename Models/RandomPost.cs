namespace MvcApp.Models;

public class RandomPost
{
    public static int GetRandomPostId()
    {
        using (AppContext db = new AppContext())
        {
            var count = db.Posts.Count();
            if(count == 0)
                return 0;
            var number = new Random().Next(0, count);
            return db.Posts.Skip(number).First().id;
        }
    }
}