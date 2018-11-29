<template>
    <div class="portal-query-workplace">

        <div class="row"></div>
        <div class="row">
            <div v-show="hasCurrentGroup" class="col s6">
                <ul class="collapsible">
                    <li v-if="hasCurrentGroup" >
                        <div class="collapsible-header"><i class="material-icons">collections</i><span><b>Group:</b> {{currentGroup.title}}</span></div>
                        <div class="collapsible-body"><span>{{currentGroup.description}}</span></div>
                    </li>
                    <li v-if="hasCurrentQuery">
                        <div class="collapsible-header"><i class="material-icons">landscape</i><span><b>Query:</b> {{currentQuery.title}}</span></div>
                        <div class="collapsible-body"><plaintext>{{currentQuery.description}}</plaintext></div>
                    </li>
                </ul>
            </div>
        </div>

        <div v-if="hasCurrentGroup && hasCurrentQuery" class="card s12">
            <query-workplace-toolbar 
                id="query-result-toolbar" 
                :group="currentGroup" 
                :query="currentQuery" 
                @openViewForm="onOpenViewForm"
                @openAddForm="onOpenAddForm"
                @openEditForm="onOpenEditForm"
                @openDeleteForm="onOpenDeleteForm"
                ></query-workplace-toolbar>

            <query-workplace-table id="query-result-table" :query="currentQuery"></query-workplace-table>
            
            <template v-if="needViewForm">
                <query-workplace-view-form 
                    id="form-view-record" 
                    @closeViewForm="onCloseViewForm" 
                    :row="viewRowData" 
                    :query="currentQuery"
                ></query-workplace-view-form>
            </template>

            <template v-if="needAddForm">
                <query-workplace-add-form 
                    id="form-add-record" 
                    @closeAddForm="onCloseAddForm" 
                    @saveAddForm="onSaveAddForm" 
                    :row="addRowData" 
                    :query="currentQuery"
                    :error-state="addErrorState"
                ></query-workplace-add-form>
            </template>

            <template v-if="needEditForm">
                <query-workplace-edit-form 
                    id="form-edit-record" 
                    @closeEditForm="onCloseEditForm" 
                    @saveEditForm="onSaveEditForm" 
                    :row="editRowData" 
                    :query="currentQuery"
                    :error-state="editErrorState"
                ></query-workplace-edit-form>
            </template>

            <template v-if="needDeleteForm">
                <query-workplace-delete-form 
                    id="form-delete-record" 
                    @closeDeleteForm="onCloseDeleteForm"
                    @saveDeleteForm="onSaveDeleteForm"  
                    :row="viewRowData" 
                    :query="currentQuery"
                ></query-workplace-delete-form>
            </template>

        </div>
    </div>
