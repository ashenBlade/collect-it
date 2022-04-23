import React, {useEffect, useState} from 'react';
import '../../NavbarStyle.css'
import useAuthFetch from "../../../services/AuthorizedFetch";
// import {serverAddress} from "../../../constants";
import User from "../../entities/user";

const UsersList = () => {
    const fetch = useAuthFetch();
    const [users, setUsers] = useState<User[]>([{id: 1, username: 'Test name', email: 'testemail@mail.cum', roles: ['Admin'], authorOf: [], subscriptions: []}]);
    // const effect = useEffect(( ) => {
    //     fetch(`${serverAddress}/api/v1/users?page_number=1&page_size=10`, {
    //         method: 'GET'
    //     }).then(res => res.json()).then(json => setUsers(json)).catch(reason => {
    //         setUsers([{id: -1, username: 'Error', subscriptions: [], roles: [], email: 'Could not download', authorOf: []}])
    //     });
    // }, []);
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
                {users.map((u, i) => (
                    <tr className="usersRow">
                        <td className='idCell'>{i}</td>
                        <td className='usersCell'>{u.username}</td>
                        <td className='usersCell'>{u.email}</td>
                        <td className='usersCell'>{u.roles?.join(', ') ?? ''}</td>
                        <td className='usersCell'>{u.subscriptions?.length ?? 0}</td>
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