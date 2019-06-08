using Layley.OAuth.Models;
using Layley.OAuth.Utilities;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Layley.OAuth.Handlers
{
    public class OAuthAuthorizationHandler
    {
        #region Fields
        private readonly OAuthConfig _oAuthConfig;

        private const string OAUTH_REQUEST_TOKEN_RESPONSE_REGEX = "oauth_token=(?<oauth_token>(?:\\w|\\-)*)&oauth_token_secret=(?<oauth_token_secret>(?:\\w)*)&oauth_callback_confirmed=(?<oauth_callback_confirmed>(?:\\w)*)";
        private const string OAUTH_ACCESS_TOKEN_RESPONSE_REGEX = "oauth_token=(?<oauth_token>(?:\\w|\\-)*)&oauth_token_secret=(?<oauth_token_secret>(?:\\w)*)&user_id=(?<user_id>(?:\\w)*)&screen_name=(?<screen_name>(?:\\w)*)";
        #endregion

        #region Constructor
        public OAuthAuthorizationHandler(OAuthConfig oAuthConfig)
        {
            _oAuthConfig = oAuthConfig;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Obtains a request token
        /// </summary>
        /// <returns></returns>
        public async Task<OAuthRequestToken> GetRequestTokenAsync()
        {
            var consumer = new Consumer(_oAuthConfig.ConsumerKey, _oAuthConfig.ConsumerSecret, callbackUrl: _oAuthConfig.CallbackUrl);

            using (var client = new OAuthHttpClient(consumer))
            {
                // Get Request Token
                var result = await client.GetAsync(_oAuthConfig.RequestTokenUrl);

                if (!result.IsSuccessStatusCode) // TODO: Replace with meaningful error message
                    throw new Exception($"Unable to get request token: {result.ReasonPhrase}");

                // Read token parameters
                var requestTokenParameters = await result.Content.ReadAsStringAsync();
                var requestTokenInformation = Regex.Match(requestTokenParameters, OAUTH_REQUEST_TOKEN_RESPONSE_REGEX);

                // Verify if callback is confirmed
                bool callbackConfirmed;
                if (!bool.TryParse(requestTokenInformation.Groups["oauth_callback_confirmed"].Value, out callbackConfirmed) || !callbackConfirmed)
                    return null;

                // Get token properties
                var requestToken = requestTokenInformation.Groups["oauth_token"].Value;
                var requestTokenSecret = requestTokenInformation.Groups["oauth_token_secret"].Value;

                return new OAuthRequestToken
                {
                    RequestToken = requestToken,
                    RequestTokenSecret = requestTokenSecret
                };
            }
        }

        /// <summary>
        /// Generates an authorization URL
        /// </summary>
        /// <param name="requestToken"></param>
        /// <returns></returns>
        public string GetAuthorizationUrl(string requestToken) =>
            $"{_oAuthConfig.AuthorizeTokenUrl}?oauth_token={requestToken}";

        /// <summary>
        /// Exchanges request token for an access token
        /// </summary>
        /// <param name="requestToken"></param>
        /// <param name="requestTokenSecret"></param>
        /// <param name="verifier"></param>
        /// <returns></returns>
        public async Task<OAuthAccessToken> GetAccessTokenAsync(string requestToken, string requestTokenSecret, string verifier)
        {
            var consumer = new Consumer(_oAuthConfig.ConsumerKey, _oAuthConfig.ConsumerSecret, requestToken, requestTokenSecret, verifier);

            using (var client = new OAuthHttpClient(consumer))
            {
                // Get Access Token
                var result = await client.PostAsync(_oAuthConfig.AccessTokenUrl, null);

                if (!result.IsSuccessStatusCode) // TODO: Replace with meaningful error message
                    throw new Exception($"Unable to get access token: {result.ReasonPhrase}");

                // Read token parameters
                var accessTokenParameters = await result.Content.ReadAsStringAsync();
                var accessTokenInformation = Regex.Match(accessTokenParameters, OAUTH_ACCESS_TOKEN_RESPONSE_REGEX);
                
                // Get token properties
                var accessToken = accessTokenInformation.Groups["oauth_token"].Value;
                var accessTokenSecret = accessTokenInformation.Groups["oauth_token_secret"].Value;

                return new OAuthAccessToken
                {
                    AccessToken = accessToken,
                    AccessTokenSecret = accessTokenSecret
                };
            }
        }
        #endregion
    }
}
