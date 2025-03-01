using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;

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
    }
  }
}
