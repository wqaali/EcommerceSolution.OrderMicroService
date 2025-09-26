using Polly;

namespace OrderMicroService.BLL.Policies;

public interface IPollyPolicies
{
  IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount);
  IAsyncPolicy<HttpResponseMessage> CircuiteBreakerPolicy(int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak);
  IAsyncPolicy<HttpResponseMessage> GetTimeOutPolicy(TimeSpan timeout);
}
