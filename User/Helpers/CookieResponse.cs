namespace Locket.UserLocket.Helpers
{
    public class CookieResponse
    {
        public static void AddTokenCookie(HttpResponse response, string token)
        {
            CookieOptions cookieOptions = new()
            {
                Expires = DateTime.Now.AddDays(1),
                HttpOnly = true,
                Secure = true
            };

            response.Cookies.Append("token", token, cookieOptions);
        }
    }
}
