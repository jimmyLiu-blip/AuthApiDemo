using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

builder.Services.
    AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AuthApiDemo", Version = "v1" }); // 在 Swagger 頁面左上角生成：「AuthApiDemo v1」

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme // AddSecurityDefinition = 告訴 Swagger：我們要有「Bearer Token」輸入欄位
    {
        In = ParameterLocation.Header, // Swagger 要把 Token 放在 HTTP Header 裡
        Description = "請輸入 JWT Token，格式：Bearer <token>",
        Name = "Authorization", // Header 的名稱固定叫 Authorization
        Type = SecuritySchemeType.ApiKey, // Swagger UI 的顯示方式 → 用「輸入欄位」呈現
        Scheme = "Bearer" // 告訴 Swagger：這個 Token 是 "Bearer" 格式
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement // Swagger：請幫我讓所有 API 都使用剛剛設定的 "Bearer Token"
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference // 告訴 Swagger：「我要用剛才那個叫 Bearer 的安全機制
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{ }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthorization();

app.MapControllers();

app.Run();
