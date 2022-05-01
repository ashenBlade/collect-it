import React, {useEffect, useState} from 'react';
import InputBlock from "../../editBlocksComponents/editInputBlock/InputBlock";
import {useNavigate, useParams} from "react-router";
import {UsersService} from "../../../services/UsersService";
import User from "../../entities/user";
import {Role} from "../../entities/role";
import {Multiselect} from "multiselect-react-dropdown";
import SaveButton from "../../UI/SaveButton/SaveButton";

const EditUser = () => {
    const params = useParams();
    const userId = Number(params.userId?.trim());
    const nav = useNavigate();
    if (!Number.isInteger(userId)) {
        alert('Invalid id')
        nav('/users');
    }

    const [user, setUser] = useState<User | null>(null);
    const [displayName, setDisplayName] = useState('');
    const [username, setUsername] = useState('');
    const [usernameError, setUsernameError] = useState('');
    const [email, setEmail] = useState('');
    const [emailError, setEmailError] = useState('');
    const [loaded, setLoaded] = useState(false);
    const [roles, setRoles] = useState<string[]>([]);

    const roleNames = [Role.Admin, Role.TechSupport]

    const addRole = (role: Role) => {
        UsersService.addUserToRoleAsync(userId, role).then(() => {
            setRoles([...roles.filter(r => r !== role), role]);
        }).catch(err => {
            console.error(err);
            alert('Could not assign role. Try later');
        })
    }

    const removeRole = (role: Role) => {
        UsersService.removeUserFromRoleAsync(userId, role).then(() => {
            setRoles([...roles.filter(r => r !== role)]);
        }).catch(err => {
            console.error(err);
            alert('Could not remove role. Try later');
        })
    }

    useEffect(() => {
        UsersService.findUserByIdAsync(userId).then(i => {
            setUsername(i.username);
            setEmail(i.email);
            setDisplayName(i.username);
            setUser(i);
            setLoaded(true);
        }).catch(err => {
            alert(err.toString())
        })
    }, []);



    const saveName = (newName: string) => {
        console.log('Setting new username', newName);
        newName = newName.trim();
        if (newName?.length < 6) {
            setUsernameError('Length of name must be greater than 6');
            return;
        } else if (newName.indexOf(' ') !== -1) {
            setUsernameError('Username must not contain whitespaces');
            return;
        }
        UsersService.changeUsernameAsync(userId, newName).then(_ => {
            setUsername(newName);
            setDisplayName(newName)
        }).catch(x => {
            console.error(x)
            alert(`Could not change user username. Try later.`);
        })
    }

    const emailRegex = /^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/;
    const saveEmail = (newEmail: string) => {
        console.log('New email', newEmail);
        if (!emailRegex.test(newEmail)) {
            setEmailError('Email is not in valid form');
            return;
        }
        UsersService.changeEmailAsync(userId, newEmail).then(_ => {
            setEmail(newEmail);
        }).catch(x => {
            console.error(x)
            alert('Could not change image username. Try later.')
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
                                Banned: {user?.lockout ? 'True' : 'False'}
                            </div>
                        </div>

                        <InputBlock id={userId}
                                    fieldName={'Name'}
                                    placeholder={"User username"}
                                    initial={username}
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
                                             const role = selectedItem === Role.Admin ? Role.Admin : selectedItem === Role.TechSupport ? Role.TechSupport : undefined;
                                             if (!role) return;
                                             addRole(role);
                                         }}
                                         onRemove={(selectedList, selectedItem) => {
                                            const role = selectedItem === Role.Admin ? Role.Admin : selectedItem === Role.TechSupport ? Role.TechSupport : undefined;
                                            if (!role) return;
                                             removeRole(role);
                                         }}
                                         options={roleNames}
                                         selectedValues={user?.roles as Role[]}/>
                        </div>

                    </div>
                    : <p>Loading...</p>
            }
        </div>
    );
};

export default EditUser;