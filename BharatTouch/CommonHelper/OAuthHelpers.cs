using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DataAccess;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using iTextSharp.text.pdf.qrcode;
using Newtonsoft.Json;
using BharatTouch.CommonHelper;

namespace BharatTouch.CommonHelper
{
    public class OAuthHelpers
    {
        private HttpClient _httpClient = new HttpClient();

        #region Google OAuth
        async public Task<TokenResponse> ExchangeCodeForTokenGoogleAsync(string code, string userId, string redirectUri, string[] scopes)
        {
            try
            {
                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = ConfigValues.GoogleApiClientId,
                        ClientSecret = ConfigValues.GoogleApiClientSecret
                    },
                    Scopes = scopes // Your required scopes
                });

                var tokenResponse = await flow.ExchangeCodeForTokenAsync(
                    userId, // A unique identifier for the user (e.g., session ID, user ID)
                    code,
                    redirectUri,
                    System.Threading.CancellationToken.None
                );

                TokenResponse data = new TokenResponse();
                data.access_token = tokenResponse.AccessToken;
                data.refresh_token = tokenResponse.RefreshToken;
                data.token_type = tokenResponse.TokenType;
                data.expires_in = tokenResponse.ExpiresInSeconds.ToIntOrZero();

                return data;
            }
            catch (Google.Apis.Auth.OAuth2.Responses.TokenResponseException ex)
            {
                Console.WriteLine($"Google Token Exchange Error: {ex.Message}");
                Console.WriteLine($"Error Code: {ex.Error.Error}");
                Console.WriteLine($"Error Description: {ex.Error.ErrorDescription}");
                throw ex;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request Error during token exchange: {ex.Message}");
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async public Task<TokenResponse> RefreshAccessTokenGoogleAsync(string refreshToken)
        {
            try
            {
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = ConfigValues.GoogleApiClientId,
                    ["client_secret"] = ConfigValues.GoogleApiClientSecret,
                    ["refresh_token"] = refreshToken,
                    ["grant_type"] = "refresh_token"
                });
                var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", content);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                var responseString = await response.Content.ReadAsStringAsync();
                var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(responseString);

                if (tokenResponse?.access_token == null)
                {
                    return null;
                }
                return tokenResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        async public Task<GoogleUserInfo> GetGoogleUserInfo(string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<GoogleUserInfo>(json);
                }
            }

            return null;
        }
        #endregion

        #region Microsoft OAuth

        public static string[] microsoftOutlookScopes = { "Calendars.ReadWrite", " offline_access", "User.Read" };

        public async Task<TokenResponse> ExchangeCodeForTokenMicrosoftAsync(string code, string redirectUri, string scopes)
        {
            try
            {
                var tokenEndpoint = $"https://login.microsoftonline.com/common/oauth2/v2.0/token";

                var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"grant_type", "authorization_code"},
                    {"code", code},
                    {"redirect_uri", redirectUri},
                    {"client_id", ConfigValues.MicrosoftClientId},
                    {"client_secret", ConfigValues.MicrosoftClientSecret},
                    {"scope", scopes }
                });

                HttpResponseMessage response = await _httpClient.PostAsync(tokenEndpoint, requestContent);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                string jsonContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonContent);
                return tokenResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<TokenResponse> RefreshAccessTokenMicrosoftAsync(string refreshToken)
        {
            try
            {
                var tokenEndpoint = $"https://login.microsoftonline.com/common/oauth2/v2.0/token";

                var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"grant_type", "refresh_token"},
                    {"client_id", ConfigValues.MicrosoftClientId},
                    {"client_secret", ConfigValues.MicrosoftClientSecret},
                    {"refresh_token", refreshToken },
                });

                HttpResponseMessage response = await _httpClient.PostAsync(tokenEndpoint, requestContent);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                string jsonContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonContent);
                return tokenResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<MicrosoftUserInfo> GetMicrosoftUserInfo(string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync("https://graph.microsoft.com/v1.0/me");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<MicrosoftUserInfo>(json);
                }
            }

            return null;
        }

        #endregion
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string id_token { get; set; }
    }

    public class GoogleUserInfo
    {
        public string id { get; set; }
        public string sub { get; set; }
        public string email { get; set; }
        public bool verified_email { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string picture { get; set; }
    }
    public class MicrosoftUserInfo
    {
        public string userPrincipalName { get; set; }  // email
        public string id { get; set; }
        public string displayName { get; set; }
        public string givenName { get; set; }
        public string surname { get; set; }
        public string preferredLanguage  { get; set; }
        public string mail { get; set; }
        public string mobilePhone { get; set; }
        public string jobTitle { get; set; }
        public string officeLocation { get; set; }
    }
}