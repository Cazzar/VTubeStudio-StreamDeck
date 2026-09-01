namespace VTubeStudio.Api;

public interface IVtsPluginInfo
{
    string Name { get; }
    string Developer { get; }
    string? Icon { get; }
}
