// Path: Program.cs
using Microsoft.EntityFrameworkCore;
using SPtoDotnet_Canonical.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Text.Json.Serialization; // Added this using directive

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add DbContext configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // Get the connection string

// Ensure connectionString is not null or empty
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString,
        new MySqlServerVersion(new Version(8, 0, 21)), // Configure DbContext to use MySQL. Adjust version if needed.
        mySqlOptions => mySqlOptions.EnableRetryOnFailure())); // Optional: Add retry on failure

builder.Services.AddControllers()
    .AddJsonOptions(options => // Added JSON options configuration
    {
        // Configure JSON serializer to handle object cycles
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
