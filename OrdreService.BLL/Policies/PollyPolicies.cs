using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using System.Text;
using System.Text.Json;

namespace OrderMicroService.BLL.Policies;
public class PollyPolicies : IPollyPolicies
{
    private readonly ILogger<UserServicePolicies> _logger;

    public PollyPolicies(ILogger<UserServicePolicies> logger)
    {
        _logger = logger;
    }
    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
    {
        AsyncTimeoutPolicy<HttpResponseMessage> policy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(800));

        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> CircuiteBreakerPolicy(int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak)
    {
        AsyncCircuitBreakerPolicy<HttpResponseMessage> retryPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                 .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3, //Number of retries
                    durationOfBreak: TimeSpan.FromMinutes(1), // Breaker Delay
                    onBreak: (outcome, timespan) =>
                    {
                        _logger.LogInformation($"Circuit Beak open for  :{timespan} Due to consecutive 3 failure");
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation($"Circuite Close for request");
                    });
        return retryPolicy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetTimeOutPolicy(TimeSpan timeout)
    {
        AsyncRetryPolicy<HttpResponseMessage> retryPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
               .WaitAndRetryAsync(
                  retryCount: 5, //Number of retries
                  sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Delay between retries
                  onRetry: (outcome, timespan, retryAttempt, context) =>
                  {
                      _logger.LogInformation($"Retry Number :{retryAttempt} ,Number Of Seconds:{timespan}");
                  });
        return retryPolicy;
    }
}
