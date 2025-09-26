using OrderMicroService.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Polly.Bulkhead;
using Microsoft.Extensions.Logging;
using OrderMicroService.BLL.Enums;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Net;

namespace OrderMicroService.BLL.HttpClients;

public class ProductsMicroserviceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductsMicroserviceClient> _logger;
    private readonly IDistributedCache _distributedCache;

    public ProductsMicroserviceClient(HttpClient httpClient, ILogger<ProductsMicroserviceClient> logger, IDistributedCache distributedCache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _distributedCache = distributedCache;
    }
    public async Task<ProductDTO?> GetProductByProductID(Guid productID)
    {
        try
        {
            string cacheKey = $"Product:{productID}";
            string? cacheProduct = await _distributedCache.GetStringAsync(cacheKey);
            if (cacheProduct != null)
            {
                ProductDTO? productFromCache = JsonSerializer.Deserialize<ProductDTO>(cacheProduct);
                return productFromCache;
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"/ecommerce/products/search/product-id/{productID}");
            //We have implemented the policies if anything goes wrong then polly fllback policy will run but after implementing 
            //Cache it is conflicting because error was also being saved in cache below will not let it save to cache
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                ProductDTO? fallBackResponse = await response.Content.ReadFromJsonAsync<ProductDTO>();

                if (fallBackResponse == null)
                {
                    throw new NotImplementedException("Fallback policy was not implement");
                }
                return fallBackResponse;
            }
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
                    throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");
                }
            }


            ProductDTO? product = await response.Content.ReadFromJsonAsync<ProductDTO>();

            if (product == null)
            {
                throw new ArgumentException("Invalid Product ID");
            }
            string productJson = JsonSerializer.Serialize(product);
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(300)).SetSlidingExpiration(TimeSpan.FromSeconds(100));
            string CachetoWrite = $"Product:{productID}";
            await _distributedCache.SetStringAsync(CachetoWrite, productJson, options);

            return product;
        }
        catch (BulkheadRejectedException ex)
        {
            _logger.LogError(ex, "Bulkhead isolation blocks the request since the request queue is full");

            return new ProductDTO(
              ProductID: Guid.NewGuid(),
              ProductName: "Temporarily Unavailable (Bulkhead)",
              Category: CategoryOptions.Accessories,
              UnitPrice: 0,
              QuantityInStock: 0);
        }
    }
}
