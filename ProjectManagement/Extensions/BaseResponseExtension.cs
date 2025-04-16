using ProjectManagement.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManagement.Api.Extensions
{
    public static class BaseResponseExtension
    {
        public static ObjectResult ToHttpResult<T>(this BaseResponse<T> response)
        {
            return response.ErrorCode switch
            {
                StatusCodes.Status400BadRequest => new BadRequestObjectResult(response),
                StatusCodes.Status404NotFound => new NotFoundObjectResult(response),
                _ => new OkObjectResult(response),
            };
        }
    }
}
