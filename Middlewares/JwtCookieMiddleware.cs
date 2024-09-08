namespace locket.Middlewares
{
    public class JwtCookieMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Cookies.ContainsKey("token"))
            {
                var token = context.Request.Cookies["token"];
                if (!String.IsNullOrEmpty(token))
                {
                    context.Request.Headers.Append("Authorization", $"Bearer {token}");
                }
            }

            await _next(context);

        }
    }
}
