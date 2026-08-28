using System.Text.Json.Nodes;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.VTubeStudio.Caches;
using System.Text.Json.Serialization;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Hotkeys;

[DeckAction(Name = "Trigger Hotkey", Tooltip = "VTubeStudio [Trigger Hotkey]",
    Icon = "vts_logo_transparent", PropertyView = "TriggerHotkey")]
public sealed class TriggerHotkeyAction(
    DeckActionContext context,
    IVTubeStudio vts,
    ModelCache models,
    HotkeyCache hotkeys) : VTubeStudioAction<TriggerHotkeyAction.Options>(context, vts)
{
    public sealed class Options
    {
        [JsonPropertyName("modelId")] public string ModelId { get; set; } = string.Empty;
        [JsonPropertyName("hotkeyId")] public string HotkeyId { get; set; } = string.Empty;
        [JsonPropertyName("showName")] public bool ShowName { get; set; } = true;
    }

    protected override void Pressed()
    {
        if (string.IsNullOrEmpty(Settings.HotkeyId)) return;

        Vts.Send(new HotkeyTriggerRequest(Settings.HotkeyId));
    }

    protected override JsonNode ClientData() => new JsonObject
    {
        ["models"] = Choices(models.Models, m => m.Id, m => m.Name),
        ["hotkeys"] = Choices(hotkeys.For(Settings.ModelId), h => h.Id, h => h.ButtonTitle()),
        ["connected"] = Vts.IsAuthenticated,
    };

    protected override void OnSettingsChanged(Options previous, Options current)
    {
        if (previous.ShowName && !current.ShowName) Title = null;

        UpdateTitle();
    }

    public override void Refresh(IPayload body)
    {
        models.Refresh();
        hotkeys.Refresh();
        base.Refresh(body);
    }

    [PropertyViewCommand("select-current-model")]
    public void SelectCurrentModel(IPayload body)
    {
        if (models.CurrentModelId is not { } id) return;

        Settings.ModelId = id;
        _ = SaveSettingsAsync();
        _ = UpdateClientAsync();
    }

    public override void Tick()
    {
        base.Tick();
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        if (!Settings.ShowName) return;

        if (hotkeys.For(Settings.ModelId).FirstOrDefault(h => h.Id == Settings.HotkeyId)?.ButtonTitle() is { Length: > 0 } title)
            Title = title;
    }
}
