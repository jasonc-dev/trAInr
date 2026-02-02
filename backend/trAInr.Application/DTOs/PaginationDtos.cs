namespace trAInr.Application.DTOs;

/// <summary>
///     Request parameters for paginated endpoints
/// </summary>
public record PagedRequest
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 20;

    private int _page = 1;
    private int _pageSize = DefaultPageSize;

    /// <summary>
    ///     Page number (1-based)
    /// </summary>
    public int Page
    {
        get => _page;
        init => _page = value < 1 ? 1 : value;
    }

    /// <summary>
    ///     Number of items per page (max 100)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value switch
        {
            < 1 => DefaultPageSize,
            > MaxPageSize => MaxPageSize,
            _ => value
        };
    }

    /// <summary>
    ///     Calculated skip value for database queries
    /// </summary>
    public int Skip => (Page - 1) * PageSize;
}

/// <summary>
///     Paginated response wrapper
/// </summary>
public record PagedResponse<T>
{
    public IEnumerable<T> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public static PagedResponse<T> Create(IEnumerable<T> items, int page, int pageSize, int totalCount)
    {
        return new PagedResponse<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
