using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;
using SharpCompress.Writers.Zip;
using Kaolinite.Models;

namespace Kaolinite.Controllers;

public class BackupController : Controller
{
    private readonly ILogger<BackupController> _logger;

    public BackupController(ILogger<BackupController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Index(int id)
    {
        ViewBag.Server = await GetServer(id);
        List<FileModel> Files = await GetFilesByPath($"backups/{id}");
        return View(Files);
    }

    public async Task<IActionResult> Create(int id)
    {
        ViewBag.Server = await GetServer(id);
        List<FileModel> Files = await GetFilesByPath($"backups/{id}/");

        var archive = ZipArchive.Create();
        archive.AddAllFromDirectory($"servers/{id}/");
        DateTime now = DateTime.Now;
        string name = $"Backup-{now.Month}-{now.Day}-{now.Year}-{now.Hour}-{now.Minute}-{now.Second}";
        archive.SaveTo($"backups/{id}/backup{name}.zip", CompressionType.Deflate);
        archive.Dispose();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Apply(int id, string path)
    {
        ViewBag.Server = await GetServer(id);
        var di = new System.IO.DirectoryInfo($"servers/{id}/");

        foreach (var dir in di.GetDirectories())
        {
            foreach (var fl in dir.GetFiles())
            {
                fl.Delete();
            }
            dir.Delete();
        }

        var archive = ZipArchive.Open(path);
        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
        {
            entry.WriteToDirectory($"servers/{id}/", new ExtractionOptions()
            {
                ExtractFullPath = true,
                Overwrite = true
            });
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id, string path)
    {
        ViewBag.Server = await GetServer(id);
        
        System.IO.File.Delete(path);

        return RedirectToAction("Index");
    }

    private async Task<ServerModel> GetServer(int id)
    {
        var serverConfig = System.IO.File.ReadAllText(@"servers.json");
        return JsonConvert.DeserializeObject<List<ServerModel>>(serverConfig).ElementAt(id);
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