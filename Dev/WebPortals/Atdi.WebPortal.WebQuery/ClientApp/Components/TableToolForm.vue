<template>
    <div :id="id" class="modal modal-fixed-footer">
        <div class="modal-content">
            <h4><slot name="title"></slot></h4>
            <div class="row portal-form-place">
                <slot name="content"></slot>
            </div>
        </div>
        <div class="modal-footer">
            <a @click="onApply" class="waves-effect waves-green btn-flat" href="javascript:undefined">Apply</a>
            <a @click="onClose" class="waves-effect waves-green btn-flat" href="javascript:undefined">Close</a>
        </div>
    </div>
</template>
<script>
    import { mapState, mapActions, mapGetters } from 'vuex'

    export default {
        name: 'TableToolForm',

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
            onApply: function () {
                this.$emit('apply');
            },

            onClose: function () {
                this.$emit('close');
            }
        },
        mounted: function (){
            const modalElements = document.querySelectorAll('.modal');
            M.Modal.init(modalElements);

             M.updateTextFields();
        }
    }
</script>