using CMS.Auth;
using CMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IFreeSql freeSql;

        public AccountController(IFreeSql freeSql)
        {
            this.freeSql = freeSql;
        }

        [HttpPost("Login")]
        public async Task<LoginResultModel> Login(UserModel user)
        {
            var db_user = await freeSql.Select<users>()
                .Where(a => a.UserName == user.username)
                .FirstAsync();
            //var db_user = new users()
            //{
            //    ID=1,
            //    Name="666",
            //    Password="123"
            //};

            if (db_user == null)
                return new LoginResultModel { msg = "UserName Not Register!" };

            if (user.password != db_user.Password)
                return new LoginResultModel { msg = "Wrong Password!" };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid,db_user.ID.ToString()),
                new Claim(ClaimTypes.Name,db_user.Name),
            };
            //  token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secret_key_secret_key_secret_key_secret_key"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                issuer: "cms.jwt",
                audience: "https://localhost:5126",
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
                );
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            // cookies中
            Response.Cookies.Append("access_token", token);

            return new LoginResultModel { code = 1, msg = "Login Success" };
        }

        [HttpPost("Logout")]
        public async Task<string> Logout()
        {
            HttpContext.Response.Cookies.Delete("access_token");
            return "{}";
        }
    }
}
