import {useContext} from "react";
import {AdminAuthContext} from "./AuthContext";


const authFetch = (info: RequestInfo, init: RequestInit | null, jwt: string): Promise<Response> => {
    init = init ?? {};
    init.headers = Object.assign(init.headers ?? {}, {
        'Authorization': `Bearer ${jwt}`
    });
    init.mode = 'cors';
    return fetch(info, init)
}

const useAuthFetch = (jwt: string | null = null) => {
    return (info: RequestInfo, init: RequestInit | null) =>
        authFetch(info, init, jwt ?? useContext(AdminAuthContext).jwt());
}

export default useAuthFetch;
