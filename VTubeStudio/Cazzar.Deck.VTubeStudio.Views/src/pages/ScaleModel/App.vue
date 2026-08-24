<template>
  <NotConnected v-if="!connected" />
  <div v-else>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Size ({{ settings.size ?? 0 }})</div>
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
    <Tools :connected="connected" :send="send" />
  </div>
</template>

<script setup>
import NotConnected from '../../components/NotConnected.vue'
import Tools from '../../components/Tools.vue'
import { usePage } from '../../usePage'

const { settings, connected, send } = usePage({ size: 0 })
</script>
