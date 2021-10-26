import { createApp } from 'vue'
import StreamDeck from '../../utils/streamdeck';
import App from './App.vue'
import { createStore } from 'vuex';

window.connectElgatoStreamDeckSocket = function (inPort, inPluginUUID, inRegisterEvent, inInfo, inActionInfo) {
    let streamDeck = new StreamDeck(inPort, inPluginUUID, inRegisterEvent, inInfo, inActionInfo);
    console.log("Connected to stream deck")

    let store = createStore({
        state: {
            streamDeck,
        },
        mutations: {
        },
        actions: {
        },
        modules: {
        }
    })

    createApp(App).use(store).mount('#app')
}



