namespace BloggingSystem.Shared.Models;

public class SortingParams
{
    public string SortBy { get; set; } = "CreatedAt";
    public bool Descending { get; set; } = true;
    
    public string GetSortExpression()
    {
        return Descending ? $"{SortBy} DESC" : SortBy;
    }
}