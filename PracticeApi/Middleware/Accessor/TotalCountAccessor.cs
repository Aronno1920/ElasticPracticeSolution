using PracticeApi.Middleware.Interfaces;

namespace PracticeApi.Middleware.Accessor
{
    public class TotalCountAccessor : ITotalCountAccessor
    {
        public int TotalCount { get; set; }
        public bool HasValue => TotalCount >= 0;
    }
}
