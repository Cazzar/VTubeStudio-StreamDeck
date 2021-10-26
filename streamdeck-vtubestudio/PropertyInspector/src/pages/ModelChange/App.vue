<template>
<NotConnected v-if="!websocketConnected" />
<div v-else>
  <div class="sdpi-item">
    <div class="sdpi-item-label">Model</div>
      <select class="sdpi-item-value select" id="hotkeyId" v-model="settings.modelId">
        <option v-for="model in models" v-bind:value="model.id" v-bind:key="model.id">
          {{ model.name }}
        </option>
      </select>
  </div>
  <div class="sdpi-item">
    <div class="sdpi-item-label">Options</div>
    <input class="sdpi-item-value" id="showName" type="checkbox" v-model="settings.showName">
    <label for="showName"><span></span>Show model name on key</label>
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
        modelId: null,
        showName: true,
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
      this.models = e.models ?? [];
      this.websocketConnected = e.connected ?? false
    })
  },
}
</script>

<style>

</style>
