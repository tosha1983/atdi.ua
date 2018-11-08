import webapi from './webapi'



export default {
    getEnvironment(handler) {
        webapi.get(webapi.SVC_ENVIRONMENT, data => handler(data));
    },

    getQueryGroups(handler) {
        webapi.get(webapi.SVC_QUERYGROUPS, data => {
            if (data && data.groups)
                handler(data.groups);
            else
                handler([]);
        });
    },

    getQueriesByTokens(tokens, handlers) {
        webapi.post(webapi.SVC_WEBQUERIES, 'get', { tokens }, data => {
            handlers(data);
        });
    },

    executeQuery(token, handlers) {
        webapi.post(webapi.SVC_WEBQUERIES, 'execute', { token }, data => {
            handlers(data);
        });
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