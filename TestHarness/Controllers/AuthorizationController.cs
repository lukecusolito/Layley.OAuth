using Layley.OAuth.Handlers;
using Layley.OAuth.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace TestHarness.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        #region Fields
        private readonly OAuthAuthorizationHandler _oAuthAuthorization;
        private readonly OAuthConfig _oAuthConfig = new OAuthConfig
        {
            ConsumerKey = "",
            ConsumerSecret = "",
            RequestTokenUrl = "",
            AuthorizeTokenUrl = "",
            AccessTokenUrl = "",
            CallbackUrl = ""
        };
        #endregion

        #region Constructor
        public AuthorizationController()
        {
            _oAuthAuthorization = new OAuthAuthorizationHandler(_oAuthConfig);
        }
        #endregion

        [HttpGet]
        [Route("RequestToken")]
        public async Task<OAuthRequestToken> GetRequestToken()
        {
            return await _oAuthAuthorization.GetRequestTokenAsync();
        }

        [HttpGet]
        [Route("AuthorizationUrl")]
        public string GetAuthorizationUrl([FromQuery]string requestToken)
        {
            return _oAuthAuthorization.GetAuthorizationUrl(requestToken);
        }

        [HttpGet]
        [Route("AccessToken")]
        public async Task<OAuthAccessToken> GetAccessToken([FromQuery(Name = "oauth_token")]string requestToken, [FromQuery(Name = "oauth_consumer_key")]string requestTokenSecret, [FromQuery(Name = "oauth_verifier")]string verifier)
        {
            return await _oAuthAuthorization.GetAccessTokenAsync(requestToken, requestTokenSecret, verifier);
        }
    }
}