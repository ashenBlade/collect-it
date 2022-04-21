import React from 'react';
import {Link} from "react-router-dom";
import '../../NavbarStyle.css'

const iterations = ['1','2','3','4','5','6','7','8','9','10'];
const Subscriptions = ['2','1','3','0'];
const Roles = ['Admin','TechSupport'];

const UsersList = () => {
    return (
        <div>
            <div className='w-75 mt-5 mx-auto'>
                <input id='email' className='form-control my-2' type='text' placeholder='Enter login/e-mail'/>
                <input id='email' className='form-control my-2' type='text' placeholder='Enter id'/>
                <tbody className='usersTable mx-auto mt-5'>
                <tr className="usersRow firstRow">
                    <td className='idCell color-purple'>ID</td>
                    <td className='usersCell color-purple'>Login</td>
                    <td className='usersCell color-purple'>E-mail</td>
                    <td className='usersCell color-purple'>Role</td>
                    <td className='usersCell color-purple'>Subscriptions</td>
                </tr>
                {iterations.map(i => (
                    <tr className="usersRow">
                        <td className='idCell'>{i}</td>
                        <td className='usersCell'>login</td>
                        <td className='usersCell'>e-mail@mail.ru</td>
                        <td className='usersCell'>{Roles[+i%2]}</td>
                        <td className='usersCell'>{Subscriptions[+i%4]}</td>
                    </tr>
                ))}
                </tbody>
                <ul className="pagination">
                    <li className="page-item">
                        <button className="page-link" type="button">1</button>
                    </li>
                    <li className="page-item">
                        <button className="page-link" type="button">2</button>
                    </li>
                    <li className="page-item">
                        <button className="page-link" type="button">3</button>
                    </li>
                    <li>
                        <h2 className='text-center mx-1 mb-0'>...</h2>
                    </li>
                    <li className="page-item">
                        <button className="page-link" type="button">9</button>
                    </li>
                </ul>
            </div>
        </div>
    );
};

export default UsersList;