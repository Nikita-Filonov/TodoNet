using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetPasswordController : Controller
    {
        private readonly TodoContext _context;
        public static string FrameworkDescription { get; }

        public ResetPasswordController(TodoContext context)
        {
            _context = context;
        }

        [HttpPost("/api/v1/users/reset-password")]
        public IActionResult ResetPassword([FromBody] UserPublicDto user)
        {
            MailMessage mailMessage = new() { ToEmail = user.Email };
            _context.MailMessages.AddRange(mailMessage);
            _context.SaveChanges();

            string resetPassworUrl = $"https://flower-todo.herokuapp.com/reset-password/{mailMessage.Id}";
            string subject = "Flower Todo Reset Password";
            string body = "<h1>Hi</h1>" +
                "<p>You got this message because someone (hopefully you), " +
                "requested password reset.</p>" +
                $"<p>Follow this <a href='{resetPassworUrl}'>link</a> to reset your password.</p>";

            try
            {
                Utils.MailMessages.SendMail(user.Email, subject, body);
            }
            catch
            {
                return BadRequest(new { 
                  message = "Please provide, correct email adress.",
                  level = "danger"
                });
            }
            
            return Ok(new {
              message = $"Password reset was sent to {user.Email}",
              level = "success"
            });
        }

        [HttpPost("/api/v1/users/reset-password/{mailMessagedGuid}")]
        public IActionResult ResetPasswordForm([FromBody] UserResetPasswordDto user, Guid mailMessagedGuid)
        {
            if (user.NewPassword != user.ConfirmPassword)
            {
                return Utils.Errors.PasswordsDidNotMatch();
            }

            var isEmailExists = _context.MailMessages.Any(m => m.Id == mailMessagedGuid);
            if (!isEmailExists)
            {
                return Utils.Errors.ForbiddenError();
            }

            var mailMessage = _context.MailMessages.Find(mailMessagedGuid);
            var dbUser = _context.Users
                .Where(u => u.Email == mailMessage.ToEmail)
                .FirstOrDefault();

            if (dbUser == null | mailMessage.IsClosed)
            {
                return Utils.Errors.ForbiddenError();
            }

            dbUser.Password = user.NewPassword;
            mailMessage.IsClosed = true;
            _context.SaveChanges();

            return Ok(new { 
              message = "Password was successfully reseted.",
              level = "success"
            });
        }

        [Authorize]
        [HttpPost("/api/v1/users/change-password")]
        public IActionResult ChangePassword([FromBody] UserResetPasswordDto user)
        {
            if (user.NewPassword != user.ConfirmPassword)
            {
                return Utils.Errors.PasswordsDidNotMatch();
            }
            
            var dbUser = _context.Users.Find((int)Int64.Parse(User.Identity.Name));
            if (dbUser.Password != user.CurrentPassword)
            {
                return BadRequest(new { 
                  message = "Please check if current password is correct.",
                  level = "danger"
                });
            }


            dbUser.Password = user.NewPassword;
            _context.SaveChanges();

            return Ok(new { 
              message = "Password successfully changed.",
              level = "success"
            });
        }
    }
}
