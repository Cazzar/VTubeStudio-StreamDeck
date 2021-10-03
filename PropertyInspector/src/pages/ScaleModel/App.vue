<template>
<NotConnected v-if="!websocketConnected" />
<div v-else>
  <div v-if="websocketConnected" class="sdpi-item">
    <div class="sdpi-item-label">Size ({{ settings.size ?? '0' }})</div>
    <span class="sdpi-item-value">
      <input type="range" min="-100" max="100" step="0.1" v-model="settings.size">
    </span>
  </div>
  <div class="sdpi-item">
    <div class="sdpi-item-label">Tools</div>
    <button class="sdpi-item-value" id="force-refresh" @click="sendAction('refresh', null)">Refresh</button>
    <button v-if="!websocketConnected" class="sdpi-item-value" @click="sendAction('force-reconnect', null)" id="force-reconnect">Reconnect</button>
    <span v-else class="sdpi-item-value">VTS Connected!</span>
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
      models: [],
      websocketConnected: false,
      settings: {
        size: 0,
      }
    }
  },
  watch: {
    settings: {
      deep: true,
      handler() {
        this.$store.state.streamDeck.saveSettings(this.settings);
      }
    }
  },
  methods: {
    sendAction(command, payload) {
      this.$store.state.streamDeck.sendToPlugin({
        command, payload
      })
    }
  },
  mounted() {
    this.$store.state.streamDeck.on('didReceiveSettings', settings => console.log(settings))
    this.$store.state.streamDeck.on('connected', payload => this.settings = payload?.payload?.settings ?? this.settings)
    this.$store.state.streamDeck.on('sendToPropertyInspector', e => {
      this.models = e.Models ?? [];
      this.websocketConnected = e.Connected ?? false
    })
  },
}
</script>

<style>

</style>
