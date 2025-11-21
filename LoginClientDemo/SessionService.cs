namespace LoginClientDemo
{
    public class SessionService // 存放「登入後要全程使用的資料」
    {
        // 登入成功後存 Token
        public static string JwtToken { get; set; } = "";

        // 存登入者名稱（ex: jimmy）
        public static string Username { get; set; } = "";
    }
}
