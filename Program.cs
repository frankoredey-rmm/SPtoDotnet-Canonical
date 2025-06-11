using Microsoft.EntityFrameworkCore;
using NewStore.Data;
using NewStore.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Load configuration from mysql_connection.env file
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .AddKeyPerFile("/run/secrets", optional: true)
    .Build();

// Configure MySQL
var connectionString = $"Server={configuration["MYSQL_HOST"]};" +
                      $"Port={configuration["MYSQL_PORT"]};" +
                      $"Database={configuration["MYSQL_DATABASE"]};" +
                      $"User={configuration["MYSQL_USER"]};" +
                      $"Password={configuration["MYSQL_PASSWORD"]};";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString,
        ServerVersion.AutoDetect(connectionString),
        options => options.EnableRetryOnFailure()));

// Register repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "NewStore API",
        Version = "v1",
        Description = "API for managing e-commerce orders"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating the database.");
    }
}

app.Run();