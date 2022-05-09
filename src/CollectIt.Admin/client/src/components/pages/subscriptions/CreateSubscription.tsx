import React, { useState } from 'react';
import { ResourceType } from "../../entities/resource-type";
import { RestrictionType } from "../../entities/restriction-type";
import { FormSelect } from "react-bootstrap";
import {AuthService} from "../../../services/AuthService";

const CreateSubscription = () => {
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [price, setPrice] = useState<number>();
    const [duration, setDuration] = useState<number>();
    const [type, setType] = useState<ResourceType>(ResourceType.Any);
    const [downloadCount, setDownloadCount] = useState<number>();
    const [error, setError] = useState<string>('');
    const NoneRestriction = 'None';
    const options = [
        RestrictionType.AllowAll,
        RestrictionType.DenyAll,
        RestrictionType.DaysTo,
        RestrictionType.DaysAfter,
        RestrictionType.Size,
        RestrictionType.Tags
    ];
    const [currentRestriction, setCurrentRestriction] = useState(NoneRestriction);
    const [daysAfter, setDaysAfter] = useState(0);
    const [daysTo, setDaysTo] = useState(0);
    const [size, setSize] = useState(0);
    const [tags, setTags] = useState('');
    const inputClassList = 'form-control my-2 mb-3';

    const onRestrictionChange = (restriction: string) => {
        setCurrentRestriction(restriction);
    }

    const onClickCreateButton = async (e : React.MouseEvent) => {
        e.preventDefault();
        const nameCleaned = name.trim();
        const descriptionCleaned = description.trim();
        const priceCleaned = price;
        const monthDurationCleaned = duration;
        const resourceTypeCleaned = type;
        const downloadCountCleaned = downloadCount;
        const response = await fetch('http://localhost:7000/subscriptions/create-subscription', {
            method: 'POST',
            body: JSON.stringify({
                name: nameCleaned,
                description: descriptionCleaned,
                duration: monthDurationCleaned,
                price: priceCleaned,
                appliedResourceType: resourceTypeCleaned,
                maxResourcesCount: downloadCountCleaned
            }),
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json'
            },
        });
        if (!response.ok) {
            const description = await response.json();
            const message = description.message;
            console.error(`Response failed with code ${response.status}. Description: ${message}`);
            setError(message);
            return;
        }
    }

    return (
        <div className='align-items-center justify-content-center shadow border col-6 mt-4 m-auto d-block rounded'>
            <div className='p-3'>
                <form>
                    <p className='h2 mb-3 text-center'>Create subscription</p>
                    <input className={inputClassList}
                           type='text'
                           placeholder='Name'
                           value={name}
                           onInput={e => setName(e.currentTarget.value)}
                           required/>
                    <input className={inputClassList}
                           type='text'
                           placeholder='Description'
                           value={description}
                           onInput={e => setDescription(e.currentTarget.value)}/>
                    <input className={inputClassList}
                           type='number'
                           placeholder='Price'
                           value={price}
                           onInput={e => setPrice(+e.currentTarget.value)}
                           required/>
                    <input className={inputClassList}
                           type='number'
                           value={duration}
                           placeholder='Month duration'
                           onInput={e => setDuration(+e.currentTarget.value)}/>
                    <input className={inputClassList}
                           type='number'
                           value={downloadCount}
                           placeholder='Max download count'
                           onInput={e => setDownloadCount(+e.currentTarget.value)}/>
                    <select className='form-select mb-3'
                            onInput={e => setType(e.currentTarget.value as ResourceType)}>
                        <option value={ResourceType.Any}>Any</option>
                        <option value={ResourceType.Image}>Image</option>
                        <option value={ResourceType.Music}>Music</option>
                        <option value={ResourceType.Video}>Video</option>
                    </select>
                    <FormSelect onChange={e => {
                        onRestrictionChange(e.currentTarget.value);
                    }}>
                        <option defaultChecked={true} value={NoneRestriction}>{NoneRestriction}</option>
                        {options.map(o => <option value={o}>{o}</option>)}
                    </FormSelect>

                    {
                        currentRestriction === RestrictionType.DaysAfter &&
                        <input value={daysAfter} type={'number'}
                               className={inputClassList}
                               onInput={e => setDaysAfter(Number(e.currentTarget.value))}
                               placeholder={'Days must last after resource upload'}/>
                    }
                    {
                        currentRestriction === RestrictionType.DaysTo &&
                        <input value={daysTo} type={'number'}
                               className={inputClassList}
                               onInput={e => setDaysTo(Number(e.currentTarget.value)) }
                               placeholder={'Days after upload resource can be purchased'}/>
                    }
                    {
                        currentRestriction === RestrictionType.Size &&
                        <input value={size} type={'number'}
                               className={inputClassList}
                               onInput={e => setSize(Number(e.currentTarget.value)) }
                               placeholder={'Max size of resource to be downloaded'}/>
                    }
                    {
                        currentRestriction === RestrictionType.Tags &&
                        <input value={tags} type={'text'}
                               className={inputClassList}
                               onInput={e => setTags(e.currentTarget.value) }
                               placeholder={'Tags must resource be tagged by to purchase resource'}/>
                    }

                    <div className={'justify-content-center d-flex'}>
                        <button className='btn btn-primary justify-content-center my-2'
                                onClick={onClickCreateButton}>
                            Create
                        </button>
                    </div>

                    {error && <span className={'text-danger d-block text-center'}>{error}</span>}
                </form>
            </div>
        </div>
    );
};

export default CreateSubscription;