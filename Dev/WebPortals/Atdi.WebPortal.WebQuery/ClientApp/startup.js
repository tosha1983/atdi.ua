import Vue from 'vue'
import App from './App.vue'
import store from './Store'
import router from './Router'
import directives from './Directives'

Vue.use(directives);

new Vue({
    el: 'body',
    store,
    router,
    render: h => h(App)
});