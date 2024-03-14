
namespace MvcApp.Models;

public readonly struct Paginator
{
    public readonly int SkipValue;
    public readonly int PageCount;
    public readonly int CurPage;
    public readonly int PageSize;
    public readonly int FirstPage;
    public readonly int LastPage;
    public Paginator(int curPage, int maxPosts)
    {

        CurPage = (curPage < 1) ? 1 : curPage;
        PageSize = maxPosts > 18? 18 : maxPosts;
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