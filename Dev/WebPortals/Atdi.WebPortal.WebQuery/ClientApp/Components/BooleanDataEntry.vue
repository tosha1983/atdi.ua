<template>
    <div :id="id" class="input-field">
        <i class="material-icons prefix">mode_edit</i>
        <input v-if="state === 'Editable'"          :id="inputId" type="text" v-model="changedValue" class="">
        <input v-if="state === 'Readonly'" readonly :id="inputId" type="text" v-model="changedValue" class="">
        <input v-if="state === 'Disabled'" disabled :id="inputId" type="text" v-model="changedValue" class="">
        <label :for="inputId">{{title}}</label>
    </div>
</template>
<script>
    export default {
        name: 'BooleanDataEntry',

        props: {
            id: String,
            title: String,
            value: {},
            state: String, // 'Editable', 'Readonly', 'Disabled'
        },

        components: {
        },

        computed: {
            inputId: function () {
                return this.id + "-input";
            },
        },

        data() {
            return {
                changedValue: this.value
            }

        },

        watch: {
            changedValue: function( ){
                this.$emit('changedValue', this.changedValue);
            }
        },
        
        methods: {

        },

        mounted: function (){
            //M.updateTextFields();
        }
    }
</script>