import React, {useEffect, useState} from 'react';
import InputBlock from "../../editBlocksComponents/editInputBlock/InputBlock";
import {useNavigate, useParams} from "react-router";
import {UsersService} from "../../../services/UsersService";
import User from "../../entities/user";
import {Role} from "../../entities/role";
import {Multiselect} from "multiselect-react-dropdown";


const EditUser = () => {
    const params = useParams();
    const userId = Number(params.userId?.trim());
    const nav = useNavigate();
    if (!Number.isInteger(userId))
        nav('/users');
    const [user, setUser] = useState<User | null>(null);
    const [displayName, setDisplayName] = useState('');
    const [name, setName] = useState('');
    const [email, setEmail] = useState('');
    const [banned, setBanned] = useState(false);
    const [loaded, setLoaded] = useState(false);
    const options = [Role.Admin, Role.TechSupport]

    useEffect(() => {
        UsersService.findUserByIdAsync(userId).then(u => {
            setName(u.username);
            setEmail(u.email);
            setDisplayName(u.username);
            setUser(u);
            setBanned(u.lockout)
            setLoaded(true);
        }).catch(err => {
            console.error(err)
            alert('Could not load user')
            nav('/users')
        })
    }, []);

    const saveName = (newName: string) => {
        if (!user) return;
        UsersService.changeUsernameAsync(userId, newName).then(_ => {
            setName(newName);
            setDisplayName(newName);
        }).catch(err => {
            console.error(err)
            alert('Could not change user name. Try later.')
        })
    }

    const banUser = () => {
        UsersService.lockoutUserAsync(userId).then(() => {
            setBanned(true);
        }).catch(err => {
            console.error(err)
            alert('Could not ban user')
        })
    }

    const unbanUser = () => {
        UsersService.activateUserAsync(userId).then(() => {
            setBanned(false);
        }).catch(err => {
            console.error(err)
            alert('Could not unban user')
        })
    }

    const saveEmail = (newEmail: string) => {
        if (!user) return;
        UsersService.changeEmailAsync(userId, newEmail).then(_ => {
            setEmail(newEmail);
        }).catch(err => {
            console.error(err)
            alert('Could not change user email. Try later.')
        })
    }

    return (
        <div className='align-items-center justify-content-center shadow border col-6 mt-4 m-auto d-block rounded'>
            {
                loaded ?
                    <div className='col-12 p-3'>
                        <p className='h2 text-center'>{displayName}</p>

                        <div className='ms-4'>
                            <div className='h6 d-block'>
                                ID: {user?.id}
                            </div>
                            <div className={'h6 d-block'}>
                                Banned: {banned ? 'True' : 'False'}
                            </div>
                        </div>

                        <InputBlock id={userId}
                                    fieldName={'Name'}
                                    placeholder={"User name"}
                                    initial={name}
                                    onSave={e => saveName(e)}/>
                        <InputBlock id={userId}
                                    fieldName={'Email'}
                                    placeholder={"User email"}
                                    initial={email}
                                    onSave={e => saveEmail(e)} />
                        <div className='row m-0 ms-2'>
                            <label className='ms-3'>Roles: </label>
                            <Multiselect isObject={false}
                                         onSelect={(selectedList, selectedItem) => {
                                             UsersService.addUserToRoleAsync(userId, selectedItem as Role).then(_ => {
                                                 console.log('Role added')
                                             })
                                                 .catch(_ => {
                                                        alert('Could not assign role. Try later.')
                                                 })
                                         }}
                                         onRemove={(selectedList, selectedItem) => {
                                             UsersService.removeUserFromRoleAsync(userId, selectedItem as Role).then(_ => {})
                                                 .catch(_ => {
                                                     alert('Could not remove role. Try later.')
                                                 })
                                         }}
                                         options={options}
                                         selectedValues={user?.roles as Role[]}/>
                        </div>
                        {banned ?
                            <button className='btn btn-success rounded my-2' onClick={e => {
                                e.preventDefault();
                                unbanUser();
                            }}>Unban</button>
                            :
                            <button className='btn btn-danger rounded my-2' onClick={e => {
                                e.preventDefault();
                                banUser()
                                }}>Ban</button>

                        }
                    </div>
                    : <p>Loading...</p>
            }
        </div>
    );
};

export default EditUser;