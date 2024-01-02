using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StreamDeckLib.Models;
using StreamDeckLib.Models.StreamDeckPlus;

namespace StreamDeckLib
{
    public class ActionRepository
    {
        private readonly ILogger<ActionRepository> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly StreamDeckRegistrationOptions _launchOptions;

        private readonly ConcurrentDictionary<string, Type> _actions = new();
        private readonly ConcurrentDictionary<string, object> _instances = new();

        public ActionRepository(IOptions<StreamDeckRegistrationOptions> launchOptions, ILogger<ActionRepository> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _launchOptions = launchOptions.Value;
        }

        public void FindActions()
        {
            if (_launchOptions.LoadingAssembly is not null)
            {
                FindActionsInAssembly(_launchOptions.LoadingAssembly);
                return;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                FindActionsInAssembly(assembly);
            }
        }

        private void FindActionsInAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var attribute = type.GetCustomAttribute<StreamDeckAction>();

                if (attribute is null) continue;

                if (_actions.TryAdd(attribute.ActionId, type)) continue;

                _logger.LogError("Already found an action of type: {ActionId} registered with type {AlreadyRegistered}, not registering {Type}", attribute.ActionId, _actions[attribute.ActionId], type);
            }
        }

        internal void Appeared(OnWillAppear appear)
        {
            if (!_actions.TryGetValue(appear.Action, out var actionType))
            {
                _logger.LogCritical("Unable to find action of type {Type}", appear.Action);
                return;
            }

            var instance = ActivatorUtilities.CreateInstance(_serviceProvider, actionType);

            if (instance is IHasSettings settings)
                settings.GotSettings(appear.Payload.Settings);

            if (instance is IContextAware contextAware)
                contextAware.ContextId = appear.Context;

            if (instance is IDeviceAware deviceAware)
                deviceAware.DeviceId = appear.Device;


            _instances[appear.Context] = instance;
        }

        internal void Disappeared(OnWillDisappear disappear)
        {
            if (!_instances.ContainsKey(disappear.Context))
            {
                _logger.LogCritical("SendToPlugin: Instance ID {Instance} was not found in the context, this is a major internal error", disappear.Context);
                return;
            }

            _instances.Remove(disappear.Context, out var button);

            if (button is IDisposable d) d.Dispose();
        }

        internal void GotSettings(DidReceiveSettings settings)
        {
            if (!_instances.TryGetValue(settings.Context, out var instance))
            {
                _logger.LogCritical("SendToPlugin: Instance ID {Instance} was not found in the context, this is a major internal error", settings.Context);
                return;
            }

            if (instance is not IHasSettings hasSettings)
                return;

            hasSettings.GotSettings(settings.Payload.Settings);
        }

        internal void ButtonDown(KeyDown keyDown)
        {
            if (!_instances.TryGetValue(keyDown.Context, out var instance))
            {
                _logger.LogCritical("SendToPlugin: Instance ID {Instance} was not found in the context, this is a major internal error", keyDown.Context);
                return;
            }

            if (instance is not IButtonReactions reactions)
                return;

            reactions.KeyDown(keyDown.Payload);
        }

        internal void ButtonUp(KeyUp keyUp)
        {
            if (!_instances.TryGetValue(keyUp.Context, out var instance))
            {
                _logger.LogCritical("SendToPlugin: Instance ID {Instance} was not found in the context, this is a major internal error", keyUp.Context);
                return;
            }

            if (instance is not IButtonReactions reactions)
                return;

            reactions.KeyUp(keyUp.Payload);
        }

        public void PropertyInspectorAppeared(PropertyInspectorDidAppear didAppear)
        {
            if (!_instances.TryGetValue(didAppear.Context, out var instance))
            {
                _logger.LogCritical("SendToPlugin: Instance ID {Instance} was not found in the context, this is a major internal error", didAppear.Context);
                return;
            }

            if (instance is not IPropertyInspector propertyInspector)
                return;

            propertyInspector.Appeared(didAppear);
        }

        public void PropertyInspectorDisappeared(PropertyInspectorDidDisappear didDisappear)
        {
            if (!_instances.TryGetValue(didDisappear.Context, out var instance))
            {
                _logger.LogCritical("SendToPlugin: Instance ID {Instance} was not found in the context, this is a major internal error", didDisappear.Context);
                return;
            }

            if (instance is not IPropertyInspector propertyInspector)
                return;

            propertyInspector.Disappeared(didDisappear);
        }

        public void SendToPlugin(SendToPlugin sendToPlugin)
        {
            if (!_instances.TryGetValue(sendToPlugin.Context, out var instance))
            {
                _logger.LogCritical("SendToPlugin: Instance ID {Instance} was not found in the context, this is a major internal error", sendToPlugin.Context);
                return;
            }

            if (instance is not IPropertyInspector propertyInspector)
                return;

            propertyInspector.OnSendToPlugin(sendToPlugin);
        }

        public void TitleParamsChange(TitleParametersDidChange titleParameters)
        {
            if (!_instances.TryGetValue(titleParameters.Context, out var instance))
            {
                _logger.LogCritical("SendToPlugin: Instance ID {Instance} was not found in the context, this is a major internal error", titleParameters.Context);
                return;
            }

            if (instance is not ITitleParams titleParams)
                return;

            titleParams.GotTitleParams(titleParameters);
        }
        
        public void TouchTap(TouchTap touchTap)
        {
            if (!_instances.TryGetValue(touchTap.Context, out var instance))
            {
                _logger.LogCritical("SendToPlugin: Instance ID {Instance} was not found in the context, this is a major internal error", touchTap.Context);
                return;
            }

            if (instance is not IStreamDeckPlus touchHandler)
                return;

            touchHandler.Touch(touchTap.Payload);
        }
        
        public void DialDown(DialDown dialDown)
        {
            if (!_instances.TryGetValue(dialDown.Context, out var instance))
            {
                _logger.LogCritical("SendToPlugin: Instance ID {Instance} was not found in the context, this is a major internal error", dialDown.Context);
                return;
            }

            if (instance is not IStreamDeckPlus dialHandler)
                return;

            dialHandler.DialDown(dialDown.Payload);
        }
        
        public void DialUp(DialUp dialUp)
        {
            if (!_instances.TryGetValue(dialUp.Context, out var instance))
            {
                _logger.LogCritical("SendToPlugin: Instance ID {Instance} was not found in the context, this is a major internal error", dialUp.Context);
                return;
            }

            if (instance is not IStreamDeckPlus dialHandler)
                return;

            dialHandler.DialUp(dialUp.Payload);
        }
        
        public void DialRotate(DialRotate dialRotate) 
        {
            if (!_instances.TryGetValue(dialRotate.Context, out var instance)) {
                _logger.LogCritical("SendToPlugin: Instance ID {Instance} was not found in the context, this is a major internal error", dialRotate.Context);
                return;
            }

            if (instance is not IStreamDeckPlus dialHandler)
                return;

            dialHandler.DialRotate(dialRotate.Payload);
        }

        public void Tick()
        {
            foreach (var (_, action) in _instances)
            {
                if (action is not ITickHandler handler) return;

                handler.Tick();
            }
        }
    }

    public interface ITickHandler
    {
        void Tick();
    }
}
