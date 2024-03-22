
namespace MvcApp.Models;

public static class Search
{
    public static List<Post> SearchByTags(string tagString, AppContext db, out int maxPosts)
    {
        string[] tags = tagString.Split(",", StringSplitOptions.TrimEntries);
        var idArr = db.Tags.Where(t => t.TagString == tags[0]).Select(t => t.PostId);
        for(int i = 1; i < tags.Length; i++)
        {
            var temp = db.Tags.Where(t => t.TagString == tags[i]).Select(t => t.PostId);
            idArr = temp.Intersect(idArr);
        }
        var posts = db.Posts.Where(p => idArr.Contains(p.Id)).ToList();
        maxPosts = posts.Count;
        return posts;
    }
}