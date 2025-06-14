using PracticeApi.Middleware.Interfaces;

namespace PracticeApi.Middleware
{
    public class TotalRecordCountMiddleware
    {
        private readonly RequestDelegate _next;

        public TotalRecordCountMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITotalCountAccessor totalCountAccessor)
        {
            context.Response.OnStarting(() =>
            {
                if (totalCountAccessor.HasValue)
                {
                    context.Response.Headers["X-Total-Count"] = totalCountAccessor.TotalCount.ToString();
                }
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }

}
