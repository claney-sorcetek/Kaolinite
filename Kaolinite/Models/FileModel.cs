using System.ComponentModel.DataAnnotations;

namespace Kaolinite.Models;

public class FileModel
{
    public string Path { get; set; }
    public bool IsDir { get; set; }
}