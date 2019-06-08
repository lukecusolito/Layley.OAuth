using Layley.OAuth.Helpers;
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
        private readonly OAuthAuthorizationHelper _oAuthAuthorization;
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
            _oAuthAuthorization = new OAuthAuthorizationHelper(_oAuthConfig);
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
    }
}