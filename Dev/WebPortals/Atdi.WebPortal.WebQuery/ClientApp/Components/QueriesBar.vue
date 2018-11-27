<template>
    <ul>
        <li v-for="query in getQueriesByGroup.list" @click="changeCurrentQuery(query)" :key="query.token.id">
            <!--
            <a href="javascript:undefined" class="waves-effect">{{query.title}}<i class="material-icons">landscape</i></a>
            -->

            <router-link :to="{ path: '/', query: { group: groupName, query: query.token.id} }" class="waves-effect" active-class="" exact-active-class="">{{query.title}}<i class="material-icons">landscape</i></router-link>

        </li>
    </ul>
</template>

<script>
    import { mapState, mapActions, mapGetters  } from 'vuex'

    export default {
        name: 'QueriesBar',

        props: {
            groupName: String,
            activeQuery: Number
        },

        components: {
        },

        computed: {
            getQueriesByGroup: function() {
                return this.$store.getters['queryGroups/getQueriesByGroup'](this.groupName);
            }
        },

        data() {
            return {
            }
        },

        updated() {
            const queries = this.getQueriesByGroup.list;
            for (let index = 0; index < queries.length; index++) {
                const query = queries[index];
                if (query.token.id ===this.activeQuery){
                    this.changeCurrentQuery(query);
                    break;
                }
            }
        },
        methods: {
            changeCurrentQuery(query) {
                this.$store.dispatch('queries/changeCurrentQuery', query)
            }
        },

        watch: {
            activeQuery: function (val, oldVal) {
                //this.changeCurrentGroup(val);

                const queries = this.getQueriesByGroup.list;
                for (let index = 0; index < queries.length; index++) {
                    const query = queries[index];
                    if (query.token.id === this.activeQuery){
                        this.changeCurrentQuery(query);
                        break;
                    }
                }

            }
        }
    }
</script>