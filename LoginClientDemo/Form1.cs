using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LoginClientDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void txtAccount_Click(object sender, EventArgs e)
        {

        }

        private void txtPassword_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            var account = txtAccount.Text.Trim();

            var password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(account) || (string.IsNullOrWhiteSpace(password)))
            {
                MessageBox.Show("請輸入帳號和密碼");
                return;
            }

            // 要送給 WebAPI 的 JSON
            var loginData = new
            {
                account = account,
                password = password
            };

            try
            {
                // 建立一個網路請求物件 HttpClient()，並在用完後自動釋放
                using var client = new HttpClient();

                // 把 loginData序列化成 Json，用 POST 方法把 JSON 送到 WebAPI 的 /api/Auth/login
                var response = await client.PostAsJsonAsync("http://localhost:7225/api/Auth/login", loginData);

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("登入失敗!帳號或密碼錯誤");
                    return;
                }

                // // 解析回傳 JSON
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                // 保存 Token 與 UserName
                if (result == null)
                {
                    MessageBox.Show("登入回傳格式異常（result 為 null）");
                    return;
                }
                SessionService.JwtToken = result.Token;
                SessionService.Username = result.UserName;

                MessageBox.Show($"登入成功！歡迎 {result.UserName}");

                // ---------------------------------------------------------
                // Step 4：測試 Token 能否成功呼叫需要授權的 API
                // ---------------------------------------------------------

                using var client2 = new HttpClient();
                client2.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", SessionService.JwtToken);

                var test = await client2.GetStringAsync("http://localhost:7225/api/Test/hello");

                MessageBox.Show("授權 API 回傳：" + test);

                MessageBox.Show($"登入成功！歡迎 {result.UserName}");

                var main = new MainForm();
                main.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("發生錯誤：" + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


    }
}
