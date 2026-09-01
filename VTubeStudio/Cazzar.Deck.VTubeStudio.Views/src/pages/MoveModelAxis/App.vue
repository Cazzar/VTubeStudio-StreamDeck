<template>
  <NotConnected v-if="!connected" />
  <div v-else>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Default position ({{ positionLabel }})</div>
      <span class="sdpi-item-value">
        <input type="range" min="-1" max="1" step="0.01" v-model.number="settings.defaultPosition">
      </span>
    </div>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Step size ({{ settings.stepSize }})</div>
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

const { settings, connected, send } = usePage({ defaultPosition: 0, stepSize: 2 })

const positionLabel = computed(() =>
  ((Number(settings.defaultPosition) + 1) / 2).toLocaleString(undefined, { style: 'percent' }))
</script>
