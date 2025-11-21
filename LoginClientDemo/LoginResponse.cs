namespace LoginClientDemo
{
    internal class LoginResponse // 存放「後端回傳的 JSON」
    {
        public string Token { get; set; } = "";

        public string UserName { get; set; } = "";
    }
}
