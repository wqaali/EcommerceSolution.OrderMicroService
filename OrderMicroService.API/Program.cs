using OrdersMicroservice.API.Middleware;
using OrderMicroService.BLL;
using OrderMicroService.DAL;
using FluentValidation.AspNetCore;
using OrderMicroService.BLL.HttpClients;
using Polly;
using OrderMicroService.BLL.Policies;

namespace OrderMicroService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Add DAL and BLL services
            builder.Services.AddDataAccessLayer(builder.Configuration);
            builder.Services.AddBusinessLogicLayer(builder.Configuration);
            builder.Services.AddControllers();


            //FluentValidations
            builder.Services.AddFluentValidationAutoValidation();

            //Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Cors
            builder.Services.AddCors(options => {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
            builder.Services.AddTransient<IUserServicePolicies, UserServicePolicies>();
            builder.Services.AddTransient<IProductsMicroservicePolicies, ProductsMicroservicePolicies>();
            builder.Services.AddTransient<IPollyPolicies, PollyPolicies>();

            builder.Services.AddHttpClient<UsersMicroserviceClient>(client =>
            {
                client.BaseAddress = new Uri($"http://{builder.Configuration["UsersMicroserviceName"]}:{builder.Configuration["UsersMicroservicePort"]}");
            })
              .AddPolicyHandler(
              builder.Services.BuildServiceProvider().GetRequiredService<IUserServicePolicies>().GetCombinedPolicy()
              );

            builder.Services.AddHttpClient<ProductsMicroserviceClient>(client =>
            {
                client.BaseAddress = new Uri($"http://{builder.Configuration["ProductsMicroserviceName"]}:{builder.Configuration["ProductsMicroservicePort"]}");
            }).AddPolicyHandler(
               builder.Services.BuildServiceProvider().GetRequiredService<IProductsMicroservicePolicies>().GetFallbackPolicy()).AddPolicyHandler(
               builder.Services.BuildServiceProvider().GetRequiredService<IProductsMicroservicePolicies>().GetBulkheadIsolationPolicy())

  ; ;
            var app = builder.Build();
            app.UseExceptionHandlingMiddleware();
            app.UseRouting();

            //Cors
            app.UseCors();

            //Swagger
            app.UseSwagger();
            app.UseSwaggerUI();

            //Auth
            //app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            //Endpoints
            app.MapControllers();
            app.Run();
        }
    }
}
