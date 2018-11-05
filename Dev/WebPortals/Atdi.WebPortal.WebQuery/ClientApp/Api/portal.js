import webapi from './webapi'



export default {
    getEnvironment(handler) {
        webapi.call(webapi.SVC_ENVIRONMENT, data => handler(data));
    },

    getQueryGroups(handler) {
        webapi.call(webapi.SVC_QUERYGROUPS, data => handler(data));
    },

    declareEnvironment () {
        return {
            title: null,
            version: null,

            user: {
                name: null
            },
            company: {
                title: null,
                 site: null,
                email: null
            }
        }
    }
}