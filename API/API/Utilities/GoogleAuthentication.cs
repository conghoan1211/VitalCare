using API.Configurations;
using API.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace API.Utilities
{
    public static class GoogleAuthentication
    {
        #region Các thuộc tính mặc định
        private const string AuthUrl = "https://accounts.google.com/o/oauth2/auth";

        private const string Scope = "email profile openid";

        private const string UserInfoUrl = "https://www.googleapis.com/oauth2/v1/userinfo";

        private const string TokenUrl = "https://oauth2.googleapis.com/token";
        #endregion

        private static string RedirectUrl = ConfigManager.gI().GoogleRedirectUri;
        private static readonly string ClientID = ConfigManager.gI().GoogleClientIp;
        private static readonly string ClientSecret = ConfigManager.gI().GoogleClientSecert;

        public static string GetRedirectUri(HttpContext httpContext)
        {
            var request = httpContext.Request;
            return $"{request.Scheme}://{request.Host}{RedirectUrl}";
        }

        public static async Task<AuthTokenResponse> GetAuthAccessTokenAsync(string? code, HttpContext httpContext)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    throw new ArgumentException("Authorization code is missing!", nameof(code));

                if (string.IsNullOrEmpty(ClientID))
                    throw new InvalidOperationException("Client ID is missing!");
                if (string.IsNullOrEmpty(ClientSecret))
                    throw new InvalidOperationException("Client Secret is missing!");
                if (string.IsNullOrEmpty(RedirectUrl))
                    throw new InvalidOperationException("Redirect URI is missing!");

                var redirectUri = GetRedirectUri(httpContext);
                Console.WriteLine($"Redirect URI: {redirectUri}");
                Dictionary<string, string> parameters = new()
                {
                    ["grant_type"] = "authorization_code",
                    ["code"] = code,
                    ["client_id"] = ClientID,
                    ["client_secret"] = ClientSecret,
                    ["redirect_uri"] = redirectUri
                };

                HttpRequestMessage req = new(HttpMethod.Post, TokenUrl)
                {
                    Content = new FormUrlEncodedContent(parameters)
                };
                HttpClient client = new();
                HttpResponseMessage res = await client.SendAsync(req);
                if (HttpStatusCode.OK != res.StatusCode)
                {
                    throw new Exception($"Failed to get access token res: {res.ReasonPhrase}");
                }

                return AuthTokenResponse.FromJSON(await res.Content.ReadAsStringAsync());
                //using var httpClient = new HttpClient();
                //var content = new FormUrlEncodedContent(parameters);

                //// Log request details
                //Console.WriteLine($">>>>>>>> Request URL: {TokenUrl}");
                //Console.WriteLine($">>>>>>>> Request Parameters: {JsonConvert.SerializeObject(parameters)}");

                //var response = await httpClient.PostAsync(TokenUrl, content);
                //var responseContent = await response.Content.ReadAsStringAsync();

                //// Log response
                //Console.WriteLine($">>>>>>>> Response Status: {response.StatusCode}");
                //Console.WriteLine($">>>>>>>> Response Content: {responseContent}");

                //if (!response.IsSuccessStatusCode)
                //{
                //    throw new Exception($"Token request failed: {response.StatusCode} - {responseContent}");
                //}

                //return AuthTokenResponse.FromJSON(responseContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>>>>>>> Error in GetAuthAccessTokenAsync: {ex.Message}");
                Console.WriteLine($">>>>>>>> StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public static async Task<GoogleUserInfo> GetUserInfoAsync(string? accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                throw new Exception("Access Token is missing !");

            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = await httpClient.GetAsync(UserInfoUrl);

            if (!response.IsSuccessStatusCode)
            {
                string errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to fetch user info. Response: {errorResponse}");
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GoogleUserInfo>(responseContent);
        }
    }
}
