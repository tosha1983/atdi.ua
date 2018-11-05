import webapi from '../../Api/webapi'
import Vue from 'vue'

const state = {
    all: [],
    current: {
        name: '',
        title: '',
        description: ''
    },
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
        webapi.call(webapi.SVC_QUERYGROUPS, data => {
            if (data && data.groups) {
                commit('setGroups', data.groups);
            } else {
                commit('setGroups', []);
            }
            
        })
    },

    changeCurrentGroup({ commit }, name) {
        commit('changeCurrent', name);

        const queries = state.queries[name];
        if (queries && queries.state === 'empty') {
            webapi.post(webapi.SVC_WEBQUERIES, 'get', { tokens: queries.group.queryTokens }, data => {
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
                state: 'empty',
                list: [{ title: 'Loading (for ' + groups[i].name + ') ...' }]
            });
        }
    },

    changeCurrent(state, name) {
        state.current = state.all.find(group => group.name === name);
    },

    setQueries(state, { group, queries }) {
        const g = state.queries[group];
        g.state = 'loaded';
        g.list.pop();
        g.list.push(...queries);
    },

    
}

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
}