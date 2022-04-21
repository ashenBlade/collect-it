import React, {useContext,useState} from 'react';
import {Link} from "react-router-dom";
import {AdminAuthContext} from "../services/AuthService";
import './NavbarStyle.css'

const NavigationPanel = () => {
    const auth = useContext(AdminAuthContext);
    const logout = () => {
        auth.logout();
        // Need to reload page
        window.location.href = '/login';
    }
    const [page, setpage] = useState(0);

    return (
        <nav className='navbar navbar-light bg-navbar'>
            <div className='container-fluid'>
                <div className='navbar-collapse' id='navbarSupportedContent'>
                    <ul className='navbar-nav me-auto mb-2 mb-lg-0 flex-row'>
                        <li className='px-4'>
                            <h2>Collect it</h2>
                        </li>
                        <li className={page === 0 ? 'bg-white nav-item px-5' : 'nav-item px-5'}>
                            <Link to='/users' className='nav-link nav-item' onClick={() => setpage(0)}>Users</Link>
                        </li>
                        <li className={page === 1 ? 'bg-white nav-item px-5' : 'nav-item px-5'}>
                            <Link to='/subscriptions' className='nav-link nav-item'onClick={() => setpage(1)}>Subscriptions</Link>
                        </li>
                        <li className={page === 2 ? 'bg-white nav-item px-5' : 'nav-item px-5'}>
                            <Link to='/resources' className='nav-link nav-item'onClick={() => setpage(2)}>Resources</Link>
                        </li>
                        <li>
                            <a className='btn btn-lg btn-danger position-absolute btn-logout' onClick={logout}>Logout</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    );
};

export default NavigationPanel;