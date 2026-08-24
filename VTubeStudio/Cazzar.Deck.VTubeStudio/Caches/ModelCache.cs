using VTubeStudio.Api;
using VTubeStudio.Api.Models;
using VTubeStudio.Api.Requests;

namespace Cazzar.Deck.VTubeStudio.Caches;

public sealed class ModelCache
{
    private readonly IVTubeStudio _vts;

    public ModelCache(IVTubeStudio vts)
    {
        _vts = vts;

        vts.AvailableModels += (_, e) => Models = e.Response.Models;
        vts.CurrentModel += (_, e) => CurrentModelId = e.Response.IsLoaded ? e.Response.Id : null;
        vts.ModelLoaded += (_, e) => CurrentModelId = e.Response.Loaded ? e.Response.ModelId : null;
        vts.Authenticated += (_, _) => Refresh();
        vts.Disconnected += (_, _) => CurrentModelId = null;
    }

    public IReadOnlyList<Model> Models { get; private set; } = [];
    public string? CurrentModelId { get; private set; }

    public void Refresh()
    {
        _vts.Send(new AvailableModelsRequest());
        _vts.Send(new CurrentModelRequest());
    }
}
