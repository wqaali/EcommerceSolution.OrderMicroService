using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OrderMicroService.BLL.DTO;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderMicroService.BLL.HttpClients;

public class UsersMicroserviceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsersMicroserviceClient> _logger;
    private readonly IDistributedCache _distributedCache;

    public UsersMicroserviceClient(HttpClient httpClient, ILogger<UsersMicroserviceClient> logger,IDistributedCache distributedCache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _distributedCache = distributedCache;
    }



    public async Task<UserDTO?> GetUserByUserID(Guid userID)
    {
        try
        {
            string CacheKey = $"User:{userID}";
            string? CachedUser=await _distributedCache.GetStringAsync(CacheKey);
            if (CachedUser != null)
            {
                UserDTO? userFromCache = JsonSerializer.Deserialize<UserDTO>(CachedUser);
                return userFromCache;
            }

            HttpResponseMessage response = await _httpClient.GetAsync($"/api/users/{userID}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException("Bad request", null, System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    //throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");

                    return new UserDTO(
                      PersonName: "Temporarily Unavailable",
                      Email: "Temporarily Unavailable",
                      Gender: "Temporarily Unavailable",
                      UserID: Guid.Empty);
                }
            }


            UserDTO? user = await response.Content.ReadFromJsonAsync<UserDTO>();

            if (user == null)
            {
                throw new ArgumentException("Invalid User ID");
            }
            string cacheKeyToWrite = $"User:{userID}";
            string userString=JsonSerializer.Serialize(user);
            DistributedCacheEntryOptions options= new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30)).SetSlidingExpiration(TimeSpan.FromSeconds(20));
            await _distributedCache.SetStringAsync(cacheKeyToWrite, userString, options);
            return user;
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "Request failed because of circuit breaker is in Open state. Returning dummy data.");

            return new UserDTO(
                    PersonName: "Temporarily Unavailable",
                    Email: "Temporarily Unavailable",
                    Gender: "Temporarily Unavailable",
                    UserID: Guid.Empty);
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, "Request failed because of timeout. Returning dummy data.");

            return new UserDTO(
                    PersonName: "Temporarily Unavailable",
                    Email: "Temporarily Unavailable",
                    Gender: "Temporarily Unavailable",
                    UserID: Guid.Empty);
        }
    }
}
