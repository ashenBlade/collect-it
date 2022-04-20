import React from "react";

const jwtKeyName = "admin.jwt"

const authState = {
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

    login: (jwt: string): void => {
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
    }


}

export const AuthContext = React.createContext(authState);