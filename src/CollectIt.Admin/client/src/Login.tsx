import React from 'react';
import {useNavigate} from 'react-router-dom';
import {AuthContext} from "./AuthService";
const testJwt = 'eyJhbGciOiJIUNiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJyb2xlcyI6WyJBZG1pbiIsIlRlY2hpbmNhbCBTdXBwb3J0Il19.dNmydD98AtJKSIPIGC2K_P_obQfb6qp2mbt_0eT2iTo';
const Login = () => {
    const authContext = React.useContext(AuthContext);
    const login = (jwt: string) => {
        try {
            authContext.adminLogin(jwt);
        } catch (e) {
            alert(e);
            return;
        }
        window.location.href = '/';
    }

    const logout = () => {
        authContext.logout();
    }
    return (
        <div>
            <div>
                {
                    authContext.isAuthenticated()
                        ? <button className='btn btn-danger p-1' onClick={logout}>Logout</button>
                        : <button className='btn btn-success p-1' onClick={e => login(testJwt)}>Login</button>
                }
            </div>
        </div>
    );
};

export default Login;