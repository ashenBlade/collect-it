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
    const [loaded, setLoaded] = useState(false);
    const options = [Role.User, Role.Admin, Role.TechSupport]

    useEffect(() => {
        UsersService.findUserByIdAsync(userId).then(i => {
            setName(i.username);
            setEmail(i.email);
            setDisplayName(i.username);
            setUser(i);
            setLoaded(true);
        }).catch(err => {
            alert(err.toString())
        })
    }, []);

    let resIds = "";
    let subIds = "";

    if ( user?.authorOf != undefined && user?.authorOf.length > 0){
        for (let i = 0; i < user?.authorOf.length; i++){
            resIds += user?.authorOf[i].id + ", "
        }
    }
    else {
        resIds = "There aren't any resource added by that user"
    }

    if ( user?.subscriptions != undefined && user?.subscriptions.length > 0){
        for (let i = 0; i < user?.subscriptions.length; i++){
            subIds += user?.subscriptions[i].id + ", "
        }
    }
    else {
        subIds = "There aren't any subscription"
    }


    const saveName = (newName: string) => {
        console.log('New name', newName);
        if (!user) return;
        UsersService.changeUsernameAsync(userId, newName).then(_ => {
            setName(newName);
            setDisplayName(newName)
        }).catch(_ => {
            alert('Could not change user name. Try later.')
        })
    }

    const saveEmail = (newEmail: string) => {
        console.log('New email', newEmail);
        if (!user) return;
        UsersService.changeEmailAsync(userId, newEmail).then(_ => {
            setEmail(newEmail);
        }).catch(_ => {
            alert('Could not change image name. Try later.')
        })
    }

    // @ts-ignore
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
                            <div className='h6 d-block'>
                                Author of: {resIds}
                            </div>
                            <div className='h6 d-block'>
                                Subscriptions: {subIds}
                            </div>
                            { user?.lockout ?
                                <div className='h6 d-block'>
                                    Lock out: Locked
                                </div>
                                :
                                <div className='h6 d-block'>
                                    Lock out: Unlocked
                                </div>
                            }
                            { user?.lockout ?
                                <div className='h6 d-block'>
                                    Lock out end date: {user?.lockoutEnd}
                                </div>
                                : <span></span>
                            }
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
                                             UsersService.addUserToRoleAsync(userId, selectedItem as Role).then(_ => {})
                                                 .catch(_ => {
                                                        alert('Could not change image name. Try later.')
                                                 })
                                         }}
                                         onRemove={(selectedList, selectedItem) => {
                                             UsersService.removeUserFromRoleAsync(userId, selectedItem as Role).then(_ => {})
                                                 .catch(_ => {
                                                     alert('Could not change image name. Try later.')
                                                 })
                                         }}
                                         options={options}
                                         selectedValues={user?.roles as Role[]}/>
                        </div>

                    </div>
                    : <p>Loading...</p>
            }
        </div>
    );
};

export default EditUser;