</template>
<script>
    import api from './../Api/portal'
    import { mapState, mapActions } from 'vuex'
    import QueryWorkplaceToolbar from './QueryWorkplaceToolbar.vue'
    import QueryWorkplaceTable from './QueryWorkplaceTable.vue'
    import QueryWorkplaceViewForm from './QueryWorkplaceViewForm.vue'
    import QueryWorkplaceEditForm from './QueryWorkplaceEditForm.vue'
    import QueryWorkplaceAddForm from './QueryWorkplaceAddForm.vue'
    import QueryWorkplaceDeleteForm from './QueryWorkplaceDeleteForm.vue'

    export default {
        name: 'QueryWorkplace',
        props: {
        },
        components: {
            QueryWorkplaceToolbar,
            QueryWorkplaceTable,
            QueryWorkplaceViewForm,
            QueryWorkplaceEditForm,
            QueryWorkplaceAddForm,
            QueryWorkplaceDeleteForm
        },

        computed: {
            ...mapState({
                currentGroup: state => state.queryGroups.current,
                hasCurrentGroup: state => state.queryGroups.current && state.queryGroups.current !== null && state.queryGroups.current !== undefined,
                currentQuery: state => state.queries.current,
                hasCurrentQuery: state => state.queries.current && state.queries.current !== null && state.queries.current !== undefined,
                
                editFormColumns: state => {
                    const current = state.queries.current;
                    if (!current){
                        return [];
                    }
                    if (current.ui && current.ui.EditFormColumns) {
                        const columns = current.ui.EditFormColumns;
                        if ( columns.length > 0){
                            return columns;
                        }
                    }
                    return current.columns.map(column => column.name);
                },
                
                viewFormColumns: state => {
                    const current = state.queries.current;
                    if (!current){
                        return [];
                    }
                    if (current.ui && current.ui.ViewFormColumns) {
                        const columns = current.ui.ViewFormColumns;
                        if ( columns.length > 0){
                            return columns;
                        }
                    }
                    return current.columns.map(column => column.name);
                },

                addFormColumns: state => {
                    const current = state.queries.current;
                    if (!current){
                        return [];
                    }
                    if (current.ui && current.ui.AddFormColumns) {
                        const columns = current.ui.AddFormColumns;
                        if ( columns.length > 0){
                            return columns;
                        }
                    }
                    return current.columns.map(column => column.name);
                }

            }),
            currentRow: function () {
                return this.$store.getters['queries/currentRow'];
            },
            currentQueryData: function () {
                return this.$store.getters['queries/currentQueryData'];
            },
        },

        data() {
            return {
                needViewForm: false,
                needAddForm: false,
                needEditForm: false,
                needDeleteForm: false,

                viewRowData: null,
                addRowData: null,
                editRowData: null,

                addErrorState: { has: false, message: null },
                editErrorState: { has: false, message: null },
                deleteErrorState: { has: false, message: null }
            }
        },

        methods: {
            onOpenViewForm: function () {
                if (!this.currentRow){
                    return;
                }

                const keyValues = [];
                const pkCondition = this.preparePrimaryKeyCondition(keyValues);
                const viewColumns = this.viewFormColumns;

                const executeOptions = {
                    token: this.currentQuery.token,
                    columns: viewColumns,
                    filter: pkCondition
                };

                const self = this;
                api.executeQuery(executeOptions, data => {
                    if (data.Dataset.RowCount !== 1){
                        return;
                    }

                    const rowData = self.prepareRowData(data.Dataset, viewColumns);
                    rowData.key = keyValues;
                    rowData.cells = data.Dataset.Cells[0];
                    
                    self.viewRowData = rowData;
                    self.needViewForm = true;
                    self.$nextTick(function () {
                        var elem = document.querySelector('#form-view-record');
                        var instance = M.Modal.getInstance(elem);
                        instance.open();
                    });
                });
            },

            onCloseViewForm: function () {
                var elem = document.querySelector('#form-view-record');
                var instance = M.Modal.getInstance(elem);
                instance.close();
                this.needViewForm = false;
                this.viewRowData = null;
            },

            preparePrimaryKeyCondition: function (keyValues) {
                const pkCondition = {
                    Type: 'Complex',
                    Filters: [],
                    FilterOperator: 'And'
                }
                
                const pk = this.currentQuery.primaryKey;
                const columnsMap = this.currentQueryData.columnsMap;
                const cells = this.currentRow.cells;

                for (let index = 0; index < pk.length; index++) {
                    const pkColumn = columnsMap[pk[index]];
                    const pkValue = cells[pkColumn.Index];
                    keyValues.push({column: pkColumn, value: pkValue});

                    const pkFilter = {
                        Type: 'Expression',
                        LeftOperand: {
                            Type: 'Column',
                            ColumnName: pkColumn.Name,
                        },
                        Operator: 'Equal',
                        RightOperand: {
                            Type: 'Value',
                            DataType: pkColumn.Type,
                            Value: pkValue
                        }    
                    };
                    pkCondition.Filters.push(pkFilter);
                } 

                return pkCondition;
            },

            createPrimaryKeyCondition: function (key) {
                const pkCondition = {
                    Type: 'Complex',
                    Filters: [],
                    FilterOperator: 'And'
                }

                for (let index = 0; index < key.length; index++) {
                    const keyData = key[index];
                    const pkColumn = keyData.column;
                    const pkValue = keyData.value;

                    const pkFilter = {
                        Type: 'Expression',
                        LeftOperand: {
                            Type: 'Column',
                            ColumnName: pkColumn.Name,
                        },
                        Operator: 'Equal',
                        RightOperand: {
                            Type: 'Value',
                            DataType: pkColumn.Type,
                            Value: pkValue
                        }    
                    };
                    pkCondition.Filters.push(pkFilter);
                } 

                return pkCondition;
            },

            prepareRowData: function(dataset, columns) {
                const rowData = {
                    columns: [],
                    map: {}
                };

                const columnsMetadata = this.currentQuery.columns;
                for (let index = 0; index < columns.length; index++) {
                    const name = columns[index];
                    const columnMetadata = columnsMetadata.find(column => column.name === name);
                    if (columnMetadata) {
                        rowData.columns.push(columnMetadata);
                    }
                }
                for (let index = 0; index < dataset.Columns.length; index++) {
                    const datasetColumn = dataset.Columns[index];
                    rowData.map[datasetColumn.Name] = datasetColumn;
                }
                return rowData;
            },

            prepareAddRowData: function(columns) {
                const rowData = {
                    columns: [],
                    map: {},
                    cells: []
                };

                const columnsMetadata = this.currentQuery.columns;
                for (let index = 0; index < columns.length; index++) {
                    const name = columns[index];
                    const columnMetadata = columnsMetadata.find(column => column.name === name);
                    if (columnMetadata) {
                        rowData.columns.push(columnMetadata);
                        rowData.map[name] = {
                            Index: index,
                            Name: name,
                            Type: columnMetadata.type
                        };
                        rowData.cells.push(null);
                    }
                }

                return rowData;
            },

            onOpenEditForm: function () {
                if (!this.currentRow){
                    return;
                }

                const keyValues = [];
                const pkCondition = this.preparePrimaryKeyCondition(keyValues);
                const editColumns = this.editFormColumns;

                const executeOptions = {
                    token: this.currentQuery.token,
                    columns: editColumns,
                    filter: pkCondition
                };

                const self = this;
                api.executeQuery(executeOptions, data => {
                    if (data.Dataset.RowCount !== 1){
                        return;
                    }

                    const rowData = self.prepareRowData(data.Dataset, editColumns);
                    rowData.key = keyValues;
                    rowData.cells = data.Dataset.Cells[0];
                    self.editErrorState.has = false;
                    self.editErrorState.message = null;
                    self.editRowData = rowData;
                    self.needEditForm = true;
                    self.$nextTick(function () {
                        var elem = document.querySelector('#form-edit-record');
                        var instance = M.Modal.getInstance(elem);
                        instance.open();
                    });
                });
            },

            onCloseEditForm: function () {
                var elem = document.querySelector('#form-edit-record');
                var instance = M.Modal.getInstance(elem);
                instance.close();
                this.needEditForm = false;
                this.editRowData = null;
            },

            onSaveEditForm: function (data) {
                const pkCondition = this.createPrimaryKeyCondition(data.key);

                const options = {
                    token: this.currentQuery.token,
                    columns: [],
                    filter: pkCondition,
                    cells: []
                };

                let index = 0;
                for (const key in data.data) {
                    if (data.data.hasOwnProperty(key)) {
                        const columnData = data.data[key];
                        const column = columnData.column;
                        const datasetColumn = {
                            Type: column.type,
                            Name: column.name,
                            Index: index++
                        };
                        options.columns.push(datasetColumn);
                        options.cells.push(columnData.value);
                    }
                }
                api.updateQueryRecord(options, result => {
                    if (result.success) {
                        this.$store.dispatch('queries/executeCurrentQuery');
                        this.onCloseEditForm();
                        return;
                    }
                        
                    this.editErrorState.message = result.message || "Failed to save record";
                    this.editErrorState.has = true;
                });
            },

            onOpenAddForm: function () {

                const addColumns = this.addFormColumns;
                const rowData = this.prepareAddRowData(addColumns);

                this.addRowData = rowData;
                this.addErrorState.has = false;
                this.addErrorState.message = null;
                this.needAddForm = true;
                this.$nextTick(function () {
                    var elem = document.querySelector('#form-add-record');
                    var instance = M.Modal.getInstance(elem);
                    instance.open();
                });
            },

            onCloseAddForm: function () {
                var elem = document.querySelector('#form-add-record');
                var instance = M.Modal.getInstance(elem);
                instance.close();
                this.needAddForm = false;
                this.addRowData = null;
            },

            onSaveAddForm: function (data) {
                const options = {
                    token: this.currentQuery.token,
                    columns: [],
                    cells: []
                };

                let index = 0;
                for (const key in data.data) {
                    if (data.data.hasOwnProperty(key)) {
                        const columnData = data.data[key];
                        const column = columnData.column;
                        const datasetColumn = {
                            Type: column.type,
                            Name: column.name,
                            Index: index++
                        };
                        options.columns.push(datasetColumn);
                        options.cells.push(columnData.value);
                    }
                }
                api.createQueryRecord(options, result => {
                    if (result.success) {
                        this.$store.dispatch('queries/executeCurrentQuery');
                        this.onCloseAddForm();
                        return;
                    }
                        
                    this.addErrorState.message = result.message || "Failed to create record";
                    this.addErrorState.has = true;
                });
            },

            onOpenDeleteForm: function () {
                if (!this.currentRow){
                    return;
                }

                const keyValues = [];
                const pkCondition = this.preparePrimaryKeyCondition(keyValues);
                const viewColumns = this.viewFormColumns;

                const executeOptions = {
                    token: this.currentQuery.token,
                    columns: viewColumns,
                    filter: pkCondition
                };

                const self = this;
                api.executeQuery(executeOptions, data => {
                    if (data.Dataset.RowCount !== 1){
                        return;
                    }

                    const rowData = self.prepareRowData(data.Dataset, viewColumns);
                    rowData.key = keyValues;
                    rowData.cells = data.Dataset.Cells[0];
                    
                    self.viewRowData = rowData;
                    self.deleteErrorState.has = false;
                    self.deleteErrorState.message = null;
                    self.needDeleteForm = true;
                    self.$nextTick(function () {
                        var elem = document.querySelector('#form-delete-record');
                        var instance = M.Modal.getInstance(elem);
                        instance.open();
                    });
                });
            },

            onCloseDeleteForm: function () {
                var elem = document.querySelector('#form-delete-record');
                var instance = M.Modal.getInstance(elem);
                instance.close();
                this.needDeleteForm = false;
                this.viewRowData = null;
            },

            onSaveDeleteForm: function (data) {
                const pkCondition = this.createPrimaryKeyCondition(data.key);

                const options = {
                    token: this.currentQuery.token,
                    filter: pkCondition
                };

                api.deleteQueryRecord(options, result => {
                    if (result.success) {
                        this.$store.dispatch('queries/executeCurrentQuery');
                        this.onCloseDeleteForm();
                        return;
                    }
                        
                    this.deleteErrorState.message = result.message || "Failed to delete record";
                    this.deleteErrorState.has = true;
                });
            },
        }
    }
</script>