using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Message.Domain.Entities;
using Po.Api.Response;

namespace Message.Infrastructure.Services;

public class AuthClient 
{
    private readonly HttpClient _httpClient;

    public AuthClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    /// <summary>
    /// 取得使用者資訊
    /// </summary>
    /// <param name="token">Json Web Token</param>
    /// <returns></returns>
    public async Task<User> GetAsync(string token)
    {   
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/oauth/information")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) },
        };
        var response = await _httpClient.SendAsync(request);
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var user = await response.Content.ReadFromJsonAsync<User>();
            if(user == null)
                throw Failure.Unauthorized();
            return user;
        }
        else
        {
            throw Failure.Unauthorized();
        }
    }
}