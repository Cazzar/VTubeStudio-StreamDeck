using Cazzar.Deck.Abstractions.Protocol;

namespace Cazzar.Deck.Hosts.Loopback;

public sealed class LoopbackLaunchOptions : IDeckLaunchOptions
{
    public int Port => 0;
    public string Uuid => "loopback";
    public string RegisterEvent => "register";
    public string? Info => null;
}
