<template>
  <div class="warning">
    <p>Not connected to VTubeStudio!</p>
    <p>This could be due to the plugin API not being enabled, or VTubeStudio isn't running</p>
    <p><a @click="openHelp">Click here</a> to open the installation</p>
  </div>
</template>

<script>
export default {
  name: 'NotConnected',
  methods: {
      openHelp() { this.$store.state.streamDeck.openUrl("https://github.com/Cazzar/VTubeStudio-StreamDeck/wiki/Installation#configuring-vtubestudio") },
  },
}
</script>

<style scoped>
.warning {
    padding-left: 30px;
}
</style>
