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

    public async Task<IActionResult> Index(int id)
    {
        var server = await GetServer(id);
        return View(server);
    }

    [HttpGet]
    public async Task<IActionResult> Files(int id)
    {
        ViewBag.Server = await GetServer(id);
        List<FileModel> Files = new List<FileModel>();
        Files = await GetFilesById(id);
        
        return View(Files);
    }

    [HttpPost]
    public async Task<IActionResult> Files(int id, string path)
    {
        ViewBag.Server = await GetServer(id);
        List<FileModel> Files;

        if (await TestIfDir(path))
            Files = await GetFilesByPath(path);
        else
            Files = await GetFilesById(id);
        
        return View(Files);
    }

    [HttpGet]
    public async Task<IActionResult> Config(int id)
    {
        var Server = await GetServer(id);
        ViewBag.Server = Server;

        var Config = System.IO.File.ReadAllText($"servers/{id}/{Server.ConfigPath}");
        if (Config is null)
            Config = "";
        return View("Config", Config);
    }

    [HttpPost]
    public async Task<IActionResult> Config(int id, string config)
    {
        var Server = await GetServer(id);
        ViewBag.Server = Server;

        System.IO.File.WriteAllText($"servers/{id}/{Server.ConfigPath}", config);

        return RedirectToAction("Config");
    }

    private async Task<ServerModel> GetServer(int id)
    {
        var serverConfig = System.IO.File.ReadAllText(@"servers.json");
        return JsonConvert.DeserializeObject<List<ServerModel>>(serverConfig).ElementAt(id);
    }

    private async Task<List<FileModel>> GetFilesById(int id)
    {
        List<FileModel> files = new List<FileModel>();
        List<string> fileNames = System.IO.Directory.EnumerateFileSystemEntries($"servers/{id}/").ToList<string>();
        foreach(string fileName in fileNames)
        {
            if (await TestIfDir(fileName))
                files.Add(new FileModel() { Path = fileName, IsDir = true });
            else
                files.Add(new FileModel() { Path = fileName, IsDir = false });
        }
        return files;
    }

    private async Task<List<FileModel>> GetFilesByPath(string path)
    {
        List<FileModel> files = new List<FileModel>();
        List<string> fileNames = System.IO.Directory.EnumerateFileSystemEntries(path).ToList<string>();
        foreach(string fileName in fileNames)
        {
            if (await TestIfDir(fileName))
                files.Add(new FileModel() { Path = fileName, IsDir = true });
            else
                files.Add(new FileModel() { Path = fileName, IsDir = false });
        }
        return files;
    }

    private async Task<bool> TestIfDir(string path)
    {
        System.IO.FileAttributes attr = System.IO.File.GetAttributes(path);
        if ((attr & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory)
            return true;
        else
            return false;
    }
}