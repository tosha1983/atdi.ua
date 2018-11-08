<template>
    <div :id="id" class="card-action">
        <a v-if="allowedFetch" class="waves-effect waves-light" @click="fetchData">Fetch</a>
        <a v-if="allowedAdd" class="waves-effect waves-light modal-trigger" href="#form-add-record">Add</a>
        <a v-if="allowedView" class="waves-effect waves-light" @click="openViewForm">View</a>
        <a v-if="allowedEdit" class="waves-effect waves-light" href="#">Edit</a>
        <a v-if="allowedDel" class="waves-effect waves-light"  href="#">Delete</a>
        <a @click="exportExcel" class="waves-effect waves-light">Excel</a>
    </div>
</template>
<script>
    import { mapState, mapActions } from 'vuex'

    export default {
        name: 'QueryWorkplaceToolbar',

        props: {
            id: String,
            group: Object,
            query: Object
        },

        components: {
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

            currentRow: function () {
                    return this.$store.getters['queries/currentRow'];
                },

            processedRows: function() {
                const data = this.queryData;
                if (data)
                    return data.rows;

                return [];
            },
        },

        data() {
            return {
            }
        },

        methods: {
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
            }
        },


        mounted() {

        }
    }
</script>