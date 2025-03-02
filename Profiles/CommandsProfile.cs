using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.Profiles
{
  public class CommandsProfile : Profile
  {
    public CommandsProfile()
    {
      // source --> target
      CreateMap<Platform, PlatformReadDto>();
      CreateMap<CommandCreateDto, Command>();
      CreateMap<Command, CommandReadDto>();
      // maps Id property from PlatformPublished object to ExternalId to Platform object
      CreateMap<PlatformPublishedDto, Platform>()
        .ForMember(destination => destination.ExternalID, option => option.MapFrom(source => source.Id));
      // maps incoming gRPC object from server to Platform object
      CreateMap<GrpcPlatformModel, Platform>()
        .ForMember(destination => destination.ExternalID, option => option.MapFrom(source => source.PlatformId))
        .ForMember(destination => destination.Name, option => option.MapFrom(source => source.Name))
        .ForMember(destination => destination.Commands, option => option.Ignore());
    }
  }
}
