using OrdersMicroservice.API.Middleware;
using OrderMicroService.BLL;
using OrderMicroService.DAL;
using FluentValidation.AspNetCore;

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
            var app = builder.Build();
            app.UseExceptionHandlingMiddleware();
            app.UseRouting();

            //Cors
            app.UseCors();

            //Swagger
            app.UseSwagger();
            app.UseSwaggerUI();

            //Auth
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            //Endpoints
            app.MapControllers();
            app.Run();
        }
    }
}
