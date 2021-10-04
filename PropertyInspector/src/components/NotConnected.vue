<template>
  <div class="warning">
    <p>Not connected to VTubeStudio!</p>
    <p>Check your setting in VTube Studio and make sure the plugin API is enabled</p>
    <p><a @click="openHelp">Click here</a> to open the installation guide</p>
    <p>If the plugin API is not on the default port (8001) you can update it below and click "connect"</p>
  </div>
  <div v-if="connected">
    <div class="sdpi-item">
      <div class="sdpi-item-label">VTS IP</div>
      <span class="sdpi-item-value">
          <input type="text" v-model="host"/>
      </span>
    </div>
    <div class="sdpi-item">
      <div class="sdpi-item-label">VTS Port</div>
      <span class="sdpi-item-value">
          <input type="number" min="1" max="65535" step="1" id="rotation" v-model="port"/>
      </span>
    </div>
    <div class="sdpi-item">
      <button class="sdpi-item-value" @click="reconnect()">Connect</button>
    </div>
  </div>
</template>

<script>
export default {
  name: 'NotConnected',
  mounted() {
    this.$store.state.streamDeck.on('globalsettings', (settings) => {
      this.host = settings.Host;
      this.port = settings.Port;
    })
    this.$store.state.streamDeck.on('connected', () => {
      this.$store.state.streamDeck.requestGlobalSettings()
      this.connected = true;
    })
    if (this.$store.state.streamDeck?.streamDeckWebsocket) {
      this.$store.state.streamDeck.requestGlobalSettings();
      this.connected = true;
    }
  },
  methods: {
    openHelp() { this.$store.state.streamDeck.openUrl("https://github.com/Cazzar/VTubeStudio-StreamDeck/wiki/Installation#configuring-vtubestudio") },
    reconnect() {
      this.$store.state.streamDeck.sendToPlugin({
        command: 'set-vtsinfo',
        payload: {
          host: this.host,
          port: this.port,
        }
      })
    }
  },
  data() {
    return {
      connected: false,
      host: '127.0.0.1',
      port: '8001',
    }
  },
}
</script>

<style scoped>
.warning {
    padding-left: 30px;
}
</style>
