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
        api.executeQuery({token: state.current.token}, data => {
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
        
        const pk = state.current.primaryKey;
        for (let index = 0; index < pk.length; index++) {
            const pkColumnName = pk[index];
            if (pkColumnName == null || pkColumnName === ""){
                console.warn("Undefined name of the primary key column with index #" + index);
            } else {
                const pkColumn = columnsMap[pkColumnName];
                if (!pkColumn){
                    console.warn("Not found primary key column with name '" + pkColumnName + "'");
                }
            }
            
        }
        
        const result = {
            columnsMap: columnsMap,
            rows: data.Dataset.Cells,
            currentPage: 1,
            currentPerPage: 10,
            currentRow: {
                index: -1,
                cells: []
            },
            filter: {
                columns: [],
                value: null,
                operation: 'contains'
            },
            sorting: {
                column: null,
                direction: 0
            },
            sortColumns: [] // [{name, index, direction},...]
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
        if (data.currentRow && data.currentRow.cells){
            data.currentRow.cells.isSelected = false;
        }
        data.currentRow = row;
        if (data.currentRow && data.currentRow.cells){
            data.currentRow.cells.isSelected = true;
        }
    },

    changeColumnSorting(state, columnName) {
        const data = state.data["q_" + state.current.token.id];
        
        const sorting = data.sortColumns.find(column => column.name === columnName);
        if (sorting) {
            if (sorting.direction === 1) {
                sorting.direction = -1
                data.sortColumns = [...data.sortColumns];
            } else {
                const newSortColumns = data.sortColumns.filter(column => column.name !== columnName)
                data.sortColumns = newSortColumns;
            }
        } else {
            // need to add
            const newSortColumns = [...data.sortColumns]; 
            newSortColumns.push({
                name: columnName,
                index: data.columnsMap[columnName].Index,
                direction: 1
            });
            data.sortColumns = newSortColumns;
        }
    },

    changeCurrentFilter(state, filter) {
        const data = state.data["q_" + state.current.token.id];
        data.filter = filter;
    }
}

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
}