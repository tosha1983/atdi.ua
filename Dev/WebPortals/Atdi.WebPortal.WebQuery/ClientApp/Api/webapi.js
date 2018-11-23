function processStatus(response) {
    if (response.status >= 200 && response.status < 300) {
        return Promise.resolve(response)
    } else {
        return Promise.reject(new Error(response.statusText))
    }
}

function processJson(response) {
    return response.json()
}

function processError(error) {
    console.log('Request failed', error);
}

export default {
    SVC_ENVIRONMENT: 'environment',
    SVC_QUERYGROUPS: 'querygroups',
    SVC_WEBQUERIES: 'webqueries',
    get(service, processData) {
        return fetch('api/' + service, { credentials: 'include' })
            .then(processStatus)
            .then(processJson)
            .then(processData)
            .catch(processError);
    },

    post(service, action, data, processData) {
        return fetch('api/' + service + '/' + action, {
            method: 'POST',
            headers: {
                    'Content-Type': 'application/json'
                },
            body: JSON.stringify(data),
            credentials: 'include'
        })
            .then(processStatus)
            .then(processJson)
            .then(processData)
            .catch(processError);
    }
}