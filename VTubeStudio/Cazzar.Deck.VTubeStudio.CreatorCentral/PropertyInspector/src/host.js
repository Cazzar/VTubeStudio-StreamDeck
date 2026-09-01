import { emitter, socket } from '@shared/deck.js'

export const globalName = 'connectCreatorCentral'

export function connect({ port, uuid, registerEvent, actionInfo }) {
  const events = emitter()
  const widget = typeof actionInfo === 'string' ? JSON.parse(actionInfo || '{}') : (actionInfo ?? {})

  const link = socket(port, ws => {
    ws.send(JSON.stringify({ event: registerEvent, uuid }))
    events.emit('connected', widget?.payload?.settings)
  }, message => {
    switch (message.event) {
      // Unlike Elgato, the payload is the settings object itself.
      case 'didReceiveWidgetSettings':
        events.emit('settings', message.payload)
        break
      case 'didReceivePackageSettings':
        events.emit('globalSettings', message.payload)
        break
      case 'sendToPropertyView':
        events.emit('fromPlugin', message.payload)
        break
    }
  })

  return {
    actionId: widget.widget ?? '',
    on: events.on,
    isOpen: link.isOpen,

    saveSettings: settings => link.send({ event: 'setWidgetSettings', context: uuid, payload: settings }),
    saveGlobalSettings: settings => link.send({
      event: 'setPackageSettings',
      context: uuid,
      payload: { settings },
    }),
    requestGlobalSettings: () => link.send({ event: 'getPackageSettings', context: uuid }),

    send: (command, payload) => link.send({
      event: 'sendToPackage',
      context: uuid,
      widget: widget.widget,
      payload: { command, payload },
    }),

    openUrl: url => link.send({ event: 'openUrl', payload: { url } }),
  }
}
