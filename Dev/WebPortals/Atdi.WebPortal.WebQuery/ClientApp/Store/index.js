import Vue from 'vue'
import Vuex from 'vuex'
import portal from './Modules/portal'
import queryGroups from './Modules/querygroups'
import queries from './Modules/queries'

Vue.use(Vuex)

const debug = process.env.NODE_ENV !== 'production'

export default new Vuex.Store({
    modules: {
        portal,
        queryGroups,
        queries
    },
    strict: debug,
    plugins: debug ? [] : []
})