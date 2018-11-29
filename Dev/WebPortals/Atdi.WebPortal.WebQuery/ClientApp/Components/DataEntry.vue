<template>

    <div :id="id" class="">
        <div class="input-field">
            <i class="material-icons prefix">filter</i>
            <input v-if="isReadonly" readonly :id="column.name" type="text" v-model="changedValue" class="">
            <input v-if="!isReadonly" :id="column.name" type="text" v-model="changedValue" class="">
            <label :for="column.name">{{column.title}}</label>
        </div>
    </div>

</template>
<script>
    export default {
        name: 'DataEntry',

        props: {
            id: String,
            column: Object,
            value: {},
            mode: String, // 'Add', 'Edit', 'View'
        },

        components: {
        },

        computed: {
            isReadonly: function() {
                if (this.mode === "View") {
                    return true;
                }

                if (this.column.readonly) {
                    return true;
                }

                if (this.mode === "Add" && this.column.notChangeableByAdd) {
                    return true;
                }

                if (this.mode === "Edit" && this.column.notChangeableByEdit) {
                    return true;
                }

                return false;

            }
        },

        data() {
            return {
                changedValue: this.value
            }

        },

        watch: {
            changedValue: function( ){
                this.$emit('changedValue', { column: this.column, value: this.changedValue});
            }
        },
        
        methods: {
            
            getValue: function () {
                return this.value;
            },

            onChangedValue: function (newValue) {
                this.$emit('changedValue', { column: this.column, value: newValue});
            },

        },
        mounted: function (){
            //M.updateTextFields();
        }
    }
</script>