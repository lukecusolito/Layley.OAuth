using Layley.OAuth.Models;
using Layley.OAuth.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Layley.OAuth.Handlers
{
    public class OAuthAuthorizationHandler
    {
        #region Fields
        private readonly OAuthConfig _oAuthConfig;
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
                var rawRequestTokenParameters = await result.Content.ReadAsStringAsync();
                var requestTokenParameters = GetParametersFromString(rawRequestTokenParameters);

                // Verify if callback is confirmed
                bool callbackConfirmed;
                if (!bool.TryParse(requestTokenParameters["oauth_callback_confirmed"], out callbackConfirmed) || !callbackConfirmed)
                    return null;
                
                return new OAuthRequestToken
                {
                    RequestToken = requestTokenParameters["oauth_token"],
                    RequestTokenSecret = requestTokenParameters["oauth_token_secret"],
                    RawParameters = requestTokenParameters
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
                var rawAccessTokenParameters = await result.Content.ReadAsStringAsync();
                var accessTokenParameters = GetParametersFromString(rawAccessTokenParameters);

                return new OAuthAccessToken
                {
                    AccessToken = accessTokenParameters["oauth_token"],
                    AccessTokenSecret = accessTokenParameters["oauth_token_secret"],
                    RawParameters = accessTokenParameters
                };
            }
        }
        #endregion

        #region Private Methods
        private Dictionary<string, string> GetParametersFromString(string parameterString)
        {
            var parameters = new Dictionary<string, string>();

            var keyValuePairs = parameterString.Split('&');

            foreach (var kvp in keyValuePairs)
            {
                var split = kvp.Split('=');
                parameters.Add(split[0], split[1]);
            }

            return parameters;
        }
        #endregion
    }
}
