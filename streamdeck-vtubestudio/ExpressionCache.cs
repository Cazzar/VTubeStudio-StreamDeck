using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Microsoft.Extensions.Logging;
using VTubeStudioAPI.Models;
using VTubeStudioAPI.Responses;

namespace Cazzar.StreamDeck.VTubeStudio;

public class ExpressionCache
{
    private readonly VTubeStudioWebsocketClient _vts;
    private readonly ILogger<ExpressionCache> _logger;

    private readonly SemaphoreSlim _semaphore = new (1, 1);

    private static readonly List<WeakReference<ExpressionCache>> instances = new();

    public ReadOnlyCollection<Expression> Expressions { get; set; }
    public string LastModelId { get; set; }
    public string LastModelName { get; set; }
    public bool ModelLoaded { get; set; }


    public ExpressionCache(ModelCache modelCache, VTubeStudioWebsocketClient vts, ILogger<ExpressionCache> logger)
    {
        instances.Add(new(this));
        _vts = vts;
        _logger = logger;
        VTubeStudioWebsocketClient.OnExpressionState += OnExpressionState;
        VTubeStudioWebsocketClient.OnModelLoad += Refresh;
        VTubeStudioWebsocketClient.OnHotkeyTriggeredEvent += RefreshHotkey;
        VTubeStudioWebsocketClient.OnCurrentModelInformation += Refresh;
        VTubeStudioWebsocketClient.OnAuthenticationResponse += HandleAuthenticated;
        VTubeStudioWebsocketClient.OnExpressionActivation += Refresh;
    }
    private void HandleAuthenticated(object sender, ApiEventArgs<AuthenticationResponse> e)
    {
        if (!e.Response.Authenticated)
            return;
        
        Refresh(sender, e);
    }
    
    private void RefreshHotkey(object sender, ApiEventArgs<HotkeyTriggeredEvent> e)
    {
        if (e.Response.Action != "ToggleExpression") return;
        
        Refresh(sender, e);
    }
    
    private void Refresh(object sender, object e)
    {
        _vts.Send(new ExpressionStateRequest());
    }

    private async void OnExpressionState(object sender, ApiEventArgs<ExpressionStateResponse> e)
    {
        try
        {
            await _semaphore.WaitAsync();
            
            Expressions = e.Response.Expressions.AsReadOnly();
            LastModelId = e.Response.ModelId;
            LastModelName = e.Response.ModelName;
            ModelLoaded = e.Response.ModelLoaded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating expression cache");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    ~ExpressionCache()
    {
        instances.Remove(new(this));
        VTubeStudioWebsocketClient.OnExpressionState -= OnExpressionState;
        VTubeStudioWebsocketClient.OnModelLoad -= Refresh;
        VTubeStudioWebsocketClient.OnHotkeyTriggeredEvent -= RefreshHotkey;
        VTubeStudioWebsocketClient.OnCurrentModelInformation -= Refresh;
        VTubeStudioWebsocketClient.OnAuthenticationResponse -= HandleAuthenticated;
    }
        
    public event EventHandler<ExpressionCacheUpdatedEventArgs> Updated;
}

public class ExpressionCacheUpdatedEventArgs : EventArgs
{
    public IDictionary<string, List<Expression>> Expressions { get; init; }
}