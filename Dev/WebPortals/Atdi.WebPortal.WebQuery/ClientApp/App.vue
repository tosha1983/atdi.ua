<template>
    <body id="portal-app" class="portal">
        <section class="portal-slidebar">
            <groups-bar :barId="groupsBarId"></groups-bar>
        </section>

        <section class="portal-content">
            <header class="navbar-fixed ">
                <main-bar :barId="groupsBarId"></main-bar>
            </header>
            
            <main>
                <div class="row">
                    <div class="col s12">
                        <query-workplace></query-workplace>
                    </div>
                </div>
            </main>

            <footer-bar :barId="footerBarId" :title="company.name" :site="company.site"></footer-bar>

        </section>

        <div id="hidden-date"></div>
        <div id="hidden-time"></div>
    </body>
</template> 

<script>
    import MainBar from './Components/MainBar.vue'
    import GroupsBar from './Components/GroupsBar.vue'
    import FooterBar from './Components/FooterBar.vue'
    import QueryWorkplace from './Components/QueryWorkplace.vue'

    export default {
        name: 'App',

        components: {
            MainBar,
            GroupsBar,
            FooterBar,
            QueryWorkplace
        },

        data() {
            return {
                groupsBarId: 'groups-bar',
                footerBarId: 'main-footer',
                company: {
                    name: 'Google LTD',
                    site: 'www.google.com'
                }
            }
        }
    }
</script>     

<style>
    .portal-slidebar {
        position: fixed;
        left: 0px;
        z-index: 999;
    }

    .portal-content {
        padding-left: 0px
    }

    @media only screen and (min-width: 993px) {
        .portal-content {
            padding-left: 300px;
        }

        .portal-content-header {
            width: calc(100% - 300px);
        }
    }

    .portal-mainbar {
        background: #09678c
    }

    .portal-footer {
        background: #09678c
    }

    .portal {
        background-color: #efefef;
    }

    .portal-slidebar-nav {
        background-color: #09678cd1;
    }

    .sidenav .collapsible > .active .collapsible-header .chevron {
        transform: rotate(-90deg)
    }

    .sidenav .collapsible .collapsible-header .chevron {
        float: right;
        height: 24px;
        width: 24px;
        line-height: 24px;
        margin: 10px 0 0 0;
        transition: transform .2s
    }

    .portal-data-table td {
        padding: 8px 5px;
    }

    .portal-data-table th {
        padding: 10px 5px;
    }

    .portal-form-place {
        background: #bfc7cb33;
    }

    .portal-form-place .row {
        margin-bottom: 0px;
    }
</style>