// The contract every page is written against. Each launcher supplies an adapter that speaks its
// own dialect underneath: sendToPlugin against sendToPackage, global against package settings.
//
//   on(event, fn)            'connected' | 'settings' | 'globalSettings' | 'fromPlugin'
//   saveSettings(settings)
//   requestGlobalSettings()
//   send(command, payload)
//   openUrl(url)

export function emitter() {
  const listeners = new Map()

  return {
    on(event, fn) {
      if (!listeners.has(event)) listeners.set(event, new Set())
      listeners.get(event).add(fn)
      return () => listeners.get(event).delete(fn)
    },
    emit(event, data) {
      listeners.get(event)?.forEach(fn => fn(data))
    },
  }
}

export function socket(port, onOpen, onMessage) {
  const ws = new WebSocket(`ws://localhost:${port}`)

  ws.onopen = () => onOpen(ws)
  ws.onmessage = event => onMessage(JSON.parse(event.data))
  ws.onerror = error => console.error('property view socket error', error)

  return {
    ws,
    send: message => ws.send(JSON.stringify(message)),
    isOpen: () => ws.readyState === WebSocket.OPEN,
  }
}

// Stands in for a launcher so pages can be opened directly during development.
export function stub() {
  const bus = emitter()

  setTimeout(() => {
    bus.emit('connected', {})
    bus.emit('fromPlugin', { connected: true, models: [], hotkeys: [], expressions: [] })
  }, 0)

  return {
    actionId: '',
    on: bus.on,
    isOpen: () => true,
    saveSettings: settings => console.info('saveSettings', settings),
    requestGlobalSettings: () => bus.emit('globalSettings', { host: '127.0.0.1', port: 8001 }),
    send: (command, payload) => console.info('send', command, payload),
    openUrl: url => window.open(url, '_blank'),
  }
}
