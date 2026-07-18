using ApiMockServer.Data;
using Microsoft.Extensions.Options;
using ApiMockServer.Repositories;
using ApiMockServer.Interfaces;
using ApiMockServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDbSettings from appsettings.json
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<MongoDbContext>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoDbContext(settings);
});

// Register the MockEndpointRepository with the DI container
builder.Services.AddScoped<IMockEndpointRepository, MockEndpointRepository>();
// Register the MockEndpointService with the DI container
builder.Services.AddScoped<IMockEndpointService, MockEndpointService>();

// Register the CollectionRepository with the DI container
builder.Services.AddScoped<ICollectionRepository, CollectionRepository>();
// Register the CollectionService with the DI container
builder.Services.AddScoped<ICollectionService, CollectionService>();

// Register the EnvironmentRepository with the DI container
builder.Services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
// Register the EnvironmentService with the DI container
builder.Services.AddScoped<IEnvironmentService, EnvironmentService>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();