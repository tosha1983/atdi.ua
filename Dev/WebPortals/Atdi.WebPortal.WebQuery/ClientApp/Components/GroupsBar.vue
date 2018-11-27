<template>
    <ul :id="barId" class="sidenav sidenav-fixed portal-slidebar-nav">
        <li>
            <div class="user-view">
                <div class="background">
                    <img class="responsive-img" src="images/office.jpg">
                </div>
                <a :href="'//' + portalEnvironment.company.site" target="_blank">
                    <img class="circle" src="images/company.jpg">
                </a>
                <a :href="'//' + portalEnvironment.company.site" target="_blank">
                    <span class="white-text name">{{portalEnvironment.company.title}}</span>
                </a>
                <a :href="'//' + portalEnvironment.company.site" target="_blank">
                    <span class="white-text email">{{portalEnvironment.company.email}}</span>
                </a>
            </div>
        </li>

        <li class="no-padding">
            <ul class="collapsible collapsible-accordion" id="portal-groups-slider">

                <li v-for="queryGroup in queryGroups" :key="queryGroup.name" :id="'portal-groups-slider-li-' + queryGroup.name" :class="[queryGroup.name === activeGroup ? 'active' : '']">
                    <!--
                        <a href="javascript:undefined" class="collapsible-header" @click="changeCurrentGroup(queryGroup.name)">{{queryGroup.title}}<i class="material-icons chevron">chevron_left</i></a>
                    -->
                    <router-link :to="{ path: '/', query: { group: queryGroup.name } }" @click="changeCurrentGroup(queryGroup.name)" class="collapsible-header" active-class="" exact-active-class="">{{queryGroup.title}}<i class="material-icons chevron">chevron_left</i></router-link>

                    <div class="collapsible-body">
                        <queries-bar :groupName="queryGroup.name" :active-query="activeQuery"></queries-bar>
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
            activeGroup: String,
            activeQuery: Number
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
          //  this.$store.dispatch('queryGroups/loadGroups')
        },

        mounted() {
            
            //sliderInstance.close();

            //const itemElement = document.getElementById('portal-groups-slider-li-' + this.activeGroup);
            //if (!itemElement)
            //    return;

            //itemElement.classList.add('active');

            //sliderInstance.open();
            const self = this;
            //this.$store.dispatch('queryGroups/setActiveGroup', this.activeGroup);
            this.$store.dispatch('queryGroups/loadGroups')
                .then(function () {
                    
                    const groups = self.$store.state.queryGroups.all;
                    let groupIndex = -1;
                    for (let index = 0; index < groups.length; index++) {
                        const group = groups[index];
                        if (group.name && group.name === self.activeGroup){
                            groupIndex = index;
                            break;
                        }
                    }
                    const sliderElement = document.getElementById('portal-groups-slider');
                    M.Collapsible.init(sliderElement);
                    if (groupIndex >= 0) {
                        const sliderInstance = M.Collapsible.getInstance(sliderElement);
                        sliderInstance.open(groupIndex);

                        self.$store.dispatch('queryGroups/changeCurrentGroup', self.activeGroup)
                        .then(function() {

                        });
                    }
                    

                });
            //this.changeCurrentGroup(this.activeGroup);
        },

        methods: {
            changeCurrentGroup(name) {
                if (!this.currentGroup || this.currentGroup.name !== name) {
                    this.$store.dispatch('queries/changeCurrentQuery', null);
                    this.$store.dispatch('queryGroups/changeCurrentGroup', name);
                }
            }
        },

        watch: {
            activeGroup: function (val, oldVal) {
                this.changeCurrentGroup(val);
            }
        }
    }
</script>