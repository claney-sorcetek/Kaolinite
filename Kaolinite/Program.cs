using System.Text.Json;
using Kaolinite.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Dashboard/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (!File.Exists("./servers.json"))
{
    File.Create("./servers.json").Close();
    var serverList = new List<ServerModel>(){ new ServerModel(){ Id = 0, Title = "ChangeMe", Memory = 2048 } };
    File.WriteAllText("./servers.json", System.Text.Json.JsonSerializer.Serialize<List<ServerModel>>(serverList, new JsonSerializerOptions(){ WriteIndented = true }));
}

if (!Directory.Exists("./servers"))
{
    Directory.CreateDirectory("./servers");
}

foreach (var server in JsonConvert.DeserializeObject<List<ServerModel>>(File.ReadAllText(@"servers.json")))
{
    if (!Directory.Exists($"./servers/{server.Id}"))
    {
        Directory.CreateDirectory($"./servers/{server.Id}");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    "Default",
    "/",
    new { controller = "Dashboard", action = "Index"}
);

app.MapControllerRoute(
    "Servers",
    "/servers",
    new { controller = "Dashboard", action = "Servers"}
);

app.MapControllerRoute(
    "Server",
    "/server/{id}",
    new { controller = "Server", action = "Index"}
);

app.MapControllerRoute(
    "Server",
    "/server/files/{id}",
    new { controller = "Server", action = "Files"}
);

app.MapControllerRoute(
    "Server",
    "/server/backup/{id}",
    new { controller = "Server", action = "Backup"}
);

app.MapControllerRoute(
    "Server",
    "/server/config/{id}",
    new { controller = "Server", action = "Config"}
);

app.Run();
