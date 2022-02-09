using Kaolinite.Models;
using Newtonsoft.Json;
using System.IO;

namespace Kaolinite.Helpers;

public static class ServerHelper
{
    public static async Task<ServerModel> GetServer(int id)
    {
        var serverConfig = File.ReadAllText(@"servers.json");
        return JsonConvert.DeserializeObject<List<ServerModel>>(serverConfig).ElementAt(id);
    }

    public static async Task<List<ServerModel>> GetServers()
    {
        var serverConfig = File.ReadAllText(@"servers.json");
        return JsonConvert.DeserializeObject<List<ServerModel>>(serverConfig);
    }
}
