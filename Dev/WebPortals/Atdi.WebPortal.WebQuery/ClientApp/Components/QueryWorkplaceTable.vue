<template>
    <div class="card-content" style="overflow: auto;">
        <div v-if="showProgress" class="progress">
            <div class="indeterminate"></div>
        </div>
        <div class="" style="overflow: auto;">

            <table :id="id" class="bordered portal-data-table">
                <thead class="portal-table-head">
                    <tr>
                        <th v-for="column in query.columns" :key="column.name">{{column.title}}</th>
                    </tr>
                </thead>
                <tbody v-if="hasData">
                    <tr v-for="(row, index) in rows" :key="index" @click="onRowClick(row, index, $event)">
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

            processedRows: function() {
                const data = this.queryData;
                if (data)
                    return data.rows;

                return [];
            },
            rows: function() {
                const data = this.queryData;
                if (data)
                    return data.rows.slice((this.currentPage - 1) * this.currentPerPage, this.currentPerPage === -1 ? data.rows.length + 1 : this.currentPage * this.currentPerPage);

                return [];
            }
        },

        data() {
            return {
            }
        },

        methods: {
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
                const curRow = document.querySelector("tr.portal-data-table-row-selected");
                if (curRow)
                    curRow.classList.toggle("portal-data-table-row-selected");
                event.currentTarget.classList.toggle("portal-data-table-row-selected");
                this.$store.commit('queries/changeCurrentRow', { index: index, cells: row});
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

</style>
