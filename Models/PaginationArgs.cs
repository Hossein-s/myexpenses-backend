namespace MyExpenses.Models;

public class PaginationArgs
{
    private int _page = 1;
    public int Page
    {
        get
        {
            int lastPage = (int)Math.Ceiling((decimal)Total / PageSize);
            if (_page > lastPage)
                return lastPage;
            if (_page < 1)
                return 1;
            return _page;
        }
        set { _page = value; }
    }

    public int PageSize { get; set; } = 10;

    public int Total { get; set; }
}
