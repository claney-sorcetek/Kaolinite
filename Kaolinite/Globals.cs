using Ductus.FluentDocker.Model.Containers;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Extensions;
using Ductus.FluentDocker.Services;
using System.Collections.Generic;

namespace Kaolinite;

public static class Globals
{
    public static Dictionary<string, IContainerService> containers { get; set; }
}
