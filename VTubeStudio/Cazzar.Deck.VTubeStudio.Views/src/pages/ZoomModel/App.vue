<template>
  <NotConnected v-if="!connected" />
  <div v-else>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Default zoom ({{ zoomLabel }})</div>
      <span class="sdpi-item-value">
        <input type="range" min="-100" max="100" step="0.01" v-model.number="settings.defaultZoom">
      </span>
    </div>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Step size ({{ stepLabel }})</div>
      <span class="sdpi-item-value">
        <input type="range" min="1" max="10" step="1" v-model.number="settings.stepSize">
      </span>
    </div>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Tools</div>
      <button class="sdpi-item-value" @click="send('use-current')">Use current</button>
      <button class="sdpi-item-value" @click="send('refresh')">Refresh</button>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import NotConnected from '../../components/NotConnected.vue'
import { usePage } from '../../usePage'

const { settings, connected, send } = usePage({ defaultZoom: 0, stepSize: 2 })

const percent = value => value.toLocaleString(undefined, { style: 'percent', maximumFractionDigits: 1 })

const zoomLabel = computed(() => percent((Number(settings.defaultZoom) + 100) / 200))
const stepLabel = computed(() => percent(Number(settings.stepSize) / 200))
</script>
