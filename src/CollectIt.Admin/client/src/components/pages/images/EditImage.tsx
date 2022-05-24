import React, {useEffect, useState} from 'react';
import Image from "../../entities/image";
import DeleteButton from "../../UI/DeleteButton/DeleteButton";
import {useNavigate, useParams} from "react-router";
import ImagesService from "../../../services/ImagesService";
import {SubmitHandler, useForm} from "react-hook-form";

interface IFormInput {
    name: string;
    tags: string;
}

const EditImage = () => {
    const { register, handleSubmit, formState: { errors } } = useForm<IFormInput>({mode: 'onBlur'});
    const onSubmit: SubmitHandler<IFormInput> = data => console.log(data);

    const params = useParams();
    const imageId = Number(params.imageId?.trim());
    const nav = useNavigate();
    if (!Number.isInteger(imageId))
        nav('/images');
    const [image, setImage] = useState<Image | null>(null);
    const [displayName, setDisplayName] = useState('');
    const [name, setName] = useState('');
    const [tags, setTags] = useState<string[]>([]);
    const [loaded, setLoaded] = useState(false);

    useEffect(() => {
        ImagesService.getImageByIdAsync(imageId).then(i => {
            setName(i.name);
            setTags(i.tags);
            setDisplayName(i.name)
            setImage(i);
            setLoaded(true);
        }).catch(err => {
            alert(err.toString())
        })
    }, []);

    const saveName = (newName: string) => {
        console.log('New name', newName);
        if (!image) return;
        ImagesService.changeImageNameAsync(imageId, newName).then(_ => {
            setName(newName);
            setDisplayName(newName)
        }).catch(_ => {
            alert('Could not change image name. Try later.')
        })
    }

    const saveTags = (newTags: string[]) => {
        console.log('Tags', newTags)
        ImagesService.changeImageTagsAsync(imageId, newTags).then(_ => {
            setTags(newTags);
        }).catch(_ => {
            alert('Could not change image tags. Try later.')
        })
    }

    const saveNameAndTags = (newName: string, newTags: string[]) => {
    }

    const deleteImage = () => {
        if (window.confirm('Delete image?')) {
            ImagesService.deleteImageByIdAsync(imageId).then(() => {
                alert('Image deleted successfully');
                nav('/images');
            }).catch(x => {
                alert(x.message);
            });
        }
    }

    return (
        <div className='align-items-center justify-content-center shadow border col-6 mt-4 m-auto d-block rounded'>
            {
                loaded ?
                    <div className='col-12 p-3'>
                        <p className='h2 text-center'>{displayName}</p>

                        <div className='ms-4'>
                            <div className='h6 d-block'>
                                ID: {image?.id}
                            </div>
                            <div className='h6 d-block'>
                                Filename: {image?.filename}
                            </div>
                            <div className='h6 d-block'>
                                Extension: {image?.extension}
                            </div>
                            <div className='h6 d-block'>
                                Owner ID: {image?.ownerId}
                            </div>
                        </div>

                        <form onSubmit={handleSubmit(onSubmit)}>
                            <div className='m-0 ms-4'>
                                <label>Name: </label>
                                <input className='border rounded my-2 col-9 me-4'
                                       type='text'
                                       placeholder={'Image name'}
                                       defaultValue={name}
                                       onInput={e => {
                                           setName(e.currentTarget.value)
                                           setDisplayName(e.currentTarget.value)
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
                                <label>Tags: </label>
                                <input className='border rounded my-2 col-9 me-4'
                                       type='text'
                                       placeholder={'Image tags separated by whitespace'}
                                       defaultValue={tags.join(', ')}
                                       onInput={e => {
                                           setTags(e.currentTarget.value.split(' ').filter(t => t !== ''))
                                       }}
                                       {...register("tags", { required: true, minLength: 3, maxLength: 50 })}/>
                                {
                                    errors?.tags?.type === "required" &&
                                    <p className='text-danger'>This field is required</p>
                                }
                                {
                                    errors?.tags?.type === "minLength" &&
                                    <p className='text-danger'>This field must have at least 3 symbols</p>
                                }
                                {
                                    errors?.tags?.type === "maxLength" &&
                                    <p className='text-danger'>This field is too long(maximum length is 50 ch)</p>
                                }
                            </div>
                            {
                                !(errors.name || errors.tags) ?
                                <button className='btn btn-primary justify-content-center my-2 col-2'
                                        onClick={e => {
                                            e.preventDefault();
                                            saveNameAndTags(name, tags);
                                        }}>
                                    Save
                                </button>
                                :
                                <button className='btn btn-primary justify-content-center my-2 col-2' disabled>
                                    Save
                                </button>
                            }
                        </form>

                        <DeleteButton onDeleteClick={deleteImage}/>
                    </div>
                : <p>Loading...</p>
            }
        </div>
    );
};

// @ts-ignore
export default EditImage;