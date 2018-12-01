<template>
    <div :id="id" class="modal modal-fixed-footer">
        <div class="modal-content">
            <h4>Edit Item: {{query.title}}</h4>
            <template v-if="errorState && errorState.has">
                <h4 class="" style="color: red">Error: {{errorState.message}}</h4>
            </template>
            <div class="row portal-form-place">
                <form class="col s12">
                    <template v-for="column in row.columns">
                        <div class="row" :key="column.name">
                            <div class="col s6">
                                <data-entry
                                    :id="'de-' + column.name"
                                    :column="column"
                                    :value="getValue(column)"
                                    mode="Edit"
                                    @changedValue="onChangedValue"
                                ></data-entry>
                            </div>
                        </div>
                    </template>
                </form>
            </div>
        </div>
        <div class="modal-footer">
            <a v-if="hasChanged" @click="onSave" class="waves-effect waves-green btn-flat" href="javascript:undefined">Save</a>
            <a @click="onClose" class="waves-effect waves-green btn-flat" href="javascript:undefined">Cancel</a>
            
        </div>
    </div>
</template>
<script>
    import { mapState, mapActions, mapGetters } from 'vuex'
    import DataEntry from './DataEntry.vue'

    export default {
        name: 'QueryWorkplaceEditForm',

        props: {
            id: String,
            query: Object,
            row: Object, // {key: [{column, value}], columns, cells, map}
            errorState: Object // { has, message}
        },

        components: {
            DataEntry
        },

        computed: {
        },

        data() {
            return {
                changedData: {},
                hasChanged: false
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

            onSave: function () {
                this.$emit('saveEditForm', {key: this.row.key, data: this.changedData});
            },

            onClose: function () {
                this.$emit('closeEditForm');
            },

            onChangedValue: function({column, value}) {
                this.changedData[column.name] = {
                    column: column,
                    value: value
                }
                this.hasChanged = true;
                console.log("- changed column value [" + column.name + "]: ", value);
            }
        },
        mounted: function (){
            const modalElements = document.querySelectorAll('.modal');
            M.Modal.init(modalElements);

             M.updateTextFields();
             
        }
    }
</script>