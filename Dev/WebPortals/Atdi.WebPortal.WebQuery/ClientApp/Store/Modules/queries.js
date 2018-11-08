import api from '../../Api/portal'
import Vue from 'vue'

const state = {
    current: null,
    data: {},
    showProgress: false
}

const getters = {
    hasCurrent: () => state.current && state.current !== null && state.current !== undefined,
    currentRow: function () {
        if (!state.current || state.current === null) {
            return {
                index: -1,
                cells: []
            };
        }

        const data = state.data["q_" + state.current.token.id];
        if (data){
            return data.currentRow;
        }

        return {
            index: -1,
            cells: []
        };
    },

    hasCurrentQueryData: () => state.current && state.current !== null && state.data["q_" + state.current.token.id],
    
    currentQueryData: function () {
        if (!state.current || state.current === null){
            return null;
        }
        return state.data["q_" + state.current.token.id];
    }
}

const actions = {
    changeCurrentQuery({ commit }, query) {
        commit('changeCurrent', query);
    },

    executeCurrentQuery({ commit }, options) {
        commit('toShowProgress');
        api.executeQuery(state.current.token, data => {
            commit('setQueryData', { data: data });
            commit('toHideProgress');
        });
    }
}

const mutations = {
 
    changeCurrent(state, query) {

        state.current = query; 
    },

    setQueryData(state, { data }) {

        const columns = data.Dataset.Columns;
        const columnsMap = { };

        for (let index = 0; index < columns.length; index++) {
            const column = columns[index];
            columnsMap[column.Name] = column;
        }
        
        const result = {
            columnsMap: columnsMap,
            rows: data.Dataset.Cells,
            currentPage: 1,
            currentPerPage: 10,
            currentRow: {
                index: -1,
                cells: []
            }
        };
        
        Vue.set(state.data, "q_" + state.current.token.id, result);
    },

    toShowProgress(state) {
        state.showProgress = true ;
    },
    toHideProgress(state) {
        state.showProgress = false ;
    },

    firstPage(state) {
        const data = state.data["q_" + state.current.token.id];
        if (data.currentPage !== 1)
            data.currentPage = 1;
    },

    lastPage(state) {
        const data = state.data["q_" + state.current.token.id];
        data.currentPage = data.rows.length/data.currentPerPage;
    },

    nextPage(state) {
        const data = state.data["q_" + state.current.token.id];
        if (data.rows.length > data.currentPerPage * data.currentPage)
            ++data.currentPage;
    },

    previousPage(state) {
        const data = state.data["q_" + state.current.token.id];
        if (data.currentPage > 1)
            --data.currentPage;
    },
    changePerPage(state, perPage) {
        const data = state.data["q_" + state.current.token.id];
        data.currentPage = 1;
        data.currentPerPage = perPage;
    },

    changeCurrentRow(state, row) {
        const data = state.data["q_" + state.current.token.id];
        data.currentRow = row;
    },
}

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
}