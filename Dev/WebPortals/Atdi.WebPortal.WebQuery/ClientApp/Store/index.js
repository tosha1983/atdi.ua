import Vue from 'vue'
import Vuex from 'vuex'
import portal from './Modules/portal'
import queryGroups from './Modules/querygroups'

Vue.use(Vuex)

const debug = process.env.NODE_ENV !== 'production'

export default new Vuex.Store({
    modules: {
        portal,
        queryGroups
    },
    strict: debug,
    plugins: debug ? [] : []
})