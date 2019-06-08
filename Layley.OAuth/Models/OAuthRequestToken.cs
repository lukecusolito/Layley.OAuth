using System.Collections.Generic;

namespace Layley.OAuth.Models
{
    public class OAuthRequestToken
    {
        public string RequestToken { get; set; }
        public string RequestTokenSecret { get; set; }
        public Dictionary<string, string> RawParameters { get; set; }
    }
}
