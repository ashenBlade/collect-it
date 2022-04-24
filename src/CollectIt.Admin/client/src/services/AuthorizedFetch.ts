import {AuthService} from "./AuthService";


const authFetch = (info: RequestInfo, init: RequestInit | null = null, jwt: string): Promise<Response> => {
    init = init ?? {};
    init.headers = Object.assign(init.headers ?? {}, {
        'Authorization': `Bearer ${jwt}`
    });
    init.mode = 'cors';
    return fetch(info, init)
}

const authorizedFetch = (jwt: string | null = null) => {
    return (info: RequestInfo, init: RequestInit | null) =>
        authFetch(info, init, jwt ?? AuthService.jwt());
}

export default authorizedFetch;
