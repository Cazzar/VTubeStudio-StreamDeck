<template>
  <NotConnected v-if="!connected" />
  <PositionTabs v-else :settings="settings" :connected="connected" :loaded="loaded" :send="send" />
</template>

<script setup>
import NotConnected from '../../components/NotConnected.vue'
import PositionTabs from '../../components/PositionTabs.vue'
import { usePage } from '../../usePage'

const { settings, connected, loaded, send } = usePage({
  posX: 0, posY: 0, seconds: 0, size: null, rotation: null, relative: true,
})
</script>
