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

        public static IActionResult PasswordsDidNotMatch() =>
            new ObjectResult(new
              {
                message = "Two passwords did not match.",
                level = "danger"
            }
            )
            { StatusCode = 400 };
    }
}
