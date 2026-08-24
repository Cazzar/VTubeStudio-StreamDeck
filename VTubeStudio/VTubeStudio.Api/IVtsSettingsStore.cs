namespace VTubeStudio.Api;

public interface IVtsSettingsStore
{
    string? Token { get; set; }

    bool IsLoaded { get; }

    void SetEndpoint(string host, ushort port);
}
