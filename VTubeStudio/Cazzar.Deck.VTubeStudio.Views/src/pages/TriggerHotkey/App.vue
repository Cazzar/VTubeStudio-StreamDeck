<template>
  <NotConnected v-if="!connected" />
  <div v-else>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Model</div>
      <select class="sdpi-item-value select" v-model="settings.modelId">
        <option v-for="model in data.models" :value="model.id" :key="model.id">{{ model.name }}</option>
      </select>
      <button class="sdpi-item-value" @click="send('select-current-model')">Current</button>
    </div>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Hotkey</div>
      <select class="sdpi-item-value select" id="hotkeyId" v-model="settings.hotkeyId">
        <option v-for="hotkey in data.hotkeys" :value="hotkey.id" :key="hotkey.id">{{ hotkey.name }}</option>
      </select>
    </div>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Options</div>
      <input class="sdpi-item-value" id="showName" type="checkbox" v-model="settings.showName">
      <label for="showName"><span></span>Show hotkey name on key</label>
    </div>
    <Tools :connected="connected" :send="send" />
  </div>
</template>

<script setup>
import NotConnected from '../../components/NotConnected.vue'
import Tools from '../../components/Tools.vue'
import { usePage } from '../../usePage'

const { settings, data, connected, send } = usePage({ modelId: null, hotkeyId: null, showName: true })
</script>

<style scoped>
#hotkeyId {
  width: 1vw;
  padding-right: 26px;
  text-overflow: ellipsis;
}
</style>
