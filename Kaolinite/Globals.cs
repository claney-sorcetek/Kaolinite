using Ductus.FluentDocker.Model.Containers;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Extensions;
using Ductus.FluentDocker.Services;

namespace Kaolinite;

public static class Globals
{
    public static List<IContainerService> containers { get; set; }
}
