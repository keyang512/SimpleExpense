using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData;
using SimpleExpenseManagement.Server.Data;

namespace SimpleExpenseManagement.Server
{
    /// <summary>
    /// Configuration class for Web API services and middleware
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Configures the services for the Web API
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration instance</param>
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add controllers with OData support
            services.AddControllers()
                .AddOData(options =>
                {
                    options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100);
                });

            // Add API explorer for Swagger
            services.AddEndpointsApiExplorer();

            // Configure Swagger/OpenAPI
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Simple Expense Management API",
                    Version = "v1",
                    Description = "A simple API for managing personal expenses with OData support",
                    Contact = new OpenApiContact
                    {
                        Name = "API Support",
                        Email = "support@example.com"
                    }
                });
            });

            // Configure Entity Framework
            ConfigureEntityFramework(services, configuration);

            // Add CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
        }

        /// <summary>
        /// Configures the Entity Framework services
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration instance</param>
        private static void ConfigureEntityFramework(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ExpenseDbContext>(options =>
            {
                // Use SQL Server for the database
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    // Fallback connection strings for different SQL Server configurations
                    var possibleConnections = new[]
                    {
                        "Server=.\\SQLEXPRESS;Database=SimpleExpenseManagement;Trusted_Connection=true;TrustServerCertificate=true;Encrypt=false;MultipleActiveResultSets=true",
                        "Server=(localdb)\\mssqllocaldb;Database=SimpleExpenseManagement;Trusted_Connection=true;MultipleActiveResultSets=true",
                        "Server=localhost;Database=SimpleExpenseManagement;Trusted_Connection=true;TrustServerCertificate=true;Encrypt=false;MultipleActiveResultSets=true"
                    };

                    // Try to find a working connection
                    connectionString = possibleConnections[0]; // Default to SQLEXPRESS
                }

                options.UseSqlServer(connectionString);
            });
        }

        /// <summary>
        /// Configures the HTTP request pipeline
        /// </summary>
        /// <param name="app">The web application instance</param>
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            // Configure the HTTP request pipeline
            if (environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Simple Expense Management API V1");
                    c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
                });
            }

            app.UseHttpsRedirection();

            // Use CORS
            app.UseCors("AllowAll");

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("/index.html");
            });
        }
    }
}
