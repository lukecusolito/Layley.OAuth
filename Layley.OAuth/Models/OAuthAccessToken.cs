using System;
using System.Collections.Generic;
using System.Text;

namespace Layley.OAuth.Models
{
    public class OAuthAccessToken
    {
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
    }
}
