using Microsoft.EntityFrameworkCore;
using ECommerceApi.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure MySQL
var connectionString = $"Server={builder.Configuration["MYSQL_HOST"]};" +
                      $"Port={builder.Configuration["MYSQL_PORT"]};" +
                      $"Database={builder.Configuration["MYSQL_DATABASE"]};" +
                      $"User={builder.Configuration["MYSQL_USER"]};" +
                      $"Password={builder.Configuration["MYSQL_PASSWORD"]};";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "E-Commerce API",
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

app.Run();