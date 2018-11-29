<template>
    <div :id="id" class="card-action portal-table-toolbar">
        <a v-if="allowedFetch" class="waves-effect waves-light" @click="fetchData" href="javascript:undefined">Fetch</a>
        <a v-if="allowedAdd" class="waves-effect waves-light" @click="openAddForm" href="javascript:undefined">Add</a>
        <a v-if="allowedView" class="waves-effect waves-light" @click="openViewForm" href="javascript:undefined">View</a>
        <a v-if="allowedEdit" class="waves-effect waves-light" @click="openEditForm" href="javascript:undefined">Edit</a>
        <a v-if="allowedDel" class="waves-effect waves-light"  @click="openDeleteForm" href="javascript:undefined">Delete</a>
        <a @click="exportExcel" class="waves-effect waves-light" href="javascript:undefined">Excel</a>

        <search-tool 
            v-if="allowedSearch"
            :value.sync="searchValue"
            >
        </search-tool>
        
        
        <div id="portal-table-toolbar-table-tools" class="fixed-action-btn horizontal direction-top direction-left" style="position: absolute; display: inline-block; right: 24px;">
            <a class="btn-floating btn-large tooltipped" data-position="bottom" data-tooltip="Table tool bar">
                <i class="large material-icons">menu</i>
            </a>
            <ul>
                <li>
                <a class="btn-floating red tooltipped" @click="showSortingTuneForm" data-position="bottom" data-tooltip="Tune sorting columns" style="opacity: 0; transform: scale(0.4) translateY(0px) translateX(40px);">
                    <i class="material-icons">format_line_spacing</i>
                </a>
                </li>
                <li>
                <a class="btn-floating yellow darken-1 tooltipped" @click="showColumnsTuneForm" data-position="bottom" data-tooltip="Tune columns positions" style="opacity: 0; transform: scale(0.4) translateY(0px) translateX(40px);">
                    <i class="material-icons">format_list_bulleted</i>
                </a>
                </li>
                <li>
                <a class="btn-floating green tooltipped" data-position="bottom" @click="showConditionsTuneForm" data-tooltip="Tune data filters" style="opacity: 0; transform: scale(0.4) translateY(0px) translateX(40px);">
                    <i class="material-icons">filter_b_and_w</i>
                </a>
                </li>
                <li>
                <a class="btn-floating blue tooltipped" data-position="bottom" @click="printTable" data-tooltip="Print current state" style="opacity: 0; transform: scale(0.4) translateY(0px) translateX(40px);">
                    <i class="material-icons">local_printshop</i>
                </a>
                </li>
            </ul>
        </div>
      
        <template v-if="needSortingTuneForm">
            <table-tool-form id="portal-table-tool-form-sorting" @close="onCloseSortingTuneForm" @apply="onApplySortingTuneForm">
                <template slot="title">Tune sorting</template>
                <template slot="content">
                    <table-sorting-tune-content id="portal-table-sorting-tune-content"></table-sorting-tune-content>
                </template>
            </table-tool-form>
        </template>
        
        <template v-if="needColumnsTuneForm">
            <table-tool-form id="portal-table-tool-form-columns" @close="onCloseColumnsTuneForm" @apply="onApplyColumnsTuneForm">
                <template slot="title">Tune columns</template>
                <template slot="content">
                    <p>Columns ...</p>
                </template>
            </table-tool-form>
        </template>

        <template v-if="needConditionsTuneForm">
            <table-tool-form id="portal-table-tool-form-conditions" @close="onCloseConditionsTuneForm" @apply="onApplyConditionsTuneForm">
                <template slot="title">Tune conditions</template>
                <template slot="content">
                    <p>Conditions ...</p>
                </template>
            </table-tool-form>
        </template>

    </div>
