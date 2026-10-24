using HotelApp.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiBaseController : ControllerBase
    {
        protected static ActionResult<T> ToActionResult<T>(Result<T> result) =>
            result.IsSuccess
                ? new OkObjectResult(result.data)
                : ToProblem(result.Errors);

        protected static ActionResult ToActionResult(Result result) =>
            result.IsSuccess
                ? new OkResult()
                : ToProblem(result.Errors);

        protected static ObjectResult ToProblem(IReadOnlyList<Error> errors)
        {
            var firstError = errors.Count > 0
                ? errors[0]
                : new Error("Unexpected", "An unexpected error occurred.", ErrorType.Failure);

            var statusCode = firstError.ErrorType switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = firstError.Code,
                Detail = firstError.Description,
                Extensions = { ["errors"] = errors }
            };

            return new ObjectResult(problem) { StatusCode = statusCode };
        }

        protected string GetEmailFromToken() =>
            User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue("email")
            ?? throw new UnauthorizedAccessException("No email claim found.");
    }
}