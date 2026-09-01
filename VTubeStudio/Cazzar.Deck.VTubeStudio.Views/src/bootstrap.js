import { createApp, defineAsyncComponent, h } from 'vue'
import { stub } from './deck.js'
import './styles/sdpi.css'

const missing = name => () => h('div', { class: 'sdpi-item' }, `No property view for "${name}".`)

// Which page to show comes from the action the host says this view belongs to. A hash overrides
// it, which is how the app is driven in `vite dev` where no launcher ever calls us.
function pick(routes, actionId) {
  const hash = decodeURIComponent(window.location.hash.replace(/^#/, ''))
  const wanted = hash || actionId

  if (routes[wanted]) return routes[wanted]

  const suffix = Object.entries(routes).find(([id]) => id.endsWith(`.${wanted.toLowerCase()}`))
  return suffix?.[1] ?? missing(wanted)
}

function mount(routes, actionId, deck) {
  createApp(defineAsyncComponent(pick(routes, actionId)))
    .provide('deck', deck)
    .mount('#app')
}

export function bootstrap(routes, host) {
  window[host.globalName] = (port, uuid, registerEvent, info, actionInfo) => {
    const deck = host.connect({ port, uuid, registerEvent, info, actionInfo })
    mount(routes, deck.actionId, deck)
  }

  if (import.meta.env.DEV && window.location.hash) mount(routes, '', stub())
}
