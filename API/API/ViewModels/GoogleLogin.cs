using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.ViewModels
{
    public class GoogleAuthRequest
    {
        [Required]
        public string? Credential { get; set; }
    }

    public class AuthTokenResponse
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonProperty("scope")]
        public string? Scope { get; set; }

        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; } // Thời gian hết hạn (giây)

        public static AuthTokenResponse FromJSON(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<AuthTokenResponse>(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse token response: {ex.Message}");
            }
        }
    }
    public class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("id_token")]
        public string? IdToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }
    }

    public class GoogleUserInfo
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("verified_email")]
        public bool VerifiedEmail { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("picture")]
        public string? Picture { get; set; }

        public string Gender { get; set; }
        public string BirthDate { get; set; }
        public string FamilyName { get; set; }
        public string Locale { get; set; }
        public string Hd { get; set; }
        public string ProfileLink { get; set; }
        public string TimeZone { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string OccupationName { get; set; }
        public string OrganizationName { get; set; }
        public string AgeRange { get; set; }
        public string CoverPhoto { get; set; }
        public string TagLine { get; set; }
    }
}
