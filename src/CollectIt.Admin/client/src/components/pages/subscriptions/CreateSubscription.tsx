import React, {createElement, useState} from 'react';
import {ResourceType} from "../../entities/resource-type";
import {Multiselect} from "multiselect-react-dropdown";
import {RestrictionType} from "../../entities/restriction-type";

const CreateSubscription = () => {
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [price, setPrice] = useState<number>();
    const [duration, setDuration] = useState<number>();
    const [type, setType] = useState<ResourceType>(ResourceType.Any);
    const [downloadCount, setDownloadCount] = useState<number>();
    const [error, setError] = useState<string>('');
    const options = [RestrictionType.Author, RestrictionType.AllowAll, RestrictionType.DenyAll, RestrictionType.DaysTo,
        RestrictionType.DaysAfter, RestrictionType.Size, RestrictionType.Tags]
    const [res, setRes] = useState<React.ReactElement[]>([]);
    // @ts-ignore
    return (
        <div className='align-items-center justify-content-center shadow border col-6 mt-4 m-auto d-block rounded'>
            <div className='p-3'>
                <form>
                    <p className='h2 mb-3 text-center'>Create subscription</p>
                    <input id='name' className='form-control my-2 mb-3' type='text' placeholder='Name' value={name}
                           onInput={e => setName(e.currentTarget.value)}/>
                    <input id='description' className='form-control my-2 mb-3' type='text' placeholder='Description'
                           onInput={e => setDescription(e.currentTarget.value)}/>
                    <input id='price' className='form-control my-2 mb-3' type='number' placeholder='Price'
                           onInput={e => setPrice(+e.currentTarget.value)}/>
                    <input id='duration' className='form-control my-2 mb-3' type='number' placeholder='Month duration'
                           onInput={e => setDuration(+e.currentTarget.value)}/>
                    <select id='type' className='form-select mb-3'
                            onInput={e => setType(e.currentTarget.value as ResourceType)}>
                        <option value={ResourceType.Any}>Any</option>
                        <option value={ResourceType.Image}>Image</option>
                        <option value={ResourceType.Music}>Music</option>
                        <option value={ResourceType.Video}>Video</option>
                    </select>
                    <input id='download_count' className='form-control my-2 mb-3' type='number' placeholder='Max download count'
                           onInput={e => setDownloadCount(+e.currentTarget.value)}/>
                    <Multiselect  isObject={false} options={options} placeholder={'Restrictions'}
                        onSelect={(selectedList, selectedItem) => {

                            let child:Array<React.ReactElement> = [];
                            switch (selectedItem as RestrictionType){
                                case RestrictionType.AllowAll:
                                case RestrictionType.Author:
                                    child.push(createElement('label', {}, 'Author: '))
                                    child.push(createElement('input', {className: 'form-control',
                                        placeholder: 'Author'}))
                                    break;
                                case RestrictionType.DaysAfter:
                                    child.push(createElement('label', {}, 'Days After: '))
                                    child.push(createElement('input', {className: 'form-control',
                                        placeholder: 'Days After', type: 'number'}))
                                    break;
                                case RestrictionType.DaysTo:
                                    child.push(createElement('label', {}, 'Days To: '))
                                    child.push(createElement('input', {className: 'form-control',
                                        placeholder: 'Days To', type: 'number'}))
                                    break;
                                case RestrictionType.DenyAll:
                                case RestrictionType.Size:
                                    child.push(createElement('label', {}, 'Size: '))
                                    child.push(createElement('input', {className: 'form-control',
                                        placeholder: 'Size', type: 'number'}))
                                    break;
                                case RestrictionType.Tags:
                                    child.push(createElement('label', {}, 'Tags: '))
                                    child.push(createElement('input', {className: 'form-control',
                                    placeholder: 'Tags separated by whitespaces'}))
                                    break;
                            }
                            const element = createElement('div',
                                {id: selectedItem, className: 'row m-0 mb-3 mt-2'}, child);
                            setRes((res) => [...res, element]);
                        }}
                        onRemove={(selectedList, selectedItem) => {
                            const temp: React.ReactElement[] = []
                            const parent = createElement('div', {id: selectedItem});
                            for (let i = 0; i < res.length; i++){
                                if (res[i].props.id != parent.props.id) {
                                    temp.push(res[i])
                                }
                            }
                            // @ts-ignore
                            setRes((res) => [temp]);}}></Multiselect>
                    <div id='restrictions'>
                        {res}
                    </div>

                    <div className={'justify-content-center d-flex'}>
                        <button
                                className='btn btn-primary justify-content-center my-2'>Create
                        </button>
                    </div>

                    {error && <span className={'text-danger d-block text-center'}>{error}</span>}
                </form>
            </div>
        </div>
    );
};

export default CreateSubscription;