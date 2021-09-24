<template>
<div class="sdpi-item" id="select_model">
    <div class="sdpi-item-label">Duration (seconds)</div>
    <span class="sdpi-item-value">
        <input type="number" step="0.1" min="0" max="2" id="seconds" required v-model="settings.seconds"/>
    </span>
</div>
<div type="checkbox" class="sdpi-item">
    <div class="sdpi-item-label">Options</div>
    <input class="sdpi-item-value" id="showName" type="checkbox" v-model="settings.relative">
    <label for="showName"><span></span>Model Moves are relative</label>
</div>
<div class="sdpi-item">
    <div class="sdpi-item-label">Movement</div>
    <CoordInput v-if="gotSettings"
      :relative="settings.relative"
      v-model:x="settings.posX"
      v-model:y="settings.posY"
    />
</div>
<div class="sdpi-item">
    <div class="sdpi-item-label">Rotation</div>
    <span class="sdpi-item-value">
        <input type="number" min="-360" max="360" step="0.1" id="rotation" v-model="settings.rotation"/>
    </span>
</div>
<div class="sdpi-item">
     <div class="sdpi-item-label">Tools</div>
     <button class="sdpi-item-value" id="force-refresh" @click="sendAction('refresh', null)">Refresh</button>
     <button v-if="!websocketConnected" class="sdpi-item-value" @click="sendAction('force-reconnect', null)" id="force-reconnect">Reconnect</button>
     <span v-else class="sdpi-item-value">VTS Connected!</span>
</div>
</template>

<script>
import CoordInput from "../../components/CoordInput.vue";

export default {
  name: 'App',
  components: {
    CoordInput,
  },
  data() {
    return {
      models: [],
      websocketConnected: false,
      gotSettings: false,
      settings: {
        posX: 0,
        posY: 0,
        seconds: 0,
        rotation: null,
        relative: true,
      },
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
    this.$store.state.streamDeck.on('connected', payload => {
      this.settings = payload?.payload?.settings ?? this.settings
      this.gotSettings = true
      console.log(payload?.payload?.settings)
    })
    this.$store.state.streamDeck.on('sendToPropertyInspector', e => {
      this.websocketConnected = e.Connected ?? false
    })
  },
}
</script>

<style>

</style>
