using CommandsService.Models;
using System.Collections.Generic;

namespace CommandsService.Data
{
  public interface ICommandRepo
  {
    bool saveChanges();

    // platforms
    IEnumerable<Platform> GetAllPlatforms();
    void CreatePlatform(Platform plat);
    bool PlatformExits(int platformId);
    bool ExternalPlatformExists(int externalPlatformId);

    // commands
    IEnumerable<Command> GetCommandsForPlatform(int platformId);
    Command GetCommand(int platformId, int commandId);
    void CreateCommand(int platformId, Command command);
  }
}
