import React, { useState } from 'react';
import { ResourceType } from "../../entities/resource-type";
import { RestrictionType } from "../../entities/restriction-type";
import { FormSelect } from "react-bootstrap";
import { SubmitHandler, useForm } from "react-hook-form";
import { useNavigate } from "react-router";
import SubscriptionsService from "../../../services/SubscriptionsService";

interface IFormInput {
    name: string;
    description: string;
    duration: number;
    price: number;
    count: number;
}

const CreateSubscription = () => {
    const { register, handleSubmit, formState: { errors } } = useForm<IFormInput>({mode: 'onBlur'});
    const onSubmit: SubmitHandler<IFormInput> = data => console.log(data);

    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [duration, setDuration] = useState(0);
    const [price, setPrice] = useState(0);
    const [type, setType] = useState<ResourceType>(ResourceType.Any);
    const [downloadCount, setDownloadCount] = useState(0);
    const [error, setError] = useState('');
    const NoneRestriction = 'None';
    const options = [
        RestrictionType.AllowAll,
        RestrictionType.DenyAll,
        RestrictionType.DaysTo,
        RestrictionType.DaysAfter,
        RestrictionType.Size,
        RestrictionType.Tags
    ];
    const [currentRestriction, setCurrentRestriction] = useState(RestrictionType.None);
    const [daysAfter, setDaysAfter] = useState(0);
    const [daysTo, setDaysTo] = useState(0);
    const [size, setSize] = useState(0);
    const [tags, setTags] = useState('');
    const inputClassList = 'form-control my-2 mb-3';

    const onRestrictionChange = (restriction: string) => {
        const number = Number(restriction);
        const type = number as RestrictionType;
        setCurrentRestriction(type);
    }

    const getCurrentRestrictionDTO = () => {
        switch (currentRestriction) {
            case RestrictionType.None: {
                return null;
            }
            case RestrictionType.Size:
                if (size < 1) throw Error('Size must be positive');
                return {
                    restrictionType: RestrictionType.Size,
                    size: size
                };
            case RestrictionType.DaysAfter:
                if (daysAfter < 1) throw Error('DaysAfter must be positive');
                return {
                    restrictionType: RestrictionType.DaysAfter,
                    daysAfter: daysAfter
                };
            case RestrictionType.DaysTo:
                if (daysTo < 1) throw Error('DaysTo must be positive');
                return {
                    restrictionType: RestrictionType.DaysTo,
                    daysTo: daysTo
                }
            case RestrictionType.Tags:
                if (tags.trim().length === 0) throw Error('Tags must be provided');
                const t = tags.split(' ').filter(t => t !== '')
                if (t.some(t => t.length > 20)) throw Error('Max tag size is 20');
                return {
                    restrictionType: RestrictionType.Tags,
                    tags: t
                }
            case RestrictionType.DenyAll:
                return {
                    restrictionType: RestrictionType.DenyAll,
                }
            case RestrictionType.AllowAll:
                return {
                    restrictionType: RestrictionType.AllowAll
                };
            default:
                throw new Error('Unsupported restriction type')
        }


    }
    const nav = useNavigate()

    const onClickCreateButton = async (e : React.MouseEvent) => {
        e.preventDefault();
        const nameCleaned = name.trim();
        const descriptionCleaned = description.trim();
        const priceCleaned = price;
        const monthDurationCleaned = duration;
        const resourceTypeCleaned = type;
        const downloadCountCleaned = downloadCount;
        await SubscriptionsService.createSubscriptionAsync({
            name: nameCleaned,
            description: descriptionCleaned,
            price: priceCleaned,
            monthDuration: monthDurationCleaned,
            resourceType: resourceTypeCleaned,
            maxResourcesCount: downloadCountCleaned,
            restriction: getCurrentRestrictionDTO()
        }).then(x => {
            alert('Subscription created successfully')
            nav(`/subscriptions/${x.id}`)
        }).catch(err => {
            console.error(err)
            alert('Could not create subscription')
        })

    }

    const [daysToError, setDaysToError] = useState('');
    const [daysAfterError, setDaysAfterError] = useState('');
    const [sizeError, setSizeError] = useState('');
    const [tagsError, setTagsError] = useState('');


    return (
        <div className='align-items-center justify-content-center shadow border col-6 mt-4 m-auto d-block rounded'>
            <div className='p-3'>
                <form onSubmit={handleSubmit(onSubmit)}>
                    <p className='h2 mb-3 text-center'>Create subscription</p>
                    <label>Name:</label>
                    <input className={inputClassList}
                           type='text'
                           placeholder='Name'
                           value={name}
                           onInput={e => setName(e.currentTarget.value)}
                           {...register("name", { required: true, minLength: 6, maxLength: 20 })}/>
                    {errors?.name?.type === "required" && <p className='text-danger'>This field is required</p>}
                    {errors?.name?.type === "minLength" && <p className='text-danger'>This field must have at least 6 symbols</p>}
                    {errors?.name?.type === "maxLength" && <p className='text-danger'>This field is too long</p>}
                    <label>Description:</label>
                    <input className={inputClassList}
                           type='text'
                           placeholder='Description'
                           value={description}
                           onInput={e => setDescription(e.currentTarget.value)}
                           {...register("description", { required: true, minLength: 10, maxLength: 50 })}/>
                    {errors?.description?.type === "required" && <p className='text-danger'>This field is required</p>}
                    {errors?.description?.type === "minLength" && <p className='text-danger'>This field must have at least 10 symbols</p>}
                    {errors?.description?.type === "maxLength" && <p className='text-danger'>This field is too long</p>}
                    <label>Price:</label>
                    <input className={inputClassList}
                           type='number'
                           placeholder='Price'
                           value={price}
                           onInput={e => setPrice(+e.currentTarget.value)}
                           {...register("price", { required: true, min: 0, max: 10000 })}/>
                    {errors?.price?.type === "required" && <p className='text-danger'>This field is required</p>}
                    {errors?.price?.type === "min" && <p className='text-danger'>This field must be not negative</p>}
                    {errors?.price?.type === "max" && <p className='text-danger'>This field must be less then 10000</p>}
                    <label>Month duration:</label>
                    <input className={inputClassList}
                           type='number'
                           value={duration}
                           placeholder='Month duration'
                           onInput={e => setDuration(+e.currentTarget.value)}
                           {...register("duration", { required: true, min: 1, max: 1000 })}/>
                    {errors?.duration?.type === "required" && <p className='text-danger'>This field is required</p>}
                    {errors?.duration?.type === "min" && <p className='text-danger'>This field must be bigger then 1</p>}
                    {errors?.duration?.type === "max" && <p className='text-danger'>This field must be less then 1000</p>}
                    <label>Max download count:</label>
                    <input className={inputClassList}
                           type='number'
                           value={downloadCount}
                           placeholder='Max download count'
                           onInput={e => setDownloadCount(+e.currentTarget.value)}
                           {...register("count", { required: true, min: 1, max: 1000 })}/>
                    {errors?.count?.type === "required" && <p className='text-danger'>This field is required</p>}
                    {errors?.count?.type === "min" && <p className='text-danger'>This field must be bigger then 1</p>}
                    {errors?.count?.type === "max" && <p className='text-danger'>This field must be less then 1000</p>}
                    <label>Resource type:</label>
                    <select className='form-select mb-3'
                            onInput={e => setType(e.currentTarget.value as ResourceType)}>
                        <option value={ResourceType.Any}>Any</option>
                        <option value={ResourceType.Image}>Image</option>
                        <option value={ResourceType.Music}>Music</option>
                        <option value={ResourceType.Video}>Video</option>
                    </select>
                    <label>Restriction:</label>
                    <FormSelect onChange={e => {
                        onRestrictionChange(e.currentTarget.value);
                    }}>
                        <option defaultChecked={true} value={NoneRestriction}>None</option>
                        <option value={RestrictionType.DaysTo}>Days To</option>
                        <option value={RestrictionType.DaysAfter}>Days After</option>
                        <option value={RestrictionType.AllowAll}>Allow all</option>
                        <option value={RestrictionType.DenyAll}>Deny all</option>
                        <option value={RestrictionType.Size}>Max size</option>
                        <option value={RestrictionType.Tags}>Tags</option>

                    </FormSelect>

                    {
                        currentRestriction === RestrictionType.DaysAfter &&
                        <>
                            <input value={ daysAfter } type={ 'number' }
                                 className={ inputClassList }
                                 onInput={ e => {
                                     let value = Number(e.currentTarget.value);
                                     if (value < 1) setDaysAfterError('Max days after publishing must be positive')
                                     else setDaysAfterError('')
                                     setDaysAfter(value)
                                 } }
                                 placeholder={ 'Days must last after resource upload' }/>
                            <span className={ 'text-danger text-center' }>{ daysAfterError }</span>
                        </>
                    }
                    {
                        currentRestriction === RestrictionType.DaysTo &&
                        <>
                            <input value={ daysTo } type={ 'number' }
                                 className={ inputClassList }
                                 onInput={ e => {
                                     const daysTo = Number(e.currentTarget.value);
                                     if (daysTo < 1) setDaysToError('Max days close to publishing must be positive')
                                     else setDaysToError('')
                                     setDaysTo(daysTo)
                                 } }
                                 placeholder={ 'Days after upload resource can be purchased' }/>
                            <span className={ 'text-danger text-center' }>{ daysToError }</span>
                        </>
                    }
                    {
                        currentRestriction === RestrictionType.Size &&
                        <>
                            <input value={ size } type={ 'number' }
                                  className={ inputClassList }
                                  onInput={ e => {

                                      let size = Number(e.currentTarget.value);
                                      if (size < 1)
                                          setSizeError('Max size can must be positive')
                                      else
                                          setSizeError('')
                                      setSize(size)
                                  } }
                                  placeholder={ 'Max size of resource to be downloaded' }/>
                            <span className={ 'text-danger text-center' }>{ sizeError }</span>
                        </>
                    }
                    {
                        currentRestriction === RestrictionType.Tags &&
                        <>
                            <input value={ tags } type={ 'text' }
                                 className={ inputClassList }
                                 onInput={ e => {
                                     const newTags = e.currentTarget.value;
                                     const trimmed = newTags.trim();
                                     if (trimmed.length === 0)
                                         setTagsError('Tags must be provided')
                                     else if (trimmed.split(' ').filter(t => t !== '').some(t => t.length > 20))
                                         setTagsError('Max size per tag is 20')
                                     else
                                         setTagsError('')
                                     setTags(newTags)
                                 }
                            }
                                 placeholder={ 'Tags must resource be tagged by to purchase resource' }/>
                            <span className={ 'text-danger text-center' }>{ tagsError }</span>
                        </>

                    }

                    <div className={'justify-content-center d-flex'}>
                        {!(errors.name || errors.description || errors.price || errors.count || errors.duration)?
                            <button className='btn btn-primary justify-content-center my-2'
                                         onClick={onClickCreateButton}>
                                Create
                            </button>
                            :
                            <button className='btn btn-primary justify-content-center my-2'
                                    disabled>
                                Create
                            </button>
                        }
                    </div>

                    {error && <span className={'text-danger d-block text-center'}>{error}</span>}
                </form>
            </div>
        </div>
    );
};

export default CreateSubscription;