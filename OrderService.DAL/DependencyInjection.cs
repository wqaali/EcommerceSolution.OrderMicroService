using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OrderMicroService.DAL.Repository;
using OrderMicroService.DAL.RepositoryContract;

namespace OrderMicroService.DAL;

    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
        var connectionTemplate = configuration.GetConnectionString("mongodb");
        var connectionStringValues = connectionTemplate.Replace("$MONGODB_HOST", Environment.GetEnvironmentVariable("MONGODB_HOST")).
            Replace("$MONGODB_PORT", Environment.GetEnvironmentVariable("MONGODB_PORT"));
        services.AddSingleton<IMongoClient>(new MongoClient(connectionStringValues));
        services.AddScoped<IMongoDatabase>(provider=>
        {
            IMongoClient client=provider.GetRequiredService<IMongoClient>();
            return client.GetDatabase("OrdersDatabase");
        });

        services.AddScoped<IOrdersRepository, OrdersRepository>();
        return services;
        }
    }

