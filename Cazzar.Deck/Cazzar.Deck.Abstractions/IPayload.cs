namespace Cazzar.Deck.Abstractions;

public interface IPayload
{
    T? As<T>();
    bool TryGet<T>(string key, out T? value);

    static IPayload Empty => EmptyPayload.Instance;
}

sealed file class EmptyPayload : IPayload
{
    public static readonly EmptyPayload Instance = new();

    public T? As<T>() => default;

    public bool TryGet<T>(string key, out T? value)
    {
        value = default;
        return false;
    }
}