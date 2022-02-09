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
    var serverList = new List<ServerModel>(){ new ServerModel(){ Id = 0, Title = "ChangeMe", Memory = 2048, ConfigPath = "settings.json" } };
    File.WriteAllText("./servers.json", System.Text.Json.JsonSerializer.Serialize<List<ServerModel>>(serverList, new JsonSerializerOptions(){ WriteIndented = true }));
}

if (!Directory.Exists("./servers"))
{
    Directory.CreateDirectory("./servers");
}

if (!Directory.Exists("./backups"))
{
    Directory.CreateDirectory("./backups");
}

foreach (var server in JsonConvert.DeserializeObject<List<ServerModel>>(File.ReadAllText(@"servers.json")))
{
    if (!Directory.Exists($"./servers/{server.Id}"))
    {
        Directory.CreateDirectory($"./servers/{server.Id}");
    }
}

foreach (var server in JsonConvert.DeserializeObject<List<ServerModel>>(File.ReadAllText(@"servers.json")))
{
    if (!Directory.Exists($"./backups/{server.Id}"))
    {
        Directory.CreateDirectory($"./backups/{server.Id}");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}"
);

app.MapControllerRoute(
    name: "servers",
    pattern: "{controller=Dashboard}/{action=Servers}"
    
);

app.MapControllerRoute(
    name: "server",
    pattern: "{controller=Server}/{action=Index}/{id}"
);

app.MapControllerRoute(
    name: "files",
    pattern: "{controller=Server}/{action=Files}/{id}"
);

app.MapControllerRoute(
    name: "backup",
    pattern: "{controller=Backup}/{action=Backup}/{id}"
);

app.MapControllerRoute(
    name: "create",
    pattern: "{controller}/{action=Create}/{id}"
);

app.MapControllerRoute(
    name: "apply",
    pattern: "{controller}/{action=Apply}/{id}"
);

app.MapControllerRoute(
    name: "delete",
    pattern: "{controller}/{action=Delete}/{id}"
);

app.MapControllerRoute(
    name: "config",
    pattern: "{controller}/{action=Config}/{id}"
);

app.Run();
