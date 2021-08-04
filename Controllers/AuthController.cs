using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class AuthController : Controller
    {
        private readonly TodoContext _context;

        public AuthController(TodoContext context)
        {
            _context = context;
        }

        [HttpPost("/api/v1/token")]
        public IActionResult Token([FromBody] User user)
        {
            var identity = GetIdentity(user.Email, user.Password);
            if (identity == null)
            {
                return BadRequest(new { 
                    message = "Invalid username or password.",
                    level = "danger"
                });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                userIdentity = identity.Name
            };

            return Json(response);
        }

        private ClaimsIdentity GetIdentity(string email, string password)
        {
            var userId = _context.Users
                .Where(user => user.Email == email && user.Password == password)
                .Select(user => user.Id)
                .FirstOrDefault();

            if (userId != 0)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, userId.ToString()),
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }
    }
}
