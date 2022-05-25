import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from "react-router";
import { UsersService } from "../../../services/UsersService";
import User from "../../entities/user";
import { Role } from "../../entities/role";
import { Multiselect } from "multiselect-react-dropdown";
import { SubmitHandler, useForm } from "react-hook-form";

interface IFormInput {
    name: string;
    email: string;
}

const EditUser = () => {
    const { register, handleSubmit, formState: { errors } } = useForm<IFormInput>({mode: 'onBlur'});
    const onSubmit: SubmitHandler<IFormInput> = data => console.log(data);

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

    const saveNameAndEmail = (newName: string, newEmail: string) => {
        if (!(newName && newEmail)) throw new Error('Name or email not provided');
        UsersService.updateUserBatchAsync({id: userId, name: newName, email: newEmail}).then(() => {
            setEmail(email);
            setName(name);
            setDisplayName(name);
            alert('Update successful');
        }).catch(err => {
            alert('Could not update user')
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

                        <form onSubmit={handleSubmit(onSubmit)}>
                            <div className='m-0 ms-4'>
                                <label>Name: </label>
                                <input className='border rounded my-2 col-9 me-4 p-1'
                                       type='text'
                                       placeholder={'User name'}
                                       defaultValue={name}
                                       onInput={e => {
                                           setName(e.currentTarget.value)
                                           // setDisplayName(e.currentTarget.value)
                                       }}
                                       {...register("name", { required: true, minLength: 6, maxLength: 20 })}/>
                                {
                                    errors?.name?.type === "required" &&
                                    <p className='text-danger'>This field is required</p>
                                }
                                {
                                    errors?.name?.type === "minLength" &&
                                    <p className='text-danger'>This field must have at least 6 symbols</p>
                                }
                                {
                                    errors?.name?.type === "maxLength" &&
                                    <p className='text-danger'>This field is too long(maximum length is 20 ch)</p>
                                }
                            </div>
                            <div className='m-0 ms-4'>
                                <label>Email: </label>
                                <input className='border rounded my-2 col-9 me-4 p-1'
                                       type='text'
                                       placeholder={'User email'}
                                       defaultValue={email}
                                       onInput={e => {
                                           setEmail(e.currentTarget.value)
                                       }}
                                       {...register("email", { required: true, minLength: 3, maxLength: 50, pattern: /^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/ })}/>
                                {
                                    errors?.email?.type === "required" &&
                                    <p className='text-danger'>This field is required</p>
                                }
                                {
                                    errors?.email?.type === "minLength" &&
                                    <p className='text-danger'>This field must have at least 3 symbols</p>
                                }
                                {
                                    errors?.email?.type === "maxLength" &&
                                    <p className='text-danger'>This field is too long(maximum length is 50 ch)</p>
                                }
                                {
                                    errors?.email?.type === "pattern" &&
                                    <p className='text-danger'>This should be email.</p>
                                }
                            </div>
                            {
                                !(errors.name || errors.email) ?
                                    <button className='btn btn-primary justify-content-center my-2 ms-4 col-2'
                                            onClick={e => {
                                                e.preventDefault();
                                                saveNameAndEmail(name, email);
                                            }}>
                                        Save
                                    </button>
                                    :
                                    <button className='btn btn-primary justify-content-center col-2 ms-4' disabled>
                                        Save
                                    </button>
                            }
                        </form>
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
                            <button className='btn btn-success rounded ms-4 my-2' onClick={e => {
                                e.preventDefault();
                                unbanUser();
                            }}>Unban</button>
                            :
                            <button className='btn btn-danger rounded ms-4 my-2' onClick={e => {
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