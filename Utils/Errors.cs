using Microsoft.AspNetCore.Mvc;

namespace WebApi.Utils
{
    public static class Errors
    {
        public static IActionResult ForbiddenError() =>
        new ObjectResult(new
            {
                message = "Operation rejected",
                level = "danger"
            }
        )
        { StatusCode = 403 };
    }
}
