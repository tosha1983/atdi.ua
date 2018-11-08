<template>
    <ul :id="barId" class="sidenav sidenav-fixed portal-slidebar-nav">
        <li>
            <div class="user-view">
                <div class="background">
                    <img class="responsive-img" src="images/office.jpg">
                </div>
                <a href="#user">
                    <img class="circle" src="images/company.jpg">
                </a>
                <a href="#name">
                    <span class="white-text name">{{portalEnvironment.company.title}}</span>
                </a>
                <a href="#email">
                    <span class="white-text email">{{portalEnvironment.company.email}}</span>
                </a>
            </div>
        </li>

        <li class="no-padding">
            <ul class="collapsible collapsible-accordion">

                <li v-for="queryGroup in queryGroups" :key="queryGroup.name" >

                    <a class="collapsible-header" @click="changeCurrentGroup(queryGroup.name)">{{queryGroup.title}}<i class="material-icons chevron">chevron_left</i></a>
                    <div class="collapsible-body">
                        <queries-bar :groupName="queryGroup.name"></queries-bar>
                    </div>
                </li>
            </ul>
        </li>
    </ul>
</template>

<script>
    import { mapState, mapActions } from 'vuex'
    import QueriesBar from './QueriesBar.vue'

    export default {
        name: 'GroupsBar',
        props: {
            barId: String,
        },

        components: {
            QueriesBar
        },

        computed: mapState({
            queryGroups: state => state.queryGroups.all,
            portalEnvironment: state => state.portal.environment,
            currentGroup: state => state.queryGroups.current
        }),

        data() {
            return {
            }
        },

        created() {
            this.$store.dispatch('queryGroups/loadGroups')
        },

        methods: {
            changeCurrentGroup(name) {
                if (!this.currentGroup || this.currentGroup.name !== name) {
                    this.$store.dispatch('queries/changeCurrentQuery', null);
                    this.$store.dispatch('queryGroups/changeCurrentGroup', name);
                }
            }
        }
    }
</script>