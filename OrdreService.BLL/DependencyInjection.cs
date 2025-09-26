using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderMicroService.BLL.Mappers;
using OrderMicroService.BLL.ServiceContract;
using OrderMicroService.BLL.Services;
using OrderMicroService.BLL.Validator;


namespace OrderMicroService.BLL
{
    public static class DependencyInjection
    {
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
    {
            //TO DO: Add business logic layer services into the IoC container
            services.AddAutoMapper(typeof(OrderAddRequestToOrderMappingProfile).Assembly);
            services.AddValidatorsFromAssemblyContaining<OrderAddRequestValidator>();
            services.AddScoped<IOrdersService, OrdersService>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = $"{configuration["Redis_HOST"]}" +
                $":{configuration["Redis_PORT"]}";
            });
            return services;
    }
}
}

