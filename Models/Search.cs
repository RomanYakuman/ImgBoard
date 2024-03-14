
namespace MvcApp.Models;

public static class Search
{
    public static List<Post> SearchByTags(string tagString, AppContext db, out int maxPosts)
    {
        string[] tags = tagString.Split(",", StringSplitOptions.TrimEntries);
        IEnumerable<int> idArr = db.Tags.Where(t => t.tag == tags[0]).Select(t => t.post_id).ToArray();
        for(int i = 1; i < tags.Length; i++)
        {
            IEnumerable<int> temp = db.Tags.Where(t => t.tag == tags[i]).Select(t => t.post_id).ToArray();
            idArr = temp.Intersect(idArr);
        }
        var posts = db.Posts.Where(p => idArr.Contains(p.id)).ToList();
        maxPosts = posts.Count;
        return posts;
    }
}