import {useContext} from "react";
import {AdminAuthContext} from "./AuthService";

const authFetch = (info: RequestInfo, init: RequestInit | null = null, jwt: string): Promise<Response> => {
    init = init ?? {};
    init.headers = Object.assign(init.headers ?? {}, {
        'Authorization': `Bearer ${jwt}`
    });
    init.mode = 'cors';
    return fetch(info, init)
}

const useAuthFetch = (jwt: string | null = null) => {
    const auth = useContext(AdminAuthContext);
    return (info: RequestInfo, init: RequestInit | null) =>
        authFetch(info, init, jwt ?? auth.jwt());
}

export default useAuthFetch;
