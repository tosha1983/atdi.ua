import Vue from 'vue'
import App from './App.vue'
import store from './Store'

new Vue({
    el: 'body',
    store,
    render: h => h(App)
});