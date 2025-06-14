namespace PracticeApi.Middleware.Interfaces
{
    public interface ITotalCountAccessor
    {
        int TotalCount { get; set; }
        bool HasValue { get; }
    }
}
