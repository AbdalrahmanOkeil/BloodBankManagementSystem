using BloodBank.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace BloodBank.API.Extensions
{
    public static class ResultExtentions
    {
        public static IActionResult ToApiResponse<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(new ApiResponse<T>(true, "Success", result.Value));

            return new BadRequestObjectResult(new ApiResponse<T>(false, result.Error, default));
        }
    }
}
