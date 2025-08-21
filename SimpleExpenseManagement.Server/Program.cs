using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using Microsoft.EntityFrameworkCore;
using SimpleExpenseManagement.Server;
using SimpleExpenseManagement.Server.Models;
using SimpleExpenseManagement.Server.Data;

var builder = WebApplication.CreateBuilder(args);

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<ExpenseDto>("Expenses");
modelBuilder.EntitySet<PersonDto>("People");
modelBuilder.EntitySet<CategoryDto>("Categories");

builder.Services.AddControllers().AddOData(
    options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents(
        "api",
        modelBuilder.GetEdmModel()));

// Configure services using WebApiConfig
WebApiConfig.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ExpenseDbContext>();
    try
    {
        // Ensure database is created
        context.Database.EnsureCreated();
        Console.WriteLine("Database initialized successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization failed: {ex.Message}");
        Console.WriteLine("Please check your SQL Server connection and ensure the server is running.");
    }
}

// Configure the HTTP request pipeline using WebApiConfig
WebApiConfig.Configure(app, app.Environment);

app.Run();
