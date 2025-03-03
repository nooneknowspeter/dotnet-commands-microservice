using CommandsService.AsyncDataServices;
using CommandsService.Data;
using CommandsService.EventProcessing;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// environment variables pre-build
var isDevelopmentSource = builder.Environment.IsDevelopment();
var isProductionSource = builder.Environment.IsProduction();

// determine environment
var isProduction = false;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configure Db context
builder.Services.AddDbContext<AppDbContext>(option => option.UseInMemoryDatabase("InMemory"));

builder.Services.AddScoped<ICommandRepo, CommandRepo>();

// configure AutoMapper for dependency injection
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// add controllers
builder.Services.AddControllers();

// added singleton for event processing
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

// add background service and subscribe to message bus
builder.Services.AddHostedService<MessageBusSubscriber>();


if (isProductionSource)
{
  Console.WriteLine("Setting Up Application for Proudction");

  // add platform data client during application scope lifetime
  // 
  //-------------------------------------------------------------------------------------
  builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();

  // set production to true
  // builds application for production
  // 
  //-------------------------------------------------------------------------------------
  isProduction = true;
}
else
{
  Console.WriteLine("Setting Up Application for Development");

  // set production to false
  // builds application for development
  // 
  //-------------------------------------------------------------------------------------
  isProduction = false;
}

var app = builder.Build();

// setup PrepDb class to apply migration during production
if (isProduction)
{
  PrepDb.PrepPopulation(app);
}

// setup CORS
if (isProduction)
{
  app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithOrigins("http://frontend-clusterip-service:3000"));
}
else
{
  app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithOrigins("http://localhost:5173"));
}


var config = app.Configuration;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// map controllers
app.MapControllers();

var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
};

app.MapGet(
        "/weatherforecast",
        () =>
        {
          var forecast = Enumerable
              .Range(1, 5)
              .Select(index => new WeatherForecast(
                  DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                  Random.Shared.Next(-20, 55),
                  summaries[Random.Shared.Next(summaries.Length)]
              ))
              .ToArray();
          return forecast;
        }
    )
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
