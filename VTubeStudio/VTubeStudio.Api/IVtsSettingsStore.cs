namespace VTubeStudio.Api;

public interface IVtsSettingsStore
{
    string? Token { get; set; }

    // False until the host has handed over its stored settings. Connecting before then would send an
    // AuthenticationTokenRequest and prompt the user for permission they have already granted.
    bool IsLoaded { get; }

    void SetEndpoint(string host, ushort port);
}
