<template>
    <div :id="id" class="modal modal-fixed-footer">
        <div class="modal-content">
            <h4>View Item: {{currentQuery.title}}</h4>
            <div class="row portal-form-place">
                <form class="col s12">
                    <template v-for="column in currentQuery.columns">
                        <div class="row" :key="column.name">
                            <div class="input-field col s6">
                                <i class="material-icons prefix">filter</i>
                                <input readonly="readonly"  :id="column.name" type="text" :value="getValue(column)" class="">
                                <label :for="column.name">{{column.title}}</label>
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

    export default {
        name: 'QueryWorkplaceViewForm',

        props: {
            id: String
        },

        components: {
        },

        computed: {
                ...{
                currentRow: function () {
                    return this.$store.getters['queries/currentRow'];
                },

                currentQueryData: function () {
                    return this.$store.getters['queries/currentQueryData'];
                },

                ...mapState({
                    currentQuery: state => state.queries.current,
                })
            }
        },

        data() {
            return {
               
            }

        },

        methods: {
            getValue: function (column) {
                try{
                    const data = this.currentQueryData;
                    const realColumn = data.columnsMap[column.name];
                    return this.currentRow.cells[realColumn.Index];
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