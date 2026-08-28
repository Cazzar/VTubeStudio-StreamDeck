import { inject, reactive, ref, watch } from 'vue'

// Common wiring for every page: settings round-trip, connection state, and whatever the action
// last pushed down. Pages declare their defaults and read the rest.
export function usePage(defaults) {
  const deck = inject('deck')

  const settings = reactive({ ...defaults })
  const data = reactive({ models: [], hotkeys: [], expressions: [] })
  const connected = ref(false)
  const loaded = ref(false)

  let applying = false

  function apply(incoming) {
    if (!incoming) return

    applying = true
    Object.assign(settings, incoming)
    loaded.value = true
    queueMicrotask(() => { applying = false })
  }

  deck.on('connected', apply)
  deck.on('settings', apply)

  deck.on('fromPlugin', payload => {
    connected.value = payload?.connected ?? false
    Object.assign(data, payload ?? {})
  })

  const filled = () => Object.fromEntries(Object.entries(settings).filter(([, value]) => value !== ''))

  // Do not echo back the settings the host just sent us.
  watch(settings, () => {
    if (!applying) deck.saveSettings(filled())
  }, { deep: true })

  return {
    deck,
    settings,
    data,
    connected,
    loaded,
    send: (command, payload = null) => deck.send(command, payload),
  }
}
