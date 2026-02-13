using BookHistoryApi.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BookHistoryApi.Middleware
{
    public class ExcepitonMiddleware : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var problemDetails = CreateProblemDetails(exception);

            httpContext.Response.StatusCode = problemDetails.Status!.Value;

            await httpContext.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken);

            return true;
        }


        private static ProblemDetails CreateProblemDetails(Exception exception)
        {
            return exception switch
            {
                BookNotFoundException ex => new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Book not found",
                    Detail = ex.Message
                },

                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal server error",
                    Detail = "An unexpected error occurred."
                }
            };
        }
    }
}
