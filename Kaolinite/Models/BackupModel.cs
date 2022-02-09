using System.ComponentModel.DataAnnotations;

namespace Kaolinite.Models;

public class BackupModel
{
    public DateTime TimeStamp { get; set; }
    public string Name { get; set; }
}