<template>
<NotConnected v-if="!websocketConnected" />
<div v-else>
  <div class="sdpi-item">
    <div class="sdpi-item-label">Zoom ({{ currentZoom }})</div>
    <span class="sdpi-item-value">
      <input type="range" min="-100" max="100" step="0.01" v-model="defaultZoom">
    </span>
  </div>
  
  <div class="sdpi-item">
    <div class="sdpi-item-label">Step Size ({{ zoomStep }})</div>
    <span class="sdpi-item-value">
      <input type="range" min="1" max="10" step="1" v-model="stepSize">
    </span>
  </div>
  
  <div class="sdpi-item">
    <div class="sdpi-item-label">Tools</div>
    <button class="sdpi-item-value" id="use-current" @click="sendAction('use-current', null)">Current Pos</button>
    <button class="sdpi-item-value" id="force-refresh" @click="sendAction('refresh', null)">Refresh</button>
  </div>
</div>
</template>

<script>
import NotConnected from "../../components/NotConnected.vue";

export default {
  name: 'App',
  components: {
    NotConnected
  },
  data() {
    return {
      websocketConnected: false,
      defaultZoom: 0.0,
      stepSize: 2
    }
  },
  computed: {
    currentZoom() {
      let pos = Number(this.defaultZoom) ?? 0
      
      return ((pos + 100) / 200.0).toLocaleString(undefined, { style: "percent", maximumFractionDigits: 1 });
    },
    zoomStep() {
      return ((Number(this.stepSize) ?? 1) / 200).toLocaleString(undefined, { style: "percent", maximumFractionDigits: 1 })
    }
  },
  watch: {
    defaultZoom() {
      this.saveSettings()
    },
    stepSize() {
      this.saveSettings()
    }
  },
  methods: {
    sendAction(command, payload) {
      this.$store.state.streamDeck.sendToPlugin({
        command, payload
      })
    },
    saveSettings() {
      this.$store.state.streamDeck.saveSettings({
        defaultZoom: this.defaultZoom,
        stepSize: this.stepSize
      });
    }
  },
  mounted() {
    this.$store.state.streamDeck.on('didReceiveSettings', payload =>
    {
      let settings = payload?.payload?.settings ?? this.settings

      this.defaultZoom = settings.defaultZoom ?? 0.0
      this.stepSize = settings.stepSize ?? 2
    })
    this.$store.state.streamDeck.on('connected', payload => {
      let settings = payload?.payload?.settings ?? this.settings
      
      this.defaultZoom = settings.defaultZoom ?? 0.0
      this.stepSize = settings.stepSize ?? 2
    })
    this.$store.state.streamDeck.on('sendToPropertyInspector', e => {
      this.websocketConnected = e.connected ?? false
    })
  },
}
</script>

<style>

</style>
