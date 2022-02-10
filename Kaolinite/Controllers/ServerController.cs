using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Kaolinite.Models;
using Kaolinite.Helpers;

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
        ViewBag.Server = await ServerHelper.GetServer(id);

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Start(int id)
    {
        ViewBag.Server = await ServerHelper.GetServer(id);

        if (!await DockerContainerHelper.StartContainerWithId(id))
        {
            ViewBag.Error = "Error occured while starting server!";
        }

        return View("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Stop(int id)
    {
        ViewBag.Server = await ServerHelper.GetServer(id);

        if (!await DockerContainerHelper.StopContainerWithId(id))
        {
            ViewBag.Error = "Error occured while stopping server!";
        }

        return View("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Restart(int id)
    {
        ViewBag.Server = await ServerHelper.GetServer(id);

        bool partOne = await DockerContainerHelper.StartContainerWithId(id);
        bool partTwo = await DockerContainerHelper.StopContainerWithId(id);

        if (!partOne && !partTwo)
        {
            ViewBag.Error = "Error occured while restarting server!";
        }

        return View("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Command(int id, string cmd)
    {
        ViewBag.Server = await ServerHelper.GetServer(id);

        if (!await DockerContainerHelper.SendCommandToContainerWithId(id, cmd))
        {
            ViewBag.Error = "Error occured while sending command!";
        }

        return View("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Files(int id)
    {
        ViewBag.Server = await ServerHelper.GetServer(id);

        List<FileModel> Files = new List<FileModel>();
        Files = await GetFilesById(id);
        
        return View(Files);
    }

    [HttpPost]
    public async Task<IActionResult> Files(int id, string path)
    {
        ViewBag.Server = await ServerHelper.GetServer(id);

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
        var Server = await ServerHelper.GetServer(id);
        
        ViewBag.Server = Server;

        var Config = System.IO.File.ReadAllText($"servers/{id}/{Server.ConfigPath}");
        if (Config is null)
            Config = "";
        return View("Config", Config);
    }

    [HttpPost]
    public async Task<IActionResult> Config(int id, string config)
    {
        var Server = await ServerHelper.GetServer(id);
        ViewBag.Server = Server;

        System.IO.File.WriteAllText($"servers/{id}/{Server.ConfigPath}", config);

        return RedirectToAction("Config");
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