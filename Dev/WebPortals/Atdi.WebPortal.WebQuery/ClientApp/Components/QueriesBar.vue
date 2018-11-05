<template>
    <ul>
        <li v-for="query in getQueriesByGroup.list">
            <a href="#" class="waves-effect">{{query.title}}<i class="material-icons">landscape</i></a>
        </li>
    </ul>
</template>

<script>
    import { mapState, mapActions, mapGetters  } from 'vuex'

    export default {
        name: 'QueriesBar',

        props: {
            groupName: String
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

        methods: {
            changeCurrentQuery(token) {
                this.$store.dispatch('queryGroups/changeCurrentQuery', token)
            }
        }
    }
</script>