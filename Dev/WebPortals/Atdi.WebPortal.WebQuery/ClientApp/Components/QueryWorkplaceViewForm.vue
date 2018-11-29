<template>
    <div :id="id" class="modal modal-fixed-footer">
        <div class="modal-content">
            <h4>View Item: {{query.title}}</h4>
            <div class="row portal-form-place">
                <form class="col s12">
                    <template v-for="column in row.columns">
                        <div class="row" :key="column.name">
                            <div class="col s6">
                                <data-entry
                                    :id="'de-' + column.name"
                                    :column="column"
                                    :value="getValue(column)"
                                    mode="View"
                                ></data-entry>
                            </div>
                        </div>
                    </template>
                </form>
            </div>
        </div>
        <div class="modal-footer">
            <a @click="onClose" class="waves-effect waves-green btn-flat" href="javascript:undefined">Close</a>
            
        </div>
    </div>
</template>
<script>
    import { mapState, mapActions, mapGetters } from 'vuex'
    import DataEntry from './DataEntry.vue'

    export default {
        name: 'QueryWorkplaceViewForm',

        props: {
            id: String,
            query: Object,
            row: Object, // {key: [{column, value}], columns, cells, map}
        },

        components: {
            DataEntry
        },

        computed: {
        },

        data() {
            return {
               
            }

        },

        methods: {
            getValue: function (column) {
                try{
                    const realColumn = this.row.map[column.name];
                    return this.row.cells[realColumn.Index];
                }
                catch{
                    return "";
                }
                
            },

            onClose: function () {
                this.$emit('closeViewForm');
            }
        },
        mounted: function (){
            const modalElements = document.querySelectorAll('.modal');
            M.Modal.init(modalElements);

             M.updateTextFields();
        }
    }
</script>