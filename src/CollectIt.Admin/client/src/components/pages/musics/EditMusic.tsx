import React, {useEffect, useState} from 'react';
import Music from "../../entities/music";
import DeleteButton from "../../UI/DeleteButton/DeleteButton";
import {useNavigate, useParams} from "react-router";
import MusicsService from "../../../services/MusicsService";
import {SubmitHandler, useForm} from "react-hook-form";

interface IFormInput {
    name: string;
    tags: string;
}

const EditMusic = () => {
    const { register, handleSubmit, formState: { errors } } = useForm<IFormInput>({mode: 'onBlur'});
    const onSubmit: SubmitHandler<IFormInput> = data => console.log(data);

    const params = useParams();
    const musicId = Number(params.musicId?.trim());
    const nav = useNavigate();
    if (!Number.isInteger(musicId))
        nav('/musics');
    const [music, setMusic] = useState<Music | null>(null)
    const [displayName, setDisplayName] = useState('');
    const [name, setName] = useState('');
    const [tags, setTags] = useState<string[]>([]);
    const [loaded, setLoaded] = useState(false);

    useEffect(() => {
        MusicsService.getMusicByIdAsync(musicId).then(m => {
            setName(m.name);
            setTags(m.tags);
            setDisplayName(m.name);
            setMusic(m);
            setLoaded(true);
        }).catch(err => {
            alert(err.toString())
        })
    }, []);

    const saveName = (newName: string) => {
        if (!music) return;
        MusicsService.changeMusicNameAsync(musicId, newName).then(_ => {
            setName(newName);
            setDisplayName(newName);
        }).catch(_ => {
            alert('Could not change music name. Try later.')
        })
    }

    const saveTags = (newTags: string[]) => {
        MusicsService.changeMusicTagsAsync(musicId, newTags).then(_ => {
            setTags(newTags);
        }).catch(_ => {
            alert('Could not change music tags. Try later.')
        })
    }

    const saveNameAndTags = (newName: string, newTags: string[]) => {
    }

    const deleteMusic = () => {
        if (window.confirm('Delete music?')) {
            MusicsService.deleteMusicByIdAsync(musicId).then(_ => {
                alert('Music deleted successfully');
                nav('/musics')
            }).catch(x => {
                console.error(x);
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
                                ID: {music?.id}
                            </div>
                            <div className='h6 d-block'>
                                Filename: {music?.filename}
                            </div>
                            <div className='h6 d-block'>
                                Extension: {music?.extension}
                            </div>
                            <div className='h6 d-block'>
                                Owner ID: {music?.ownerId}
                            </div>
                            <div className='h6 d-block'>
                                Duration: {music?.duration} seconds
                            </div>
                        </div>

                        <form onSubmit={handleSubmit(onSubmit)}>
                            <div className='m-0 ms-4'>
                                <label>Name: </label>
                                <input className='border rounded my-2 col-9 me-4'
                                       type='text'
                                       placeholder={'Music name'}
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
                                       placeholder={'Music tags separated by whitespace'}
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

                        <DeleteButton onDeleteClick={deleteMusic}/>
                    </div>
                    : <p>Loading...</p>
            }
        </div>
    );
};

export default EditMusic;