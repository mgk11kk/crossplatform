using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class Auth0TokenService
{
    private readonly HttpClient _httpClient;
    private readonly string _domain;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public Auth0TokenService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _domain = configuration["Auth0:Domain"];
        _clientId = configuration["Auth0:ClientId"];
        _clientSecret = configuration["Auth0:ClientSecret"];
    }

    public async Task<string> GetManagementApiTokenAsync()
    {
        var requestBody = new
        {
            client_id = _clientId,
            client_secret = _clientSecret,
            audience = $"https://{_domain}/api/v2/",
            grant_type = "client_credentials"
        };

        var response = await _httpClient.PostAsync(
            $"https://{_domain}/oauth/token",
            new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<dynamic>(content);
        
        // Management API токен
        return tokenResponse.access_token; 
    }
}