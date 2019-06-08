using System;
using System.Collections.Generic;
using System.Text;

namespace Layley.OAuth.Models
{
    public class OAuthRequestToken
    {
        public string RequestToken { get; set; }
        public string RequestTokenSecret { get; set; }
    }
}
