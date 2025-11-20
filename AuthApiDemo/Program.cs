using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 讀取 JWT 設定
var jwtConfig = builder.Configuration.GetSection("JWT"); // 從 appsettings.json 裡，找到"JWT"設定把它讀成一個 設定物件
var key = Encoding.UTF8.GetBytes(jwtConfig["Key"]!); // 把 Key 字串轉成 byte[]

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  // 開啟 Authentication（開門）
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,                  // 要求檢查 Token 的簽發者（Issuer）
                ValidateAudience = true,                // 要求檢查 Token 是不是給對象使用的
                ValidateLifetime = true,                // Token 過期就不能再用
                ValidateIssuerSigningKey = true,        // 檢查 Token 是否被竄改
                ValidIssuer = jwtConfig["Issuer"],      // 設定 Issuer（簽發者）
                ValidAudience = jwtConfig["Audience"],  // 設定 Audience（受眾）
                IssuerSigningKey = new SymmetricSecurityKey(key) // 設定簽章金鑰
            };
        });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
