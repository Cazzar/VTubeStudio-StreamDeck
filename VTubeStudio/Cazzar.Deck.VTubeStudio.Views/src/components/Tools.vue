<template>
  <div class="sdpi-item">
    <div class="sdpi-item-label">Tools</div>
    <button class="sdpi-item-value" @click="send('refresh')">Refresh</button>
    <button v-if="!connected" class="sdpi-item-value" @click="send('force-reconnect')">Reconnect</button>
    <span v-else class="sdpi-item-value">VTS connected</span>
  </div>
</template>

<script setup>
defineProps({
  connected: { type: Boolean, default: false },
  send: { type: Function, required: true },
})
</script>
