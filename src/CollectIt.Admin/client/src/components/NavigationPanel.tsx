import React from 'react';
import {Link, NavLink} from "react-router-dom";
import UsersList from "./pages/users/UsersList";

const NavigationPanel = () => {
    return (
        <nav className='nav navbar'>
            <ul >
                <li className='nav-item'>
                    {/*<a href='/users' className='nav-link'>Users</a>*/}
                    {/*<a href='/subscriptions' className='nav-link'>Users</a>*/}
                    {/*<a href='/resources' className='nav-link'>Users</a>*/}
                    <Link to='/users' >Users</Link>
                </li>
                <li>
                    <Link to='/subscriptions'>Subscriptions</Link>
                </li>
                <li>
                    <Link to='/resources'>Resources</Link>
                </li>
            </ul>
        </nav>
    );
};

export default NavigationPanel;