import React from 'react';
import {AuthContext} from "./AuthService";

const Login = () => {
    const authContext = React.useContext(AuthContext);

    return (
        <div>
            <div>
                {
                    authContext.isAuthenticated()
                        ? <button className='btn btn-danger p-1' onClick={() => authContext.logout()}>Logout</button>
                        : <button className='btn btn-success p-1' onClick={() => authContext.login('some-jwt-test')}>Login</button>
                }
            </div>
        </div>
    );
};

export default Login;