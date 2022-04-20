import React, {useContext} from 'react';
import {Link, NavLink} from "react-router-dom";
import {AdminAuthContext} from "../services/AuthService";

const NavigationPanel = () => {
    const auth = useContext(AdminAuthContext);
    const logout = () => {
        auth.logout();
        window.location.href = '/login';
    }
    return (
        <nav className='navbar navbar-light bg-light'>
            <div className='container-fluid'>
                <div className='navbar-collapse' id='navbarSupportedContent'>
                    <ul className='navbar-nav me-auto mb-2 mb-lg-0 flex-row'>
                        <li className='nav-item mx-2'>
                            <Link to='/users' className='nav-link active nav-item'>Users</Link>
                        </li>
                        <li className='nav-item mx-2'>
                            <Link to='/subscriptions' className='nav-link nav-item'>Subscriptions</Link>
                        </li>
                        <li className='nav-item mx-2'>
                            <Link to='/resources' className='nav-link nav-item'>Resources</Link>
                        </li>
                    </ul>
                    <a className='btn btn-danger p-1' onClick={logout}>Logout</a>
                </div>
            </div>
        </nav>
    );
};

export default NavigationPanel;