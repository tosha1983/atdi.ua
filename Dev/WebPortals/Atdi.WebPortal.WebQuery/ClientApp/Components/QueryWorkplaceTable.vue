<template>
    <div class="card-content" style="overflow: auto; padding-top: 0px;">
        <div v-visible="showProgress" class="progress" style="margin-top: 0px; margin-bottom: 3px;">
            <div class="indeterminate"></div>
        </div>
        <div class="" style="overflow: auto;">

            <table :id="id" class="bordered portal-data-table">
                <thead class="portal-table-head">
                    <tr>
                        <th v-for="column in query.columns" :key="column.name" @click="onColumnClick(column.name)">
                            <div class="portal-data-table-column waves-effect" layout="row center-justify">
                                <div class="waves-effect">{{column.title}}</div>
                                <div :class="getSortingState(column) === 0 ? 'waves-effect' : 'waves-effect portal-data-table-sort-arrow ' + ((getSortingState(column) === 1) ? 'portal-data-table-sort-arrow-asc': 'portal-data-table-sort-arrow-desc')"></div>
                            </div>
                        </th>
                    </tr>
                </thead>
                <tbody v-if="hasData">
                    <tr 
                        v-for="(row, index) in rows" 
                        :key="index" 
                        @click="onRowClick(row, index, $event)"
                        :class=" (row && row.isSelected) ? 'portal-data-table-row-selected' : ''"
                    >
                        <td v-for="column in query.columns" :key="column.name">{{getValue(row, column)}}</td>
                    </tr>

                </tbody>
            
            </table>
        </div>
        <nav v-if="hasData" style="height: 44px; line-height: 44px;" class="portal-slidebar-nav">
            <div class="nav-wrapper">
                <ul class="right">
                    <li>
                        <span style="margin-right: 15px;">Rows per page:</span>
                    </li>
                    <li>
                        <select class="browser-default rows-per-pages" @change="changePerPage">
                            <option v-for="(option, index) in perPageOptions" :value="option" :selected="option == currentPerPage" :key="index">
                            {{ option === -1 ? 'all' : option }}
                            </option>
                        </select>
                    </li>
                    <li>
                        <a href="javascript:undefined" class="waves-effect btn-flat portal-table-btn" @click.prevent="firstPage" tabindex="0">
                            <i class="material-icons">first_page</i>
                        </a>
                    </li>
                    <li>
                        <a href="javascript:undefined" class="waves-effect portal-table-btn btn-flat" @click.prevent="previousPage" tabindex="0">
                            <i class="material-icons">chevron_left</i>
                        </a>
                    </li>
                    <li>
                        <div class="">
                        {{(currentPage - 1) * currentPerPage ? (currentPage - 1) * currentPerPage : 1}}-{{Math.min(processedRows.length, currentPerPage * currentPage)}} out of pages {{processedRows.length}}
                        </div>
                    </li>   
                    <li>
                        <a href="javascript:undefined" class="waves-effect portal-table-btn btn-flat" @click.prevent="nextPage" tabindex="0">
                            <i class="material-icons">chevron_right</i>
                        </a>
                    </li>
                    <li>
                        <a href="javascript:undefined" class="waves-effect portal-table-btn btn-flat" @click.prevent="lastPage" tabindex="0">
                            <i class="material-icons">last_page</i>
                        </a>
                    </li>
                </ul>
            </div>
        </nav>
        
    </div>
