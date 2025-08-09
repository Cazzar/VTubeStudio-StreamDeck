<template>
<NotConnected v-if="!websocketConnected" />
<div v-else>
  <div v-if="websocketConnected" class="sdpi-item">
    <div class="sdpi-item-label">Size ({{ settings.size ?? '0' }})</div>
    <span class="sdpi-item-value">
      <select v-model="settings.expression">
        <option v-for="expression in expressions" :value="expression.file">{{ expression.name }}</option>
      </select>
      <input type="range" min="0" max="5" step="0.1" v-model="settings.fadeTime">
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
      expressions: [],
      websocketConnected: false,
      settings: {
        expression: '',
        fadeTime: 0.5,
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
      this.expressions = e.expressions ?? [];
      this.websocketConnected = e.connected ?? false
    })
  },
}
</script>

<style>

</style>
