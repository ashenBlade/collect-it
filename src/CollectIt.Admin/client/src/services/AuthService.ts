import jwt_decode from 'jwt-decode';

const jwtKeyName = "admin.jwt";
const loginPath = '/login';

const assertAdminRole = (jwt: string) => {
    const decoded: any = jwt_decode(jwt);
    if (!decoded.roles?.some((r: string) => r === 'Admin')) {
        throw new Error('User not in admin role');
    }
}

export const AuthService = {
    isAuthenticated: () : boolean => {
        return !!window.localStorage.getItem(jwtKeyName);
    },

    jwt: () : string => {
        const jwt = window.localStorage.getItem(jwtKeyName);
        if (!jwt) {
            throw new Error('No jwt stored');
        }
        return jwt;
    },

    adminLogin: (jwt: string): void => {
        assertAdminRole(jwt);
        window.localStorage.setItem(jwtKeyName, jwt);
    },

    logout: (): void => {
        window.localStorage.removeItem(jwtKeyName);
    },

    tryGetJwt: (): [boolean, string | null] => {
        const jwt = window.localStorage.getItem(jwtKeyName);
        return jwt
            ? [true, jwt]
            : [false, null];
    },

    loginPath: (): string => loginPath,
}

// export const AdminAuthContext = React.createContext(AuthService);