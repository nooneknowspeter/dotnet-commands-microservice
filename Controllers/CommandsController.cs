using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
  [Route("api/commands/platforms/{platformId}/[controller]")]
  [ApiController]
  public class CommandsController : ControllerBase
  {
    private readonly ICommandRepo _repository;
    private readonly IMapper _mapper;

    public CommandsController(ICommandRepo repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }


    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
      Console.WriteLine($"Hit GetCommandsForPlatform: {platformId}");

      if (!_repository.PlatformExits(platformId))
      {
        return NotFound();
      }

      var commands = _repository.GetCommandsForPlatform(platformId);

      return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }
  }
}
