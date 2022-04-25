import React, {useEffect, useState} from 'react';
import Image from "../../entities/image";
import InputBlock from "../../editBlocksComponents/editInputBlock/InputBlock";
import DeleteButton from "../../UIComponents/deleteButtonComponent/DeleteButton";
import {useNavigate, useParams} from "react-router";
import ImagesService from "../../../services/ImagesService";


const EditImage = () => {
    const params = useParams();
    const imageId = Number(params.imageId);
    const nav = useNavigate();
    if (imageId === undefined) nav('/images');
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

    return (
        <div className='align-items-center justify-content-center shadow border col-6 mt-4 m-auto d-block rounded'>
            {
                loaded ?
                    <form className='col-12 p-3'>
                        <p className='h2 text-center'>{displayName}</p>

                        <div className='ms-4 row'>
                            <div className='h6 d-block' style={{width: "50%"}}>
                                ID: {image?.id}
                            </div>
                            <div className='h6 d-block' style={{width: "50%"}}>
                                Filename: {image?.filename}
                            </div>
                            <div className='h6 d-block' style={{width: "50%"}}>
                                Extension: {image?.extension}
                            </div>
                            <div className='h6 d-block' style={{width: "50%"}}>
                                Owner ID: {image?.ownerId}
                            </div>
                        </div>

                        <InputBlock id={imageId}
                                    fieldName={'Name'}
                                    placeholder={'Image name'}
                                    initial={name}
                                    onSave={e => saveName(e)}/>
                        <InputBlock id={imageId}
                                    fieldName={'Tags'}
                                    placeholder={'Image tags separated by whitespace'}
                                    initial={tags.join(', ')}
                                    onSave={e => saveTags(e.split(' ').filter(t => t !== ''))}/>

                        <DeleteButton className='btn btn-danger justify-content-center my-2 hc-0 ms-4'></DeleteButton>
                    </form>
                : <p>Loading...</p>
            }
        </div>
    );
};

export default EditImage;