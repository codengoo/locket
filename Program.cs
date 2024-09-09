using locket.Helpers;
using locket.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using locket.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using locket.Services;

var builder = WebApplication.CreateBuilder(args);
var config = (new AppConfig(builder.Configuration)).Option;

// ADD Service 
builder.Services.AddScoped<KafkaProducerService>();
builder.Services.AddHostedService<KafkaConsumerService>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddControllers();
builder.Services.AddSingleton<AppConfig>();
builder.Services.AddHttpClient();

// Config DTO validation
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        return new BadRequestObjectResult(
            new ApiResponse(null, "Invalid parameters", new SerializableError(context.ModelState))
        );
    };
});

// Add Context
builder.Services.AddDbContext<LocketDbContext>(option => option.UseNpgsql(config.Database.ConnectionString));

// Add JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Authentication.JWTKey))
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = config.Authentication.Google.ClientID;
        options.ClientSecret = config.Authentication.Google.ClientSecret;
        options.CallbackPath = new PathString("/auth/google-redirect");
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    });

builder.Services.AddAuthorization();
var app = builder.Build();

// Add custom middlewares
app.UseMiddleware<JwtCookieMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

// Add middleware to the request pipeline
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
