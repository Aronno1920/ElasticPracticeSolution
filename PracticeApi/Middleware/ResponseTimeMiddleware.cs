using System.Diagnostics;

namespace PracticeApi.Middleware
{
    public class ResponseTimeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseTimeMiddleware> _logger;

        public ResponseTimeMiddleware(RequestDelegate next, ILogger<ResponseTimeMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var watch = Stopwatch.StartNew();
            context.Response.OnStarting(() =>
            {
                watch.Stop();
                var responseTimeForCompleteRequest = watch.ElapsedMilliseconds;
                context.Response.Headers["X-Response-Time-ms"] = responseTimeForCompleteRequest.ToString();
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }

}
