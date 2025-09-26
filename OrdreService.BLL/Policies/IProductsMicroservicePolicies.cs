using Polly;

namespace OrderMicroService.BLL.Policies;

public interface IProductsMicroservicePolicies
{
    IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy();
    IAsyncPolicy<HttpResponseMessage> GetBulkheadIsolationPolicy();
}
