using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrderMicroService.BLL.Policies;

public class UserServicePolicies : IUserServicePolicies
{
    private readonly ILogger<UserServicePolicies> _logger;
    private readonly IPollyPolicies _pollyPolicies;
    public UserServicePolicies(ILogger<UserServicePolicies> logger, IPollyPolicies pollyPolicies)
    {
        _logger = logger;
        _pollyPolicies = pollyPolicies;

    }     

    public IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
    {
        var retryPolicy = _pollyPolicies.GetRetryPolicy(2);
        var circuitBreakerPolicy = _pollyPolicies.CircuiteBreakerPolicy(2,TimeSpan.FromHours(1));
        var timeoutPolicy = _pollyPolicies.GetTimeOutPolicy(TimeSpan.FromHours(1));

        AsyncPolicyWrap<HttpResponseMessage> wrappedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);
        return wrappedPolicy;
    }
}
