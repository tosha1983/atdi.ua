import Vue from 'vue'
import Router from 'vue-router'
import Portal from './../Portal.vue'

Vue.use(Router)

export default new Router({
    mode: 'history',
    routes: [
        {
            path: '/',
            component: Portal,
            props: (route) => {
                return {
                    activeGroup: route.query.group,
                    activeQuery: parseInt(route.query.query)
                }
            }
        },
        {
            path: '/portal/',
            component: Portal,
            props: (route) => {
                return {
                    activeGroup: route.query.group
                }
            }
        },
        {
            path: '/portal/index',
            component: Portal
        }
    ]
})