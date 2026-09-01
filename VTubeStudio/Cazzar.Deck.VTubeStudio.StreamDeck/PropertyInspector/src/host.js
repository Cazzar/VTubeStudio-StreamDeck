import { emitter, socket } from '@shared/deck.js'

export const globalName = 'connectElgatoStreamDeckSocket'

export function connect({ port, uuid, registerEvent, actionInfo }) {
  const events = emitter()
  const action = JSON.parse(actionInfo ?? '{}')

  const link = socket(port, ws => {
    ws.send(JSON.stringify({ event: registerEvent, uuid }))
    events.emit('connected', action?.payload?.settings)
  }, message => {
    switch (message.event) {
      case 'didReceiveSettings':
        events.emit('settings', message.payload?.settings)
        break
      case 'didReceiveGlobalSettings':
        events.emit('globalSettings', message.payload?.settings)
        break
      case 'sendToPropertyInspector':
        events.emit('fromPlugin', message.payload)
        break
    }
  })

  return {
    actionId: action.action ?? '',
    on: events.on,
    isOpen: link.isOpen,

    saveSettings: settings => link.send({ event: 'setSettings', context: uuid, payload: settings }),
    requestGlobalSettings: () => link.send({ event: 'getGlobalSettings', context: uuid }),

    send: (command, payload) => link.send({
      event: 'sendToPlugin',
      action: action.action,
      context: uuid,
      payload: { command, payload },
    }),

    openUrl: url => link.send({ event: 'openUrl', payload: { url } }),
  }
}
