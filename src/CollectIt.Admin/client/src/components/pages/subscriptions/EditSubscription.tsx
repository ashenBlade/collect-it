import React, {useEffect, useState} from 'react';
import {useNavigate, useParams} from "react-router";
import Subscription from "../../entities/subscription";
import SubscriptionsService from "../../../services/SubscriptionsService";
import {Button} from "react-bootstrap";
import {SubmitHandler, useForm} from "react-hook-form";

interface IFormInput {
    name: string;
    description: string;
}

const EditSubscription = () => {
    const {register, handleSubmit, formState: {errors}} = useForm<IFormInput>({mode: 'onBlur'});
    const onSubmit: SubmitHandler<IFormInput> = data => console.log(data);

    const params = useParams();
    const subscriptionId = Number(params.subscriptionId?.trim());
    const nav = useNavigate();
    if (!Number.isInteger(subscriptionId)) {
        alert('Not valid subscription id provided')
        nav('/subscriptions');
    }

    const [subscription, setSubscription] = useState<Subscription>();
    const [name, setName] = useState('');
    const [displayName, setDisplayName] = useState('');
    const [description, setDescription] = useState('');
    const [displayDescription, setDisplayDescription] = useState('');
    const [loaded, setLoaded] = useState(false);
    const [active, setActive] = useState(true);

    useEffect(() => {
        SubscriptionsService.getSubscriptionByIdAsync(subscriptionId).then(s => {
            setDisplayName(s.name);
            setName(s.name);
            setDescription(s.description);
            setDisplayDescription(s.description);
            setActive(s.active);
            setSubscription(s);
            setLoaded(true);
        }).catch(err => {
            console.error(err)
        })
    }, []);

    const saveNameAndDescription = (newName: string,newDescription: string) => {
        if (!subscription) return;
        SubscriptionsService.updateSubscriptionBatchAsync(subscriptionId, newName, newDescription)
            .then(() => {
                setDisplayName(newName)
                setDisplayDescription(newDescription)
            })
            .catch(e => {
                console.error(e)
            }).then(()=>{alert('Subscription updated successfully')})
    }

    const switchSub = () => {
        active ? SubscriptionsService.deactivateSubscriptionAsync(subscriptionId)
                .catch(e => {alert(e)})
                .then(()=>{setActive(false)})
                .then(()=>{alert('Subscription deactivated successfully')})
            : SubscriptionsService.activateSubscriptionAsync(subscriptionId)
                .catch(e => {alert(e)})
                .then(()=>{setActive(true)})
                .then(()=>{alert('Subscription activated successfully')})
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
                                ID: {subscription?.id}
                            </div>
                            <div className='h6 d-block'>
                                Name: {displayName}
                            </div>
                            <div className='h6 d-block'>
                                Description: {displayDescription}
                            </div>
                            <div className='h6 d-block'>
                                Status: {active ? 'Active' : 'Deactivated'}
                            </div>
                            <div className='h6 d-block'>
                                Duration: {subscription?.monthDuration} months
                            </div>
                            <div className='h6 d-block'>
                                Price: {subscription?.price}₽
                            </div>
                        </div>
                        <form onSubmit={handleSubmit(onSubmit)}>
                            Name:
                            <div className='w-100 mx-auto my-2'>
                                <input type='text'
                                       className='form-control mx-1'
                                       defaultValue={displayName}
                                       onInput={e => {
                                           setName(e.currentTarget.value)
                                       }}
                                       {...register("name", {required: true, minLength: 6, maxLength: 20})}/>
                                {errors?.name?.type === "required" &&
                                    <p className='text-danger'>This field is required</p>}
                                {errors?.name?.type === "minLength" &&
                                    <p className='text-danger'>This field must have at least 6 symbols</p>}
                                {errors?.name?.type === "maxLength" &&
                                    <p className='text-danger'>This field is too long (maximum length is 20
                                        characters)</p>}
                            </div>
                            Description:
                            <div className='w-100 mx-auto my-2'>
                                <input type='text'
                                       className='form-control mx-1'
                                       defaultValue={displayDescription}
                                       onInput={e => {
                                           setDescription(e.currentTarget.value)
                                       }}
                                       {...register("description", {required: true, minLength: 10, maxLength: 50})}/>
                                {errors?.description?.type === "required" &&
                                    <p className='text-danger'>This field is required</p>}
                                {errors?.description?.type === "minLength" &&
                                    <p className='text-danger'>This field must have at least 10 symbols</p>}
                                {errors?.description?.type === "maxLength" &&
                                    <p className='text-danger'>This field is too long (maximum length is 50
                                        characters)</p>}
                            </div>
                            {!(errors.name || errors.description) ?
                                <Button type='button' className='btn btn-primary my-2' onClick={e => {
                                    e.preventDefault();
                                    saveNameAndDescription(name, description);
                                }}>
                                    Save
                                </Button>
                                :
                                <Button type='button' className='btn btn-primary my-2' onClick={e => {
                                    e.preventDefault();
                                    saveNameAndDescription(name, description);
                                }} disabled>
                                    Save
                                </Button>
                            }
                        </form>

                        { active ?
                            <Button type ='button' className='btn btn-danger my-2' onClick={e => {
                                e.preventDefault();
                                switchSub();
                            }}>
                                Deactivate
                            </Button>
                            :
                            <Button type ='button' className='btn btn-success my-2' onClick={e => {
                                e.preventDefault();
                                switchSub();
                            }}>
                                Activate
                            </Button>}
                    </div>
                    : <p>Loading...</p>
            }
        </div>
    );
};


export default EditSubscription;