<template>
  <div class="warning">
    <p>Not connected to VTube Studio.</p>
    <p>Check your settings in VTube Studio and make sure the plugin API is enabled.</p>
    <p><a href="#" @click.prevent="openHelp">Open the installation guide</a></p>
    <p>If the plugin API is not on the default port (8001), update it below and click Connect.</p>
  </div>
  <div v-if="ready">
    <div class="sdpi-item">
      <div class="sdpi-item-label">VTS host</div>
      <span class="sdpi-item-value">
        <input type="text" v-model="host">
      </span>
    </div>
    <div class="sdpi-item">
      <div class="sdpi-item-label">VTS port</div>
      <span class="sdpi-item-value">
        <input type="number" min="1" max="65535" step="1" v-model="port">
      </span>
    </div>
    <div class="sdpi-item">
      <button class="sdpi-item-value" @click="connect">Connect</button>
    </div>
  </div>
</template>

<script setup>
import { inject, onMounted, ref } from 'vue'

const deck = inject('deck')

const ready = ref(false)
const host = ref('127.0.0.1')
const port = ref(8001)

const HELP_URL = 'https://github.com/Cazzar/VTubeStudio-StreamDeck/wiki/Installation#configuring-vtubestudio'

deck.on('globalSettings', settings => {
  host.value = settings?.host ?? host.value
  port.value = settings?.port ?? port.value
})

deck.on('connected', () => {
  deck.requestGlobalSettings()
  ready.value = true
})

onMounted(() => {
  if (!deck.isOpen()) return

  deck.requestGlobalSettings()
  ready.value = true
})

function openHelp() {
  deck.openUrl(HELP_URL)
}

function connect() {
  deck.send('set-vtsinfo', { host: host.value, port: Number(port.value) })
}
</script>

<style scoped>
.warning {
  padding-left: 30px;
}
</style>
