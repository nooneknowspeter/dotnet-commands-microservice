using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
  // different route for now to prevent clashing with
  // PlatformsController from the PlatformsService
  [Route("/api/commands/[controller]")]
  [ApiController]
  public class PlatformsController : ControllerBase
  {
    private readonly ICommandRepo _repository;
    private readonly IMapper _mapper;

    public PlatformsController(ICommandRepo repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
      Console.WriteLine("Getting Platform from CommandsService");

      var platformItems = _repository.GetAllPlatforms();

      return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    }

    [HttpPost]
    public ActionResult TestInboundConnection()
    {
      Console.WriteLine("Inbound POST # Commands Service");

      return Ok("Inbound Succesful");
    }
  }
}
