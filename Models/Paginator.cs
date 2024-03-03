
namespace MvcApp.Models;

public class Paginator
{
    public readonly int[] PagArr;
    public readonly int SkipValue;
    public readonly int PageCount;
    public readonly int CurPage;
    public Paginator(int curPage, int pageSize)
    {
        CurPage = (curPage < 1) ? 1 : curPage;
        using (AppContext db = new())
        {
            var maxPosts = db.Posts.Count();
            if(maxPosts < 1)
            {
                PagArr = [1];
                CurPage = 1;
                SkipValue = 0;
            }
            else
            {
                PageCount =(int)Math.Ceiling(maxPosts/(double)pageSize);
                CurPage = (CurPage > PageCount) ? PageCount : CurPage;
                SkipValue = (CurPage - 1) * pageSize;
                if (PageCount <= CurPage + 7)
                    PagArr = createArray(int.Max(1,PageCount-7), PageCount);
                else
                    PagArr = createArray(CurPage, CurPage+7);
            }
            
        }
    }
    private int[] createArray(int lowerBound, int upperBound)
    {
        var diff = upperBound - lowerBound;
        var arr = new int[diff+1];
        for (int i = 0; i <= diff; i++)
        {
            arr[i] = lowerBound++;
        }
        return arr;
    }
}