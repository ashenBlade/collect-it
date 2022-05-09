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
    const [description, setDescription] = useState('');
    const [loaded, setLoaded] = useState(false);
    const [active, setActive] = useState(true)

    useEffect(() => {
        SubscriptionsService.getSubscriptionByIdAsync(subscriptionId).then(s => {
            setName(s.name);
            setDescription(s.description);
            setActive(s.active)
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
            setName(newName)
            setDescription(newDescription)
        })
        .catch(e => {
            console.error(e)
        })
    }

    const switchSub = () => {
        subscription?.active ? SubscriptionsService.deactivateSubscriptionAsync(subscriptionId).catch(e => {alert(e)})
            : SubscriptionsService.activateSubscriptionAsync(subscriptionId).catch(e => {alert(e)})
    }

    return (
        <div className='align-items-center justify-content-center shadow border col-6 mt-4 m-auto d-block rounded'>
            {
                loaded ?
                    <div className='col-12 p-3'>
                        <p className='h2 text-center'>{name}</p>
                        <div className='ms-4'>
                            <div className='h6 d-block'>
                                ID: {subscription?.id}
                            </div>
                            <div className='h6 d-block'>
                                Name: {subscription?.name}
                            </div>
                            <div className='h6 d-block'>
                                Description: {description}
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
                                   defaultValue={name}
                                   onChange={e => {setName(e.currentTarget.value)}}/>
                        </div>
                        Description:
                        <div className='d-flex w-100 mx-auto my-2'>
                            <input type='text'
                                   className='form-control mx-1'
                                   defaultValue={description}
                                   onChange={e => {setDescription(e.currentTarget.value)}}/>
                        </div>
                        <Button type ='button'  className='btn btn-primary my-2' onClick={e => {
                            e.preventDefault();
                            saveNameAndDescription(name,description);
                        }}>
                            Save
                        </Button>
                        <Button type ='button' className='btn btn-primary my-2 mx-5' onClick={e => {
                            e.preventDefault();
                            switchSub();
                        }}>
                            Activate/Deactivate
                        </Button>
                    </div>
                    : <p>Loading...</p>
            }
        </div>
    );
};

export default EditSubscription;