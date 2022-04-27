import React, {ChangeEvent, useEffect, useState} from 'react';
import '../../UI/NavBar/NavbarStyle.css'
import User from "../../entities/user";
import Pagination from "../../UI/pagination/Pagination";
import {UsersService} from "../../../services/UsersService";

const UsersList = () => {
    const pageSize = 15;
    const [page, setPage] = useState(1);
    const [users, setUsers] = useState<User[]>([{id: 1, username: 'Test name', email: 'testemail@mail.cum', roles: ['Admin'], authorOf: [], subscriptions: []}]);
    const [enteredText, setEnteredText] = useState("");
    useEffect(() => {
        UsersService.getUsersPagedAsync(page, pageSize).then(x => {
            setUsers(x.users);
        })
    }, [])
    return (
        <div>
            <div className='w-75 mt-5 mx-auto'>
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
            </div>
        </div>
    );
};

export default UsersList;