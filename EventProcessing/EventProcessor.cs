using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
  public class EventProcessor : IEventProcessor
  {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
      _scopeFactory = scopeFactory;
      _mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
      var eventType = DetermineEvent(message);

      switch (eventType)
      {
        case EventType.PlatformPublished:
          addPlatform(message);
          break;
        default:
          break;
      }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
      Console.WriteLine("--> Determining Event");

      var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

      switch (eventType.Event)
      {
        case "Platform_Published":
          Console.WriteLine("Platform Published Event Detected");
          return EventType.PlatformPublished;
        default:
          Console.WriteLine("--> Could Not Determine Event Type");
          return EventType.Undetermined;
      }
    }

    private void addPlatform(string platformPublishedMessage)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

        var PlatformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(
            platformPublishedMessage
        );

        try
        {
          var platform = _mapper.Map<Platform>(PlatformPublishedDto);

          if (!repo.ExternalPlatformExists(platform.ExternalID))
          {
            repo.CreatePlatform(platform);
            repo.saveChanges();
            Console.WriteLine("S --> Platform Created");
          }
          else
          {
            Console.WriteLine("X --> Platform Already Exists");
          }
        }
        catch (Exception exception)
        {
          Console.WriteLine(
              $"X --> Could Not Add Platform To Database {exception.Message}"
          );
        }
      }
    }
  }

  enum EventType
  {
    PlatformPublished,
    Undetermined,
  }
}
