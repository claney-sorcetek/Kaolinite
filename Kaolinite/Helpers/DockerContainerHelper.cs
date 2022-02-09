using System.IO;
using Ductus.FluentDocker.Model.Containers;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Extensions;
using Ductus.FluentDocker.Services;

namespace Kaolinite.Helpers;

public static class DockerContainerHelper
{
    public static async void StartContainerWithId(int id)
    {
        var server = await ServerHelper.GetServer(id);

        if (Globals.containers.Where(c => c.Name == server.Title).Count() <= 0 )
            return;

        string dockerfile = await GetDockerFileById(id);
        var containerBuilder = new Builder();

        var container = containerBuilder
            .DefineImage($"kaolinite/{server.Title}").ReuseIfAlreadyExists()
            .FromFile(dockerfile)
            .Maintainer("Zombie").Builder()

            .UseContainer()
            .WithName(server.Title)
            .UseImage($"kaolinite/{server.Title}")
            .ExposePort(server.Port)
            .Mount($"server/{id}/", "/home/game/", Ductus.FluentDocker.Model.Builders.MountType.ReadWrite)
            .Build()
            .Start();

        Globals.containers.Add(container);
    }

    public static async void StopContainerWithId(int id)
    {
        var server = await ServerHelper.GetServer(id);
        var container = Globals.containers.Where(c => c.Name == server.Title).First();
        
        if (container is null)
            return;

        container.Stop();
    }

    public static async void SendCommandToContainerWithId(int id)
    {
        var server = await ServerHelper.GetServer(id);
        var container = Globals.containers.Where(c => c.Name == server.Title).First();
        
        if (container is null)
            return;

        container.
    }

    private static async Task<string> GetDockerFileById(int id)
    {
        var server = await ServerHelper.GetServer(id);
        string dockerfile = await File.ReadAllTextAsync($"dockerfiles/{id}/{server.DockerFile}");

        return dockerfile;
    }
}
