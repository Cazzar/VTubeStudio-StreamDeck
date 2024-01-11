<template>
<NotConnected v-if="!websocketConnected" />
<div v-else>
  <div class="sdpi-item">
    <div class="sdpi-item-label">Rotation ({{ currentRotation }})</div>
    <span class="sdpi-item-value">
      <input type="range" min="0" max="360" step="0.01" v-model="defaultRotation">
    </span>
  </div>
  
  <div class="sdpi-item">
    <div class="sdpi-item-label">Step Size ({{ stepSize ?? '0' }})</div>
    <span class="sdpi-item-value">
      <input type="range" min="1" max="100" step="1" v-model="stepSize">
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
      defaultRotation: 0.0,
      stepSize: 2
    }
  },
  computed: {
    currentRotation() {
      let pos = Number(this.defaultRotation) ?? 0
      
      return pos.toLocaleString(undefined, { maximumFractionDigits: 2 }) + "°";
    }
  },
  watch: {
    defaultRotation() {
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
        defaultRotation: this.defaultPosition,
        stepSize: this.stepSize
      });
    }
  },
  mounted() {
    this.$store.state.streamDeck.on('didReceiveSettings', payload =>
    {
      let settings = payload?.payload?.settings ?? this.settings

      this.defaultRotation = settings.defaultRotation ?? 0.0
      this.stepSize = settings.stepSize ?? 2
    })
    this.$store.state.streamDeck.on('connected', payload => {
      let settings = payload?.payload?.settings ?? this.settings
      
      this.defaultRotation = settings.defaultRotation ?? 0.0
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
