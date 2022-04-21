import React, {useState} from 'react';
import {AdminAuthContext} from "../../../services/AuthService";
// Only for tests
const testJwt = 'eyJhbGciOiJIUNiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJyb2xlcyI6WyJBZG1pbiIsIlRlY2hpbmNhbCBTdXBwb3J0Il19.dNmydD98AtJKSIPIGC2K_P_obQfb6qp2mbt_0eT2iTo';
const isTest = process.env.NODE_ENV === 'development' || true;
const Login = () => {
    const auth = React.useContext(AdminAuthContext);
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState<string>('');
    const onClickLoginButton = async (e : React.MouseEvent) => {
        e.preventDefault();
        const usernameCleaned = username.trim();
        const passwordCleaned = password;
        if (!(usernameCleaned && passwordCleaned)) {
            setError('Fill password and username');
            return;
        }
        const response = await fetch('http://localhost:7000/auth/login', {
            method: 'POST',
            body: JSON.stringify({
                password: passwordCleaned,
                username: usernameCleaned
            }),
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json'
            },
        });
        if (!response.ok) {
            const description = await response.json();
            const message = description.message;
            console.error(`Response failed with code ${response.status}. Description: ${message}`);
            setError(message);
            return;
        }

        const jwt = (await response.json()).token;
        if (!jwt) {
            setError('No token provided in response');
            return;
        }

        try {
            auth.adminLogin(jwt);
        } catch (e: any) {
            console.error(e);
            setError(e.message);
        }
    }
    const onClickTestButton = (e: React.MouseEvent) => {
        e.preventDefault();
        auth.adminLogin(testJwt);
        window.location.href = '/';
    }
    return (
        <div className='h-100 d-flex align-items-center justify-content-center'>
            <div>
                <form>
                    <p className={'h2 mb-2'}>CollectIt - Admin CRM</p>
                    <input id='email' className='form-control my-2' type='text' placeholder='Username' value={username}
                           onInput={e => setUsername(e.currentTarget.value)}/>
                    <input id='password' className='form-control my-2' type='password' placeholder='Password'
                           onInput={e => setPassword(e.currentTarget.value)}/>
                    <div className={'justify-content-center d-flex'}>
                        <button onClick={onClickLoginButton}
                                className='btn btn-success justify-content-center my-2'>Login
                        </button>
                    </div>
                    {isTest &&
                        <div className={'justify-content-center d-flex'}>
                            <button className='btn btn-outline-success p-1 justify-content-center'
                                    onClick={onClickTestButton}>Enter as admin (test
                                only)
                            </button>
                        </div>}
                    {error && <span className={'text-danger d-block text-center'}>{error}</span>}
                </form>
            </div>
        </div>
    )
};

export default Login;