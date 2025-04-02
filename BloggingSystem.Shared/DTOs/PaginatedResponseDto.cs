namespace BloggingSystem.Shared.DTOs;

public class PaginatedResponseDto<T> : ResponseDto<List<T>>
{
    
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
    
    public static PaginatedResponseDto<T> Create(
        List<T> items, 
        int count, 
        int pageIndex, 
        int pageSize, 
        string message = "Data retrieved successfully")
    {
        return new PaginatedResponseDto<T>
        {
            Success = true,
            Message = message,
            Data = items,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };
    }
}