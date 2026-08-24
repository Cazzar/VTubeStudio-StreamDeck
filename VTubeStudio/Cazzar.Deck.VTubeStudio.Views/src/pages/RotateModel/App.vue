<template>
  <NotConnected v-if="!connected" />
  <div v-else>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Default rotation ({{ rotationLabel }})</div>
      <span class="sdpi-item-value">
        <input type="range" min="-360" max="360" step="0.01" v-model.number="settings.defaultRotation">
      </span>
    </div>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Step size ({{ stepLabel }})</div>
      <span class="sdpi-item-value">
        <input type="range" min="1" max="100" step="1" v-model.number="settings.stepSize">
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

const { settings, connected, send } = usePage({ defaultRotation: 0, stepSize: 2 })

const degrees = value => `${value.toLocaleString(undefined, { maximumFractionDigits: 2 })}\u00b0`

const rotationLabel = computed(() => degrees(Number(settings.defaultRotation)))
const stepLabel = computed(() => degrees(Number(settings.stepSize) / 10))
</script>
