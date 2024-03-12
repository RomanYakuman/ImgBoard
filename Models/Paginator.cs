
namespace MvcApp.Models;

public struct Paginator
{
    public readonly int SkipValue;
    public readonly int PageCount;
    public readonly int CurPage;
    public readonly int PageSize;
    public readonly int FirstPage;
    public readonly int LastPage;
    public Paginator(int curPage, AppContext db)
    {

        CurPage = (curPage < 1) ? 1 : curPage;
        var maxPosts = db.Posts.Count();
        PageSize = maxPosts > 18? 18 : db.Posts.Count();
        PageCount =(int)Math.Ceiling(maxPosts/(double)PageSize);
        CurPage = (CurPage > PageCount) ? PageCount : CurPage;
        SkipValue = (CurPage - 1) * PageSize;
        if (PageCount <= CurPage + 7)
        {
            FirstPage = int.Max(1,PageCount-7);
            LastPage = PageCount;
        }
        else
        {
            FirstPage = CurPage;
            LastPage = CurPage+7;
        }
    }
}