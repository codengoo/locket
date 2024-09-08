using locket.Helpers;
using locket.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using locket.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = (new AppConfig(builder.Configuration)).Option;

// Add MVC 
builder.Services.AddMvc();
builder.Services.AddSingleton<AppConfig>();

// Add Context
builder.Services.AddDbContext<LocketDbContext>(option => option.UseNpgsql(config.Database.ConnectionString));

// Add JWT
builder.Services.AddAuthentication(options =>
{
    //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = config.Authentication.Google.ClientID;
        options.ClientSecret = config.Authentication.Google.ClientSecret;
        options.CallbackPath = new PathString("/auth/google-redirect");
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Authentication.JWTKey))
        };
    });

builder.Services.AddAuthorization();

// 

var app = builder.Build();

// Add custom middlewares
//app.UseMiddleware<JwtCookieMiddleware>();

// Add middleware to the request pipeline
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
