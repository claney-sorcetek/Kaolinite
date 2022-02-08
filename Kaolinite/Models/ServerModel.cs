using System.ComponentModel.DataAnnotations;

namespace Kaolinite.Models;

public class ServerModel
{
    public int Id { get; set; }
    public string? Title { get; set;}
    public int? Memory { get; set; }
}