namespace Locket.UserLocket.Middlewares
{
    public class JwtCookieMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine(context.Request.Headers.Authorization);
            if (context.Request.Cookies.ContainsKey("token"))
            {
                var token = context.Request.Cookies["token"];
                Console.WriteLine(token);
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers.Append("Authorization", $"Bearer {token}");
                }
            }

            await _next(context);

        }
    }
}
