<template>
<NotConnected v-if="!websocketConnected" />
<div v-else>
  <div class="sdpi-item">
    <div class="sdpi-item-label">Position ({{ currentPos }})</div>
    <span class="sdpi-item-value">
      <input type="range" min="-1" max="1" step="0.01" v-model="defaultPosition">
    </span>
  </div>
  
  <div class="sdpi-item">
    <div class="sdpi-item-label">Step Size ({{ stepSize ?? '0' }})</div>
    <span class="sdpi-item-value">
      <input type="range" min="1" max="10" step="1" v-model="stepSize">
    </span>
  </div>
  
  <div class="sdpi-item">
    <div class="sdpi-item-label">Tools</div>
    <button class="sdpi-item-value" id="use-current" @click="sendAction('use-current', null)">Current</button>
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
      defaultPosition: 0.0,
      stepSize: 2
    }
  },
  computed: {
    currentPos() {
      let pos = Number(this.defaultPosition) ?? 0
      
      return ((pos + 1) / 2.0).toLocaleString(undefined, { style: "percent" });
    }
  },
  watch: {
    defaultPosition() {
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
        defaultPosition: this.defaultPosition,
        stepSize: this.stepSize
      });
    }
  },
  mounted() {
    this.$store.state.streamDeck.on('didReceiveSettings', payload =>
    {
      console.log(payload)
      let settings = payload?.payload?.settings ?? this.settings

      this.defaultPosition = settings.defaultPosition ?? 0.0
      this.stepSize = settings.stepSize ?? 2
    })
    this.$store.state.streamDeck.on('connected', payload => {
      let settings = payload?.payload?.settings ?? this.settings
      
      this.defaultPosition = settings.defaultPosition ?? 0.0
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
