using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Kaolinite.Models;

namespace Kaolinite.Controllers;

public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(ILogger<DashboardController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        return View();
    }

    public async Task<IActionResult> Servers()
    {
        var servers = await GetServers();
        return View(servers);
    }

    private async Task<List<ServerModel>> GetServers()
    {
        var serverConfig = System.IO.File.ReadAllText(@"servers.json");
        return JsonConvert.DeserializeObject<List<ServerModel>>(serverConfig);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
