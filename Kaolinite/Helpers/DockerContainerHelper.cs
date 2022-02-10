using System.IO;
using System.Net;
using CoreRCON;
using CoreRCON.Parsers.Standard;
using Ductus.FluentDocker.Builders;

namespace Kaolinite.Helpers;

public static class DockerContainerHelper
{
    public static async Task<bool> StartContainerWithId(int id)
    {
        try 
        {
            var server = await ServerHelper.GetServer(id);

            if (await TestIfContainerStarted(id))
                return false;

            string dockerfile = await GetDockerFileById(id);
            var containerBuilder = new Builder();

            var container = containerBuilder
                .DefineImage($"kaolinite/{server.Title.ToLower()}").ReuseIfAlreadyExists()
                .FromFile(dockerfile)
                .Maintainer("Zombie").Builder()

                .UseContainer()
                .WithName(server.Title)
                .UseImage($"kaolinite/{server.Title}")
                .ExposePort(server.Port)
                .Mount($"server/{id}/", "/home/game/", Ductus.FluentDocker.Model.Builders.MountType.ReadWrite)
                .Build()
                .Start();

            Globals.containers.Add(server.Title, container);

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public static async Task<bool> StopContainerWithId(int id)
    {
        try 
        {
            var server = await ServerHelper.GetServer(id);
            var container = Globals.containers[server.Title];
            
            if (await TestIfContainerStarted(id))
                return false;

            Globals.containers.Remove(server.Title);
            container.Stop();

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public static async Task<bool> SendCommandToContainerWithId(int id, string cmd)
    {
        try 
        {
            var server = await ServerHelper.GetServer(id);
            var container = Globals.containers[server.Title];
            
            if (await TestIfContainerStarted(id))
                return false;

            var rcon = new RCON(IPAddress.Parse("127.0.0.1"), (ushort) server.Port, server.RconPassword);
            await rcon.SendCommandAsync(cmd);
            rcon.Dispose();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    private static async Task<string> GetDockerFileById(int id)
    {
        var server = await ServerHelper.GetServer(id);
        string dockerfile = await File.ReadAllTextAsync($"dockerfiles/{server.DockerFile}");

        return dockerfile;
    }

    private static async Task<bool> TestIfContainerStarted(int id)
    {
        var server = await ServerHelper.GetServer(id);
        if (Globals.containers.ContainsKey(server.Title))
            return true;
        else
            return false;
    }
}
