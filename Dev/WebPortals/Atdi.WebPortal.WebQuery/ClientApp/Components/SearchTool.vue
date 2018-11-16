<template>
    <div class="input-field waves-effect portal-table-search">
        <i class="material-icons prefix">search</i>
        <input id="icon_prefix" type="text" class="validate" v-model="searchValue">
        <label for="icon_prefix">Search</label>
    </div>
</template>

<script>
export default {
    name: 'SearchTool',

    props: {
        value: String
        },

    data: function () {
        return {

        };
    },

    computed: {
        searchValue: {
                get: function() {
                    this.$nextTick(function () { M.updateTextFields(); });
                    return this.value;
                },

                set: function(value) {
                    this.$emit('update:value', value);
                }
            }
    },

    methods: {
    },

    mounted() {
            M.updateTextFields();
            
        },
}
</script>

<style scoped>

.portal-table-search {
     height: 50px;
     margin: 0px;
 }

</style>
