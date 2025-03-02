using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc
{
  public class PlatformDataClient : IPlatformDataClient
  {
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public PlatformDataClient(IConfiguration configuration, IMapper mapper)
    {
      _configuration = configuration;
      _mapper = mapper;
    }

    public IEnumerable<Platform> ReturnAllPlatforms()
    {
      var endpoint = _configuration["GrpcPlatform"];


      Console.WriteLine($"--> Calling gRPC Service {endpoint}");

      var channel = GrpcChannel.ForAddress(endpoint);
      var client = new GrpcPlatform.GrpcPlatformClient(channel);
      var request = new GetAllRequest();

      try
      {
        var reply = client.GetAllPlatforms(request);

        Console.WriteLine($"S --> Successfully Called gRPC Server and Pulled Platforms from Server");

        return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
      }
      catch (Exception exception)
      {
        Console.WriteLine($"X --> Failed to Call gRPC Server: {exception.Message}");

        return null;
      }
    }
  }
}
