using System.Collections.Generic;

namespace Layley.OAuth.Models
{
    public class OAuthAccessToken
    {
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public Dictionary<string, string> RawParameters { get; set; }
    }
}
