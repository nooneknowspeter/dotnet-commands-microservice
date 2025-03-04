using System;
using System.Collections.Generic;
using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.Data
{
  public static class PrepDb
  {
    public static void PrepPopulation(IApplicationBuilder applicationBuilder)
    {
      using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
      {
        var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

        var platforms = grpcClient.ReturnAllPlatforms();

        SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
      }
    }

    private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
    {
      Console.WriteLine("--> Seeding New Platforms");

      foreach (var platform in platforms)
      {
        if (!repo.ExternalPlatformExists(platform.ExternalID))
        {
          repo.CreatePlatform(platform);
        }
        repo.saveChanges();
      }
    }
  }
}
