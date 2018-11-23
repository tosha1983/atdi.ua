<template>
    <div class="portal-query-workplace">

        <div class="row"></div>
        <div class="row">
            <div v-show="hasCurrentGroup" class="col s6">
                <ul class="collapsible">
                    <li v-if="hasCurrentGroup" >
                        <div class="collapsible-header"><i class="material-icons">collections</i><span><b>Group:</b> {{currentGroup.title}}</span></div>
                        <div class="collapsible-body"><span>{{currentGroup.description}}</span></div>
                    </li>
                    <li v-if="hasCurrentQuery">
                        <div class="collapsible-header"><i class="material-icons">landscape</i><span><b>Query:</b> {{currentQuery.title}}</span></div>
                        <div class="collapsible-body"><plaintext>{{currentQuery.description}}</plaintext></div>
                    </li>
                </ul>
            </div>
        </div>

        <div v-if="hasCurrentGroup && hasCurrentQuery" class="card s12">
            <query-workplace-toolbar id="query-result-toolbar" :group="currentGroup" :query="currentQuery" @openViewForm="onOpenViewForm"></query-workplace-toolbar>

            <query-workplace-table id="query-result-table" :query="currentQuery"></query-workplace-table>
            
            <template v-if="needViewForm">
                <query-workplace-view-form id="form-view-record" @closeViewForm="onCloseViewForm"></query-workplace-view-form>
            </template>

        </div>
    </div>
</template>
<script>
    import { mapState, mapActions } from 'vuex'
    import QueryWorkplaceToolbar from './QueryWorkplaceToolbar.vue'
    import QueryWorkplaceTable from './QueryWorkplaceTable.vue'
    import QueryWorkplaceViewForm from './QueryWorkplaceViewForm.vue'

    export default {
        name: 'QueryWorkplace',
        props: {
        },
        components: {
            QueryWorkplaceToolbar,
            QueryWorkplaceTable,
            QueryWorkplaceViewForm
        },

        computed: mapState({
            currentGroup: state => state.queryGroups.current,
            hasCurrentGroup: state => state.queryGroups.current && state.queryGroups.current !== null && state.queryGroups.current !== undefined,
            currentQuery: state => state.queries.current,
            hasCurrentQuery: state => state.queries.current && state.queries.current !== null && state.queries.current !== undefined,
            
        }),

        data() {
            return {
                needViewForm: false
            }
        },

        methods: {
            onOpenViewForm: function () {
                this.needViewForm = true;
                this.$nextTick(function () {
                    var elem = document.querySelector('#form-view-record');
                    var instance = M.Modal.getInstance(elem);
                    instance.open();
                });
            },

            onCloseViewForm: function () {
                var elem = document.querySelector('#form-view-record');
                var instance = M.Modal.getInstance(elem);
                instance.close();
                this.needViewForm = false;
            }
        }
    }
</script>