</template>
<script>
    import { mapState, mapActions } from 'vuex'
    import utilities from './../Utilities';
    import SearchTool from './SearchTool.vue'
    import TableToolForm from './TableToolForm.vue'
    import TableSortingTuneContent from './TableSortingTuneContent.vue'

    export default {
        name: 'QueryWorkplaceToolbar',

        props: {
            id: String,
            group: Object,
            query: Object
        },

        components: {
            SearchTool,
            TableToolForm,
            TableSortingTuneContent
        },

        computed: { 
            ...mapState({
                queryData: state => {
                    if (typeof state.queries.data["q_" + state.queries.current.token.id] !== "undefined")
                        return state.queries.data["q_" + state.queries.current.token.id];
                    return null;
                },
            }),
            
            allowedFetch: function () {
                    return true;
                },
            
            allowedAdd: function () {
                    return this.group.canCreate;
                },
            
            allowedView: function (){
                    return this.$store.getters['queries/currentRow'].index !== -1
                },
            
            allowedEdit: function (){
                    return this.group.canModify && this.$store.getters['queries/currentRow'].index !== -1
                },

            allowedDel: function (){
                    return this.group.canDelete && this.$store.getters['queries/currentRow'].index !== -1
                },

            allowedSearch: function (){
                    const data = this.queryData;
                    if (data)
                        return data.rows.length > 0;
                    
                    return false;
                },

            currentRow: function () {
                    return this.$store.getters['queries/currentRow'];
                },

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

            searchValue: {
                get: function() {
                    const data = this.queryData;
                    if (data){
                        return data.filter.value;
                    }

                    return null;
                },

                set: function(value) {
                    this.$store.commit('queries/changeCurrentFilter', { columns: [], operation: 'contains', value: value});
                }
            }
        },

        data() {
            return {
               needSortingTuneForm: false,
               needColumnsTuneForm: false,
               needConditionsTuneForm: false
            }
        },

        methods: {
            showSortingTuneForm: function() {
                this.needSortingTuneForm = true;
                this.$nextTick(function () {
                    var formElement = document.querySelector('#portal-table-tool-form-sorting');
                    var instance = M.Modal.getInstance(formElement);
                    instance.open();
                });
            },
            onCloseSortingTuneForm: function () {
                var formElement = document.querySelector('#portal-table-tool-form-sorting');
                var instance = M.Modal.getInstance(formElement);
                instance.close();
                this.needSortingTuneForm = false;
            },

            onApplySortingTuneForm: function (options) {
                this.onCloseSortingTuneForm();
            },

            showColumnsTuneForm: function() {
                this.needColumnsTuneForm = true;
                this.$nextTick(function () {
                    var formElement = document.querySelector('#portal-table-tool-form-columns');
                    var instance = M.Modal.getInstance(formElement);
                    instance.open();
                });
            },
            onCloseColumnsTuneForm: function () {
                var formElement = document.querySelector('#portal-table-tool-form-columns');
                var instance = M.Modal.getInstance(formElement);
                instance.close();
                this.needColumnsTuneForm = false;
            },
            
            onApplyColumnsTuneForm: function (options) {
                this.onCloseColumnsTuneForm();
            },

            showConditionsTuneForm: function() {
                this.needConditionsTuneForm = true;
                this.$nextTick(function () {
                    var formElement = document.querySelector('#portal-table-tool-form-conditions');
                    var instance = M.Modal.getInstance(formElement);
                    instance.open();
                });
            },
            onCloseConditionsTuneForm: function () {
                var formElement = document.querySelector('#portal-table-tool-form-conditions');
                var instance = M.Modal.getInstance(formElement);
                instance.close();
                this.needConditionsTuneForm = false;
            },
            onApplyConditionsTuneForm: function (options) {
                this.onCloseConditionsTuneForm();
            },

            fetchData: function () {
                this.$store.dispatch('queries/executeCurrentQuery');
            },

            getValue(row, column) {
                const data = this.queryData;
                const realColumn = data.columnsMap[column.name];
                return row[realColumn.Index];
            },

            exportExcel: function () {
                const mimeType = 'data:application/vnd.ms-excel';
                const html = this.renderTable().replace(/ /g, '%20');
                const documentPrefix = this.query.title != '' ? this.query.title.replace(/ /g, '-') : 'Sheet'
                const d = new Date();
                var dummy = document.createElement('a');
                dummy.href = mimeType + ', ' + html;
                dummy.download = documentPrefix
                    + '-' + d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate()
                    + '-' + d.getHours() + '-' + d.getMinutes() + '-' + d.getSeconds()
                    + '.xls';
                document.body.appendChild(dummy);
                dummy.click();
            },

            printTable: function () {
                const self = this;
                setTimeout(function() {
                    let win = window.open("");
                    win.document.write(self.renderTable());
                    win.print();
                    win.close();
                }, 100);
                
            },

            renderTable: function () {
                var table = '<table><thead>';
                table += '<tr>';
                for (var i = 0; i < this.query.columns.length; i++) {
                    const column = this.query.columns[i];
                    table += '<th>';
                    table += column.title;
                    table += '</th>';
                }
                table += '</tr>';
                table += '</thead><tbody>';
                for (var i = 0; i < this.processedRows.length; i++) {
                    const row = this.processedRows[i];
                    table += '<tr>';
                    for (var j = 0; j < this.query.columns.length; j++) {
                        const column = this.query.columns[j];
                        table += '<td>';
                        table += this.getValue(row, column);
                        table += '</td>';
                    }
                    table += '</tr>';
                }
                table += '</tbody></table>';
                return table;
            },

            openViewForm: function () {
                this.$emit('openViewForm');
            },
            openAddForm: function () {
                this.$emit('openAddForm');
            },
            openEditForm: function () {
                this.$emit('openEditForm');
            },
            openDeleteForm: function () {
                this.$emit('openDeleteForm');
            }
        },


        mounted() {
            const fabElement = document.getElementById('portal-table-toolbar-table-tools');
            var fabInstance = M.FloatingActionButton.init(fabElement, {
                direction: 'left'
            });
            const tooltipElements = document.querySelectorAll('.tooltipped');
            M.Tooltip.init(tooltipElements);
        },

        watch: {
           
        }
    }
</script>
<style>


.portal-table-toolbar {
    display: flex;
    align-items: center;
}
 
</style>
