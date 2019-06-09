# Layley.OAuth
A simplified OAuth 1.0 consumer

## Useage

### Calling a OAuth endpoint
```cs
//
// Using custom OAuthHttpClient
//
var consumer = new Consumer("ConsumerKey", "ConsumerSecret", "AccessToken", "AccessTokenSecret");

using (var client = new OAuthHttpClient(consumer))
{
    var result = await client.GetAsync("http://api.com/data");
    var data = await result.Content.ReadAsStringAsync();
}

//
// Manually using HttpClient
//
var consumer = new Consumer("ConsumerKey", "ConsumerSecret", "AccessToken", "AccessTokenSecret");

using (var client = new HttpClient())
{
    var uri = new Uri("http://api.com/data");
    var method = HttpMethod.Get;

    var message = new HttpRequestMessage();
    message.RequestUri = uri;
    message.Method = method;
    message.Headers.Authorization = consumer.GetOAuthHeader(uri, method);

    var result = await client.SendAsync(message);
    var data = await result.Content.ReadAsStringAsync();
}
//
// For the raw http authorization value use the following. This will need to be prefixed with 'OAuth '
//
string oAuthHeader = consumer.GetOAuthHeaderValue(uri, method);
```

### Obtaining an OAuth token - 3 legged
```cs
// Define configuration for 3-legged oAuth
OAuthConfig oAuthConfig = new OAuthConfig
{
    ConsumerKey = "ConsumerKey",
    ConsumerSecret = "ConsumerSecret",
    RequestTokenUrl = "http://api.com/oauth/requestoken",
    AuthorizeTokenUrl = "http://api.com/oauth/authorize",
    AccessTokenUrl = "http://api.com/oauth/accesstoken",
    CallbackUrl = "http://myapi.com/oauth/callback"
};

// OAuth handler to assist with obtaining a token
var authHandler = new OAuthAuthorizationHandler(oAuthConfig);

// Step 1: Obtain a request token
var requestToken = await authHandler.GetRequestTokenAsync();

// Step 2: Generate an authorization URL. Use this URL to authorize with the provider application
var authorizeUrl = authHandler.GetAuthorizationUrl(requestToken.RequestToken);

// Step 3: Exchange the request token for an access token. Verifier is available via callback of the authorization provider
var accessToken = authHandler.GetAccessTokenAsync(requestToken.RequestToken, requestToken.RequestTokenSecret, "verifier");
```

## TODO
* Implement token store
* NuGet
