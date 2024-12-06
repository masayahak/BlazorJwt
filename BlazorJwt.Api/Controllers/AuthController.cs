using BlazorJwt.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorJwt.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IConfiguration configuration) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginModel loginModel)
        {
            var token = GenerateJwtToken(loginModel.Username, loginModel.Password);
            await Task.CompletedTask;

            if (token != string.Empty)
            {
                return Ok(token);
            }
            else
            {
                return Unauthorized(string.Empty);
            }
        }

        // Logoutは不要
        // クライアント側で保持していたJWTをクライアント側で削除し
        // クライアント側のステータスのみ更新すればログアウトは成功
        // サーバー側のログアウト処理はない


        // ========================================================
        // JWT TOKEN GENERATOR
        // ========================================================

        private string GenerateJwtToken(string userName, string password)
        {
            List<Claim>? claims = GenerateClaims(userName, password);
            if (claims is null) { return string.Empty; }

            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? string.Empty;
            var expiryMinutes = int.Parse(jwtSettings["ExpiryInMinutes"] ?? string.Empty);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private List<Claim>? GenerateClaims(string userName, string password)
        {
            // 認証チェックはダミー
            if (userName.StartsWith("admin@") && password == "admin")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Role, "Administrator")
                };
                return claims;
            }
            else if (userName.StartsWith("user@") && password == "user")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Role, "User")
                };
                return claims;
            }

            return null;
        }

    }
}
