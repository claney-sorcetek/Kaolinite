using System.ComponentModel.DataAnnotations;

namespace Kaolinite.Models;

public class ServerModel
{
    public int Id { get; set; }
    public string Title { get; set;}
    public int Memory { get; set; }
    public int Port { get; set; }
    public string ConfigPath { get; set; }
    public string RconPassword { get; set; }
    public string DockerFile { get; set; }
}