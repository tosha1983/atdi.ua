import webapi from './webapi'



export default {
    getEnvironment(handler) {
        return webapi.get(webapi.SVC_ENVIRONMENT, data => handler(data));
    },

    getQueryGroups(handler) {
        return webapi.get(webapi.SVC_QUERYGROUPS, data => {
            if (data && data.groups)
                handler(data.groups);
            else
                handler([]);
        });
    },

    getQueriesByTokens(tokens, handlers) {
        return webapi.post(webapi.SVC_WEBQUERIES, 'get', { tokens }, data => {
            handlers(data);
        });
    },

    executeQuery({ token, columns, filter, orders, limit }, handlers) {
        return webapi.post(webapi.SVC_WEBQUERIES, 'Execute', { token, columns, filter, orders, limit }, data => {
            handlers(data);
        });
    },

    createQueryRecord({ token, columns, cells }, handlers) {
        return webapi.post(webapi.SVC_WEBQUERIES, 'CreateRecord', { token, columns, cells }, data => {
            handlers(data);
        });
    },

    updateQueryRecord({ token, columns, filter, cells }, handlers) {
        return webapi.post(webapi.SVC_WEBQUERIES, 'UpdateRecord', { token, columns, filter, cells }, data => {
            handlers(data);
        });
    },

    deleteQueryRecord({ token, filter }, handlers) {
        return webapi.post(webapi.SVC_WEBQUERIES, 'DeleteRecord', { token, filter }, data => {
            handlers(data);
        });
    },

    declareEnvironment() {
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