namespace CollectIt.Database.Abstractions;

public class PagedResult<TItem>
{
    public IEnumerable<TItem> Result { get; set; }
    public int TotalCount { get; set; }
}