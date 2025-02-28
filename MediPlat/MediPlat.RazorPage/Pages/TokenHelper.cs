namespace MediPlat.RazorPage.Pages
{
    public static class TokenHelper
    {
        public static string GetCleanToken(HttpContext httpContext)
        {
            var token = httpContext?.Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }

            if (token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length);
            }

            return token;
        }
    }

}
