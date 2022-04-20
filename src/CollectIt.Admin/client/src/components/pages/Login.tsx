import React from 'react';
import {AdminAuthContext} from "../../services/AuthService";
// Only for tests
const testJwt = 'eyJhbGciOiJIUNiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJyb2xlcyI6WyJBZG1pbiIsIlRlY2hpbmNhbCBTdXBwb3J0Il19.dNmydD98AtJKSIPIGC2K_P_obQfb6qp2mbt_0eT2iTo';
const Login = () => {
    const authContext = React.useContext(AdminAuthContext);
    const login = (jwt: string) => {
        try {
            authContext.adminLogin(jwt);
        } catch (e) {
            alert(e);
            return;
        }
        // Have to reload
        window.location.href = '/';
    }

    const logout = () => {
        authContext.logout();
        // Have to reload site
        window.location.href = authContext.loginPath();
    }
    return (
        <div>
            <div>
                {
                    authContext.isAuthenticated()
                        ? <button className='btn btn-danger p-1' onClick={logout}>Logout</button>
                        : <button className='btn btn-success p-1' onClick={_ => login(testJwt)}>Login</button>
                }
            </div>
        </div>
    );
};

export default Login;