namespace Layley.OAuth.Models
{
    public class OAuthConfig
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string CallbackUrl { get; set; }
        public string RequestTokenUrl { get; set; }
        public string AuthorizeTokenUrl { get; set; }
        public string AccessTokenUrl { get; set; }
    }
}
