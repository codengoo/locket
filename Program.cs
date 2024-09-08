using locket.Helpers;
using locket.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = (new AppConfig(builder.Configuration)).Option;

// Add MVC 
builder.Services.AddMvc();
builder.Services.AddSingleton<AppConfig>();
// Add JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.JWTKey))
        };
    });

builder.Services.AddAuthorization();

// 

var app = builder.Build();

// Add custom middlewares
app.UseMiddleware<JwtCookieMiddleware>();

// Add middleware to the request pipeline
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
