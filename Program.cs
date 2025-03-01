using CommandsService.AsyncDataServices;
using CommandsService.Data;
using CommandsService.EventProcessing;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

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
