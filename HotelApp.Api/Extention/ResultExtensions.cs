using HotelApp.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Extention
{
    public static class ResultExtensions
    {
        public static IActionResult ToProblem(this Result result)
        {
            var error = result.Errors.First();

            var status = error.ErrorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.InvaildCredentias => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            return new ObjectResult(new ProblemDetails
            {
                Status = status,
                Title = error.Code,
                Detail = error.Description
            })
            { StatusCode = status };
        }
    }
}
