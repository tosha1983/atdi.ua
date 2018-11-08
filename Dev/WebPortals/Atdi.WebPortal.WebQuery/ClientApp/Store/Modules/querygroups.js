import api from '../../Api/portal'
import Vue from 'vue'

const state = {
    all: [],
    current: null,
    queries: {}
}

const getters = {
    getQueriesByGroup: state => name => {
        if (state.queries[name] === undefined) {
            return { list: [] };
        }

        return state.queries[name];
    }
}

const actions = {
    loadGroups({ commit }) {
        api.getQueryGroups(groups => {
            commit('setGroups', groups);
        });
    },

    changeCurrentGroup({ commit }, name) {
        commit('changeCurrent', name);

        const queries = state.queries[name];
        if (queries && queries.state === 'created') {
            api.getQueriesByTokens(queries.group.queryTokens, data => {
                commit('setQueries', { group: name, queries: data });
            });
        }
    }
}

const mutations = {
    setGroups(state, groups) {
        state.all = groups

        for (var i = 0; i < groups.length; i++) {
            Vue.set(state.queries, groups[i].name, {
                group: groups[i],
                state: 'created',
                list: [{ title: 'Loading (for ' + groups[i].name + ') ...', token: { id: -1} }]
            });
        }
    },

    changeCurrent(state, name) {
        state.current = state.all.find(group => group.name === name);
    },

    setQueries(state, { group, queries }) {
        const g = state.queries[group];
        g.list.pop();
        if (queries && queries.length > 0) {
            g.state = 'loaded';
            g.list.push(...queries);
        }
        else {
            g.state = 'empty'
        }
    },

    
}

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
}