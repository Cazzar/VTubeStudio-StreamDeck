<template>
  <div id="container">
    <canvas width="200" height="111.36" ref="canvas" @click="canvasClick" @mousemove="mouseMove" id="position"></canvas>
    <!-- <input type="range" min="-1" max="1" v-model="position.y" step="0.001" orient="vertical">
    <input type="range" min="-1" max="1" v-model="position.x" step="0.001"> -->
    <svg xmlns="http://www.w3.org/2000/svg" id="reset" @click="reset" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
    </svg>
  </div>
</template>

<script>

export default {
  name: 'CoordInput',
  props: {
    relative: {
      type: Boolean,
      default: false,
    },
    x: {
      type: Number,
      default: 0
    },
    y: {
      type: Number,
      default: 0
    }
  },
  emits: [
    'update:x',
    'update:y',
  ],
  data() {
    return {
      /** @type {CanvasRenderingContext2D} */
      context: null,
      lines: {
        horizontal: 3,
        vertical: 5,
      },
      position: {
        x: this.x,
        y: this.y
      }
    }
  },
  mounted() {
    this.context = this.$refs.canvas.getContext("2d");
    this.updateCanvas();
  },
  watch: {
    relative() { this.updateCanvas() },
    position: {
      handler() {
        this.updateCanvas()
      },
      deep: true,
    },
    'position.x'(newValue) {
      this.$emit('update:x', newValue);
    },
    'position.y'(newValue) {
      this.$emit('update:y', newValue);
    },
    x(newValue) {
      this.position.x = newValue;
    },
    y(newValue) {
      this.position.y = newValue;
    }
  },
  methods: {
    reset() {
      this.position = {x: 0, y: 0}
    },
    mouseMove(event) {
      if ((event.buttons & 1) == 0) return;
      this.updatePos(event.offsetX, event.offsetY);
    },
    canvasClick(event) {
      this.updatePos(event.offsetX, event.offsetY);
    },
    updatePos(x, y) {
      this.position = {
        x: Math.min(Math.max((((x / this.context.canvas.width) * 2) - 1).toFixed(3), -1), 1),
        y: Math.min(Math.max(((((y / this.context.canvas.height) * 2) - 1) * -1).toFixed(3), -1), 1),
      }
    },
    updateCanvas() {
      /** @type {CanvasRenderingContext2D} */
      let ctx = this.context;
      ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);

      for (let i = 1; i <= this.lines.vertical; i++) {
          ctx.beginPath();
          ctx.setLineDash([2, 5]);
          let pos = (ctx.canvas.width / (this.lines.vertical + 1)) * i;
          ctx.moveTo(pos, 0);
          ctx.lineTo(pos, ctx.canvas.height);
          ctx.strokeStyle = "#78edff55"
          ctx.stroke();
      }

      for (let i = 1; i <= this.lines.horizontal; i++) {
          ctx.beginPath();
          ctx.setLineDash([2, 5]);
          let pos = (ctx.canvas.height / (this.lines.horizontal + 1)) * i;
          ctx.moveTo(0, pos);
          ctx.lineTo(ctx.canvas.width, pos);
          ctx.strokeStyle = "#78edff55"
          ctx.stroke();
      }


      if (this.relative) {
        ctx.beginPath();
        ctx.arc(ctx.canvas.width / 2, ctx.canvas.height / 2, 5, 0, 6.18);
        ctx.fillStyle = "#78edff";
        ctx.fill();
      }

      let x = ((1 + parseFloat(this.position.x)) / 2.0) * ctx.canvas.width;
      let y = ((1 - parseFloat(this.position.y)) / 2.0) * ctx.canvas.height;
      ctx.beginPath();
      ctx.arc(x, y, 5, 0, 6.18);
      ctx.fillStyle = "white";
      ctx.fill()
    }
  },
}
</script>

<style>
#container {
  background-color: #3D3D3D;
  color: white;
}

#position {
  border: 1px solid white;
  box-sizing: border-box;
}

input[type=range] {
  padding: 0;
}

input[type=range][orient=vertical] {
    margin-top: -4px;
    writing-mode: bt-lr; /* IE */
    -webkit-appearance: slider-vertical; /* WebKit */
    width: 8px;
    height: 100%;
    padding: 0 6px;
}

.span-all {
  grid-column: 1 / -1;
}

#container {
  display: grid;
  grid-template-columns: min-content min-content;
  grid-template-rows: min-content min-content;
}

#reset {
  padding: 1px;
}
</style>