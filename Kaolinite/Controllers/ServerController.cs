using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Kaolinite.Models;

namespace Kaolinite.Controllers;

public class ServerController : Controller
{
    private readonly ILogger<ServerController> _logger;

    public ServerController(ILogger<ServerController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int id)
    {
        var server = await GetServer(id);
        return View(server);
    }

    [HttpGet]
    public async Task<IActionResult> Files(int id)
    {
        ViewBag.Server = await GetServer(id);
        ViewBag.Files = await GetFiles(id);
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Files(int id, string path)
    {
        ViewBag.Server = await GetServer(id);
        ViewBag.Files = System.IO.Directory.EnumerateFileSystemEntries(path);
        
        return View("Files");
    }

    [HttpGet]
    public async Task<IActionResult> Backup(int id)
    {
        var server = await GetServer(id);
        return View(server);
    }

    [HttpGet]
    public async Task<IActionResult> Config(int id)
    {
        var server = await GetServer(id);
        return View(server);
    }

    private async Task<ServerModel> GetServer(int id)
    {
        var serverConfig = System.IO.File.ReadAllText(@"servers.json");
        return JsonConvert.DeserializeObject<List<ServerModel>>(serverConfig).ElementAt(id);
    }

    private async Task<List<string>> GetFiles(int id)
    {
        return System.IO.Directory.EnumerateFileSystemEntries($"servers/{id}/").ToList<string>();
    }
}