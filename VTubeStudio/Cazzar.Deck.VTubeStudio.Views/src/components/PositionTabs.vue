<template>
  <div class="tab-bar">
    <div v-for="tab in tabs" :key="tab" class="tab-button" :class="{ active: page === tab }" @click="page = tab">
      {{ labels[tab] }}
    </div>
  </div>

  <div v-if="page === 'move'" class="page-move">
    <CoordInput v-if="loaded" :relative="settings.relative ?? false"
                v-model:x="settings.posX" v-model:y="settings.posY" />
    <div class="content">
      <div>X position</div>
      <input type="number" v-model.number="settings.posX">
      <div>Y position</div>
      <input type="number" v-model.number="settings.posY">
    <div v-if="captureOnEveryTab" class="sdpi-item">
      <button class="sdpi-item-value" @click="send('get-params')">Get current</button>
    </div>
    </div>
  </div>

  <div v-else-if="page === 'scale'">
    <div class="sdpi-item">
      <div class="sdpi-item-label">Size</div>
      <span class="sdpi-item-value">
        <input type="range" min="-100" max="100" step="0.1" v-model.number="settings.size">
      </span>
    </div>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Value</div>
      <span class="sdpi-item-value">
        <input type="number" min="-100" max="100" step="0.1" v-model.number="settings.size">
      </span>
    </div>
    <div v-if="captureOnEveryTab" class="sdpi-item">
      <button class="sdpi-item-value" @click="send('get-params')">Get current</button>
    </div>
  </div>

  <div v-else-if="page === 'rotate'">
    <div class="sdpi-item">
      <div class="sdpi-item-label">Rotation</div>
      <span class="sdpi-item-value">
        <input type="number" min="-360" max="360" step="0.1" v-model.number="settings.rotation">
      </span>
    </div>
    <div v-if="captureOnEveryTab" class="sdpi-item">
      <button class="sdpi-item-value" @click="send('get-params')">Get current</button>
    </div>
  </div>

  <div v-else-if="page === 'options'">
    <div class="sdpi-item">
      <div class="sdpi-item-label">Duration (seconds)</div>
      <span class="sdpi-item-value">
        <input type="number" min="0" max="2" step="0.1" v-model.number="settings.seconds">
      </span>
    </div>
    <div v-if="showRelative" class="sdpi-item">
      <div class="sdpi-item-label">Options</div>
      <input class="sdpi-item-value" id="relative" type="checkbox" v-model="settings.relative">
      <label for="relative"><span></span>Model moves are relative</label>
    </div>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Tools</div>
      <button class="sdpi-item-value" @click="send('get-params')">Get current</button>
      <button v-if="!connected" class="sdpi-item-value" @click="send('force-reconnect')">Reconnect</button>
      <span v-else class="sdpi-item-value">VTS connected</span>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import CoordInput from './CoordInput.vue'

defineProps({
  settings: { type: Object, required: true },
  connected: { type: Boolean, default: false },
  loaded: { type: Boolean, default: false },
  send: { type: Function, required: true },
  showRelative: { type: Boolean, default: true },
  captureOnEveryTab: { type: Boolean, default: false },
})

const tabs = ['move', 'scale', 'rotate', 'options']
const labels = { move: 'Move', scale: 'Size', rotate: 'Rotate', options: 'Options' }
const page = ref('move')
</script>

<style scoped>
.page-move {
  display: grid;
  grid-template-columns: min-content 1fr;
  gap: 10px;
}

.page-move > .content > input {
  min-width: 0;
  width: 150px;
}

.tab-bar {
  display: flex;
  padding-bottom: 10px;
  padding-left: 50px;
}

.tab-button {
  border: 1px solid var(--sdpi-buttonbordercolor);
  padding: 0 5px;
  cursor: pointer;
}

.tab-button.active {
  background-color: var(--sdpi-buttonbordercolor);
  color: var(--sdpi-bordercolor);
}
</style>
