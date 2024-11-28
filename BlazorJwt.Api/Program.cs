using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ----- �F�؁E�F�� ----------------------------------
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

// �F�؁E�F��
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
