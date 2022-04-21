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
    const [page, setPage] = useState(0);

    return (
        <nav className='navbar navbar-light bg-light'>
            <div className='container-lg'>
                <div className='navbar-collapse navbar' id='navbarSupportedContent'>
                    <div>
                        <a className={'navbar-brand px-4'}>CollectIt</a>
                    </div>
                    <ul className='navbar-nav flex-row'>
                        <li className='nav-item px-5'>
                            <Link to='/users' className={page === 0 ? 'nav-link active' : 'nav-link'} onClick={() => setPage(0)}>Users</Link>
                        </li>
                        <li className=' nav-item px-5'>
                            <Link to='/subscriptions' className={page === 1 ? 'nav-link active' : 'nav-link'} onClick={() => setPage(1)}>Subscriptions</Link>
                        </li>
                        <li className='nav-item px-5'>
                            <Link to='/resources' className={page === 2 ? 'nav-link active' : 'nav-link'} onClick={() => setPage(2)}>Resources</Link>
                        </li>
                    </ul>
                    <span>
                        <a className='btn btn-danger justify-content-end' onClick={logout}>Logout</a>
                    </span>
                </div>
            </div>
        </nav>
    );
};

export default NavigationPanel;