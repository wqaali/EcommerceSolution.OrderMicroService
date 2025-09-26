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
        string connectionStringTemplate = configuration.GetConnectionString("MongoDB")!;
        string connectionString = connectionStringTemplate
          .Replace("$MONGODB_HOST", Environment.GetEnvironmentVariable("MONGODB_HOST"))
          .Replace("$MONGODB_PORT", Environment.GetEnvironmentVariable("MONGODB_PORT"));

        services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
        services.AddScoped<IMongoDatabase>(provider=>
        {
            IMongoClient client=provider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(Environment.GetEnvironmentVariable("MONGODB_DATABASE"));
        });

        services.AddScoped<IOrdersRepository, OrdersRepository>();
        return services;
        }
    }

