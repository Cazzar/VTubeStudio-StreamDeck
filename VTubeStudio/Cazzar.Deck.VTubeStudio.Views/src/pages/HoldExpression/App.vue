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
      <div class="sdpi-item-label">Expression</div>
      <select class="sdpi-item-value select" v-model="settings.expressionFile">
        <option v-for="expression in data.expressions" :value="expression.id" :key="expression.id">
          {{ expression.name }}
        </option>
      </select>
    </div>
    <div class="sdpi-item">
      <div class="sdpi-item-label">Options</div>
      <input class="sdpi-item-value" id="showName" type="checkbox" v-model="settings.showName">
      <label for="showName"><span></span>Show expression name on key</label>
    </div>
    <Tools :connected="connected" :send="send" />
  </div>
</template>

<script setup>
import NotConnected from '../../components/NotConnected.vue'
import Tools from '../../components/Tools.vue'
import { usePage } from '../../usePage'

const { settings, data, connected, send } = usePage({ modelId: null, expressionFile: null, showName: true })
</script>
