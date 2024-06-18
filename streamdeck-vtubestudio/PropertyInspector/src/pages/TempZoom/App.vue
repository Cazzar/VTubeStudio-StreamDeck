<template>
<NotConnected v-if="!websocketConnected" />
<div v-else>
  <div id="tab-bar">
    <div class="tab-button" :class="{'active': page == 'move'}" @click="page = 'move'">Move</div>
    <div class="tab-button" :class="{'active': page == 'scale'}" @click="page = 'scale'">Size</div>
    <div class="tab-button" :class="{'active': page == 'rotate'}" @click="page = 'rotate'">Rotate</div>
    <div class="tab-button" :class="{'active': page == 'options'}" @click="page = 'options'">Options</div>
  </div>
  <div v-if="page == 'move'" class="page-move">
    <CoordInput v-if="gotSettings"
      :relative="settings.relative"
      v-model:x="settings.posX"
      v-model:y="settings.posY"
    ></CoordInput>
    <div class="content">
      <div>X Position</div>
      <input type="number" v-model="settings.posX" >
      <div>Y Position</div>
      <input type="number" v-model="settings.posY" >
      <div class="sdpi-item">
        <button class="sdpi-item-value" id="get-params1" @click="sendAction('get-params', null)">Get Current</button>
      </div>
    </div>
  </div>

  <div v-else-if="page == 'scale'">
    <div class="sdpi-item">
      <div class="sdpi-item-label">Size</div>
      <span class="sdpi-item-value">
        <input type="range" min="-100" max="100" step="0.1" v-model="settings.size">
      </span>
    </div>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Value</div>
      <span class="sdpi-item-value">
        <input type="number" min="-100" max="100" step="0.1" v-model="settings.size">
      </span>
    </div>
    <div class="sdpi-item">
      <button class="sdpi-item-value" id="get-params2" @click="sendAction('get-params', null)">Get Current</button>
    </div>
  </div>

  <div v-else-if="page == 'rotate'">
    <div class="sdpi-item">
      <div class="sdpi-item-label">Rotation</div>
      <span class="sdpi-item-value">
        <input type="number" min="-360" max="360" step="0.1" id="rotation" v-model="settings.rotation"/>
      </span>
    </div>
    <div class="sdpi-item">
      <button class="sdpi-item-value" id="get-params3" @click="sendAction('get-params', null)">Get Current</button>
    </div>
  </div>

  <div v-else-if="page == 'options'">
    <div class="sdpi-item" id="select_model">
      <div class="sdpi-item-label">Duration (seconds)</div>
      <span class="sdpi-item-value">
        <input type="number" step="0.1" min="0" max="2" id="seconds" required v-model="settings.seconds"/>
      </span>
    </div>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Tools</div>
      <button class="sdpi-item-value" id="get-params" @click="sendAction('get-params', null)">Get Current</button>
      <button v-if="!websocketConnected" class="sdpi-item-value" @click="sendAction('force-reconnect', null)" id="force-reconnect">Reconnect</button>
      <span v-else class="sdpi-item-value">VTS Connected!</span>
    </div>
  </div>

<!--


-->
</div>
</template>

<script>
import CoordInput from "../../components/CoordInput.vue";
import NotConnected from "../../components/NotConnected.vue";

export default {
  name: 'App',
  components: {
    CoordInput,
    NotConnected
  },
  data() {
    return {
      models: [],
      websocketConnected: false,
      gotSettings: false,
      page: 'move',
      settings: {
        posX: 0,
        posY: 0,
        seconds: 0,
        size: null,
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
    this.$store.state.streamDeck.on('didReceiveSettings', settings => this.settings = settings?.payload?.settings ?? this.settings)
    this.$store.state.streamDeck.on('connected', payload => {
      this.settings = payload?.payload?.settings ?? this.settings
      this.gotSettings = true
    })
    this.$store.state.streamDeck.on('sendToPropertyInspector', e => {
      this.websocketConnected = e.connected ?? false
    })
  },
}
</script>

<style scoped>
.page-move {
  display: grid;
  grid-template-columns: min-content 1fr;
  gap: 10px;
}

.page-move > .content > input {
  min-width: 0px;
  width: 150px;
}

#tab-bar {
  display: flex;
  padding-bottom: 10px;
  padding-left: 50px;
}

.tab-button {
  border: 1px solid var(--sdpi-buttonbordercolor);
  /* border-radius: 5px 5px 0px 0px; */
  padding: 0px 5px;
  cursor: pointer;
}

.tab-button.active {
  background-color: var(--sdpi-buttonbordercolor);
  color: var(--sdpi-bordercolor);
}
</style>