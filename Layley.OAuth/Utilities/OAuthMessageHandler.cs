using System.Net.Http;
using System.Threading.Tasks;

namespace Layley.OAuth.Utilities
{
    internal class OAuthMessageHandler : DelegatingHandler
    {
        #region Fields
        private readonly Consumer _consumer;
        #endregion

        #region Constructor
        public OAuthMessageHandler(Consumer consumer) : base(new HttpClientHandler())
        {
            _consumer = consumer;
        }
        #endregion

        #region Public Methods
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            request.Headers.Authorization = _consumer.GetOAuthHeader(request.RequestUri, request.Method);

            return base.SendAsync(request, cancellationToken);
        }
        #endregion
    }
}
