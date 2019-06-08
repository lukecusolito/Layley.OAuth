using System.Net.Http;

namespace Layley.OAuth.Utilities
{
    public class OAuthHttpClient : HttpClient
    {
        public OAuthHttpClient(Consumer consumer) : base(new OAuthMessageHandler(consumer)) { }
    }
}