</template>
<script>
    import { mapState, mapActions } from 'vuex'
    import utilities from './../Utilities';

    export default {
        name: 'QueryWorkplaceTable',

        props: {
            id: String,
            query: Object
        },

        components: {
        },

        computed: { 
            ...mapState({
                currentPage: state => state.queries.data["q_" + state.queries.current.token.id].currentPage,
                currentPerPage: state => state.queries.data["q_" + state.queries.current.token.id].currentPerPage,
                perPageOptions: state => [10, 20, 50, 100],
                
                showProgress: state => state.queries.showProgress,
                hasData: state => typeof state.queries.data["q_" + state.queries.current.token.id] !== "undefined",
                queryData: state => {
                    if (typeof state.queries.data["q_" + state.queries.current.token.id] !== "undefined")
                        return state.queries.data["q_" + state.queries.current.token.id];
                    return null;
                },
                
            }),

            filteredRows: function () {
                const data = this.queryData;
                if (!data)
                    return [];

                let result = data.rows;

                if (data.filter && data.filter.value) {
                    result = utilities.applyFilter(result, data.filter.value)
                }
                return result;
            },
            processedRows: function () {
                const data = this.queryData;
                let result = this.filteredRows;
                if (data && data.sorting && data.sorting.column) {
                    const realColumn = data.columnsMap[data.sorting.column];
                    result = utilities.applySorting(result, (cells) => cells[realColumn.Index], data.sorting.direction)
                }
                return result;
            },
            rows: function() {
                const data = this.queryData;
                if (data)
                    return this.processedRows.slice((this.currentPage - 1) * this.currentPerPage, this.currentPerPage === -1 ? data.rows.length + 1 : this.currentPage * this.currentPerPage);

                return [];
            }
        },

        data() {
            return {
            }
        },

        watch: {
            rows: function(){
                this.hideRowSelection();
            }
        },

        methods: {
            getSortingState(column) {
                const data = this.queryData;
                if (data && data.sorting && data.sorting.column === column.name) {
                    return data.sorting.direction;
                }
                return 0;
            },
            getValue(row, column) {
                const data = this.queryData;
                const realColumn = data.columnsMap[column.name];
                return row[realColumn.Index];
            },
            firstPage: function() {
				this.$store.commit('queries/firstPage');
            },
            lastPage: function() {
				this.$store.commit('queries/lastPage');
			},
            nextPage: function() {
				this.$store.commit('queries/nextPage');
			},
			previousPage: function() {
				this.$store.commit('queries/previousPage');
			},
			changePerPage: function(event) {
                this.$store.commit('queries/changePerPage', parseInt(event.target.value));
            },
            onRowClick: function(row, index, event) {
                this.hideRowSelection();
                event.currentTarget.classList.toggle("portal-data-table-row-selected");
                this.$store.commit('queries/changeCurrentRow', { index: index, cells: row});
            },
            hideRowSelection: function() {
                const curRow = document.querySelector("tr.portal-data-table-row-selected");
                if (curRow)
                    curRow.classList.toggle("portal-data-table-row-selected");
            },
            onColumnClick: function (columnName) {
                const data = this.queryData;
                let newSorting = null;
                if (data.sorting.column === columnName) {
                    if (data.sorting.direction === 1) {
                        newSorting = {
                            column: columnName,
                            direction: -1
                        };
                    } else {
                        newSorting = {
                            column: null,
                            direction: 0
                        };
                    }
                } else {
                    newSorting = {
                        column: columnName,
                        direction: 1
                    };
                }

                this.$store.commit('queries/toShowProgress');
                const self = this;

                this.$nextTick(function () {
                    setTimeout(function() {
                        self.$store.commit('queries/changeCurrentSorting', newSorting);
                        self.$store.commit('queries/toHideProgress');
                    }, 300);
                });
                
                
                
            }
        }
    }
</script>

<style>
.footer-wrapper {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0 18px;
}

nav ul a.portal-table-btn{ 
    margin-left: 2px;
    padding-left: 0px;
    padding-right: 0px;
    margin-right: 2px;
        color: #fff;
}

select.rows-per-pages {
    margin-top: 3px;
    height: 39px;
}

thead.portal-table-head {
    background: #09678cd1;
}

table.portal-data-table tbody tr:hover {
    background: #327f9d33;
}
table.portal-data-table tbody tr.portal-data-table-row-selected {
    background: #327f9d94;
}
table.portal-data-table th, table.portal-data-table td  {
    border-radius: 0px;
}

table.portal-data-table th:hover  {
    background: #85c9e494;
}

.portal-data-table-column {
    display: flex;
    padding: .75rem 1rem;
    cursor: pointer;
    -webkit-user-select: none;
    -moz-user-select: none;
    -ms-user-select: none;
    user-select: none;
}

.portal-data-table-sort-arrow {
    margin-left: 10px;
    margin-top: 5px;
    display: flex;
    width: 0;
    height: 0;
    border: .375rem solid transparent;
        border-top-color: transparent;
        border-top-style: solid;
        border-top-width: 0.375rem;
        border-right-color: transparent;
        border-right-style: solid;
        border-right-width: 0.375rem;
        border-bottom-color: transparent;
        border-bottom-style: solid;
        border-bottom-width: 0.375rem;
        border-left-color: transparent;
        border-left-style: solid;
        border-left-width: 0.375rem;
        border-image-outset: 0;
        border-image-repeat: stretch;
        border-image-slice: 100%;
        border-image-source: none;
        border-image-width: 1;
}

.portal-data-table-sort-arrow-asc {
    border-bottom-color: currentColor;
    -webkit-transform: translateY(-.1875rem);
    transform: translateY(-.1875rem);
}

.portal-data-table-sort-arrow-desc {
    border-top-color: currentColor;
    -webkit-transform: translateY(.1875rem);
    transform: translateY(.1875rem);
}

</style>
