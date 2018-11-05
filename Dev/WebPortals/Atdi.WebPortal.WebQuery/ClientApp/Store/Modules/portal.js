import portalApi from '../../Api/portal'

const state = {
    environment: portalApi.declareEnvironment()
}

const getters = {
}

const actions = {
    defineEnvironment ({ commit }) {
        portalApi.getEnvironment(portalEnvironment => {
            commit('setEnvironment', portalEnvironment)
        })
    }
}

const mutations = {
    setEnvironment(state, portalEnvironment) {
        state.environment = portalEnvironment
    }
}

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
}