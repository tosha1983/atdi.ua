<template>
    <div :id="id" class="portal-form-data-entry">
        <template v-if="entryType === 'text'">
            <text-data-entry
                :id="id + '-text'"
                :title="column.title"
                :value="getValue()"
                :state="entryState"
                @changedValue="onChangedValue"
            ></text-data-entry>
        </template>

        <template v-if="entryType === 'number'">
            <number-data-entry
                :id="id + '-text'"
                :title="column.title"
                :value="getValue()"
                :state="entryState"
                @changedValue="onChangedValue"
            ></number-data-entry>
        </template>

        <template v-if="entryType === 'datetime'">
            <datetime-data-entry
                :id="id + '-text'"
                :title="column.title"
                :value="getValue()"
                :state="entryState"
                @changedValue="onChangedValue"
            ></datetime-data-entry>
        </template>

        <template v-if="entryType === 'boolean'">
            <boolean-data-entry
                :id="id + '-text'"
                :title="column.title"
                :value="getValue()"
                :state="entryState"
                @changedValue="onChangedValue"
            ></boolean-data-entry>
        </template>
    </div>

</template>
<script>
    import TextDataEntry from './TextDataEntry.vue'
    import DatetimeDataEntry from './DatetimeDataEntry.vue'
    import BooleanDataEntry from './BooleanDataEntry.vue'
    import NumberDataEntry from './NumberDataEntry.vue'

    export default {
        name: 'DataEntry',

        props: {
            id: String,
            column: Object,
            value: {},
            mode: String, // 'Add', 'Edit', 'View'
        },

        components: {
            TextDataEntry,
            DatetimeDataEntry,
            BooleanDataEntry,
            NumberDataEntry
        },

        computed: {
            entryType: function () {
                if (this.column.type === 0 || this.column.type === "Undefined") {
                    return "text"
                }
                if (this.column.type === 1 || this.column.type === "String") {
                    return "text"
                }
                if (this.column.type === 2 || this.column.type === "Boolean") {
                    return "boolean"
                }
                if (this.column.type === 3 || this.column.type === "Integer") {
                    return "number"
                }
                if (this.column.type === 4 || this.column.type === "DateTime") {
                    return "datetime"
                }
                if (this.column.type === 5 || this.column.type === "Double") {
                    return "number"
                }
                if (this.column.type === 6 || this.column.type === "Float") {
                    return "number"
                }
                if (this.column.type === 7 || this.column.type === "Decimal") {
                    return "number"
                }
                if (this.column.type === 8 || this.column.type === "Byte") {
                    return "number"
                }
                if (this.column.type === 9 || this.column.type === "Bytes") {
                    return "text"
                }
                if (this.column.type === 10 || this.column.type === "Guid") {
                    return "text"
                }
                if (this.column.type === 11 || this.column.type === "DateTimeOffset") {
                    return "datetime"
                }
                if (this.column.type === 12 || this.column.type === "Time") {
                    return "datetime"
                }
                if (this.column.type === 13 || this.column.type === "Date") {
                    return "datetime"
                }
                if (this.column.type === 14 || this.column.type === "Long") {
                    return "number"
                }
                if (this.column.type === 15 || this.column.type === "Short") {
                    return "number"
                }
                if (this.column.type === 16 || this.column.type === "Char") {
                    return "text"
                }
                if (this.column.type === 17 || this.column.type === "SignedByte") {
                    return "number"
                }
                if (this.column.type === 18 || this.column.type === "UnsignedShort") {
                    return "number"
                }
                if (this.column.type === 19 || this.column.type === "UnsignedInteger") {
                    return "number"
                }
                if (this.column.type === 20 || this.column.type === "UnsignedLong") {
                    return "number"
                }
                if (this.column.type === 23 || this.column.type === "Xml") {
                    return "xml"
                }
                if (this.column.type === 24 || this.column.type === "Json") {
                    return "json"
                }

            },

            entryState: function() {
                if (this.mode === "View") {
                    return "Readonly";
                }

                if (this.column.readonly) {
                    return "Readonly";
                }

                if (this.mode === "Add" && this.column.notChangeableByAdd) {
                    return "Readonly";
                }

                if (this.mode === "Edit" && this.column.notChangeableByEdit) {
                    return "Readonly";
                }

                return "Editable";

            }
        },

        data() {
            return {
            }

        },

        watch: {
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