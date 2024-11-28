using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ----- 認証・認可 ----------------------------------
var secret = builder.Configuration.GetValue<string>("JwtSettings:SecretKey") ?? string.Empty;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ValidateLifetime = true,
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddAuthorization();
// ---------------------------------------------------

var app = builder.Build();




app.UseHttpsRedirection();

// 認証・認可
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
