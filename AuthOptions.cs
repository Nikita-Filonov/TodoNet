using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApi
{
    public class AuthOptions
    {
        public const string ISSUER = "LamaTime"; // издатель токена
        public const string AUDIENCE = "LamaTimeClient"; // потребитель токена
        const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
        public const int LIFETIME = 60 * 24 * 7; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
