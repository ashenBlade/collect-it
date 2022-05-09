import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from "react-router";
import Subscription from "../../entities/subscription";
import SubscriptionsService from "../../../services/SubscriptionsService";
import { Button } from "react-bootstrap";

const EditSubscription = () => {
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
                        Name:
                        <div className='d-flex w-100 mx-auto my-2'>
                            <input type='text'
                                   className='form-control mx-1'
                                   defaultValue={displayName}
                                   onChange={e => {setName(e.currentTarget.value)}}/>
                        </div>
                        Description:
                        <div className='d-flex w-100 mx-auto my-2'>
                            <input type='text'
                                   className='form-control mx-1'
                                   defaultValue={displayDescription}
                                   onChange={e => {setDescription(e.currentTarget.value)}}/>
                        </div>
                        <Button type ='button'  className='btn btn-primary my-2' onClick={e => {
                            e.preventDefault();
                            saveNameAndDescription(name,description);
                        }}>
                            Save
                        </Button>
                        { active ?
                        <Button type ='button' className='btn btn-danger my-2 mx-5' onClick={e => {
                            e.preventDefault();
                            switchSub();
                        }}>
                            Deactivate
                        </Button>
                        :
                        <Button type ='button' className='btn btn-success my-2 mx-5' onClick={e => {
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