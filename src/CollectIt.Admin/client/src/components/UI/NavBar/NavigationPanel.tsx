import React, { useState} from 'react';
import {Link} from "react-router-dom";
import {AuthService} from "../../../services/AuthService";
import './NavbarStyle.css'

const NavigationPanel = () => {
    const logout = () => {
        AuthService.logout();
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
                            <Link to='/images' className={page === 3 ? 'nav-link active' : 'nav-link'} onClick={() => setPage(3)}>Images</Link>
                        </li>
                        <li className='nav-item px-5'>
                            <Link to='/musics' className={page === 4 ? 'nav-link active' : 'nav-link'} onClick={() => setPage(4)}>Musics</Link>
                        </li>
                        <li className='nav-item px-5'>
                            <Link to='/videos' className={page === 5 ? 'nav-link active' : 'nav-link'} onClick={() => setPage(5)}>Videos</Link>
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