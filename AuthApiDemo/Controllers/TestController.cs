using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApiDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        // [Authorize] 代表要帶合法 JWT 才能進來
        [HttpGet("hello")]
        [Authorize] // 這個 API 必須帶 JWT Token 才能進來
        public ActionResult<string> Hello()
        {
            var userName = User.Identity?.Name ?? "Unknown";
            return Ok($"Hello, {userName}! 這是需要登入才能看到的訊息。");
        }
    }
}

