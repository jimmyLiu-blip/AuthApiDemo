using AuthApiDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthApiDemo.Controllers  // 整個系統負責「登入、驗證身分、產生 JWT Token」的控制器
{
    [ApiController] // 每一個 API Controller（Controller class）都要寫一次
    [Route("api/[controller]")] //[controller] 會自動替換成 class 名的前面部分： 在 Controller（類別）上寫一次 [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
        // ActionResult<LoginResponse>：把它想成 WinForms 最後會收到的資料格式
        // LoginResponse是 成功時回傳的 JSON 格式型別
        // [FromBody] 代表：WinForms 或前端送的 JSON，要從 Body 讀進來轉成 LoginRequest
        // 我會接收一個 LoginRequest (來自 Body 的 JSON) ； 我會回傳一個 ActionResult<LoginResponse> 給前端
        {
            // 1. 驗證帳密（先用寫死測試，之後再改成資料庫）
            if (!ValidateUser(request.Account, request.Password))
            {
                return Unauthorized("帳號或密碼錯誤");
            }

            // 2. 產生 JWT
            var token = GenerateJwtToken(request.Account);

            return Ok(new LoginResponse
            {
                Token = token,
                UserName = request.Account
            });
        }

        private bool ValidateUser(string account, string password)
        {
            // ◎ 練習階段：寫死帳號
            // 之後你可以改成查 DB, AD 登入等等
            return account == "jimmy" && password == "1234";
        }

        private string GenerateJwtToken(string account)
        {
            var jwtConfig = _config.GetSection("JWT");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, account),
                new Claim("account", account),
                new Claim(ClaimTypes.Name, account),
                // 之後可以加角色：new Claim(ClaimTypes.Role, "Engineer")
            };

            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtConfig["ExpireMinutes"]!)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
