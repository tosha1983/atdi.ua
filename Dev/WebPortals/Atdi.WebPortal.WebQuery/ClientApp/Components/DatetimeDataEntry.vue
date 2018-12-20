<template>
    <div :id="id" class="row">
        <div class="input-field col s8">
            <i class="material-icons prefix">event</i>
            <input v-if="state === 'Editable'"          :id="dateId" type="text"  @change="changeDateText" class="datepicker">
            <input v-if="state === 'Readonly'" readonly :id="dateId" type="text"  class="datepicker">
            <input v-if="state === 'Disabled'" disabled :id="dateId" type="text"  class="datepicker">
            <label :for="dateId">{{title}}</label>
        </div>
        <div class="input-field col s4">
            <input v-if="state === 'Editable'"          :id="timeId" type="text" @change="changeTimeText"  class="timepicker">
            <input v-if="state === 'Readonly'" readonly :id="timeId" type="text"  class="timepicker">
            <input v-if="state === 'Disabled'" disabled :id="timeId" type="text"  class="timepicker">
            <label :for="timeId">Time:</label>
        </div>
    </div>
</template>
<script>
    export default {
        name: 'DatetimeDataEntry',

        props: {
            id: String,
            title: String,
            value: {},
            state: String, // 'Editable', 'Readonly', 'Disabled'
        },

        components: {
        },

        computed: {
            dateId: function () {
                return this.id + "-input-date";
            },
            timeId: function () {
                return this.id + "-input-time";
            },
        },

        data() {
            return {
                date: this.value ? new Date(this.value) : null,
                hours: this.value ? (new Date(this.value)).getHours() : null,
                minutes: this.value ? (new Date(this.value)).getMinutes() : null,
                isoValue: this.value
            }

        },

        watch: {
        },
        
        methods: {
            setDate: function(newDate) {
                if (!this.date && !newDate) {
                    return
                }
                if ((!this.date && newDate) 
                || (this.date && !newDate) 
                || this.date.getFullYear() !== newDate.getFullYear()
                || this.date.getMonth() !== newDate.getMonth()
                || this.date.getDate() !== newDate.getDate()){
                    this.date = newDate;
                    this.buildValue();
                }
            },

            setTime: function(hours, minutes) {
                let needRebuild = false;
                if ((!this.hours && hours) || (this.hours && !hours) || this.hours !== hours){
                    needRebuild = true;
                    this.hours = hours;
                }
                if ((!this.minutes && minutes) || (this.minutes && !minutes) || this.minutes !== minutes){
                    needRebuild = true;
                    this.minutes = minutes;
                }

                if (needRebuild){
                    this.buildValue();
                }
            },

            buildValue: function() {
                if (!this.date){
                    this.isoValue = null
                } else {
                    this.date.setHours(this.hours);
                    this.date.setMinutes(this.minutes);
                    this.date.setSeconds(0);
                    this.date.setMilliseconds(0);
                    this.isoValue = this.datetimeToISOString(this.date);
                }
                
                this.$emit('changedValue', this.isoValue);
            },

            pad: function (n) {
                if (n < 10) {
                    return '0' + n;
                }
                return n;
            },

            datetimeToISOString : function (datetime) {
                return datetime.getFullYear() +
                    '-' + this.pad(datetime.getMonth() + 1) +
                    '-' + this.pad(datetime.getDate()) +
                    'T' + this.pad(datetime.getHours()) +
                    ':' + this.pad(datetime.getMinutes()) +
                    ':' + this.pad(datetime.getSeconds()) +
                    '.' + (datetime.getMilliseconds() / 1000).toFixed(3).slice(2, 5)
            },
            changeDateText: function () {
                const datepickerElement = document.getElementById(this.dateId);
                const dateInstance = M.Datepicker.getInstance(datepickerElement);
                if (dateInstance.date){
                    this.setDate(dateInstance.date);
                } else {
                    this.setDate(null);
                }
                
            },
            changeTimeText: function(value) {
                const timepickerElement = document.getElementById(this.timeId);
                const timeInstance = M.Timepicker.getInstance(timepickerElement);
                this.setTime(timeInstance.hours, timeInstance.minutes);
            }
        },

        mounted: function (){
            
            let initState = true;
            const datepickerElement = document.getElementById(this.dateId);
            const timepickerElement = document.getElementById(this.timeId);
            
            self = this;

            const dateOptions = {
                container: 'body',
                autoClose: true,
                showClearBtn: true,
            }
            M.Datepicker.init(datepickerElement, dateOptions);

            let timeValue = "";
            let dateValue = null;
            if (this.value){
                dateValue = new Date(this.value);
                this.hours = dateValue.getHours();
                this.minutes = dateValue.getMinutes();
                timeValue = this.hours.toString() + ":" + this.minutes.toString();
            }

            const timeOptions = {
                container: 'body',
                autoClose: false,
                showClearBtn: false,
                defaultTime:  timeValue,
                twelveHour: false
            };

            M.Timepicker.init(timepickerElement, timeOptions);

            if (this.value){

                const dateInstance = M.Datepicker.getInstance(datepickerElement);
                const timeInstance = M.Timepicker.getInstance(timepickerElement);
                
                dateInstance.setDate(dateValue);
                datepickerElement.value = dateInstance.toString();

                timepickerElement.value = timeValue;
            }

            initState = false;
        }
    }
</script>