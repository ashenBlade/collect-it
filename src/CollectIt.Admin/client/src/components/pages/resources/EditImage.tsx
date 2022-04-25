import React, {useEffect, useState} from 'react';
import Image from "../../entities/image";
import DeleteButton from "../../UIComponents/deleteButtonComponent/DeleteButton";
import InputBlock from "../../editBlocksComponents/editInputBlock/InputBlock";
import {useParams, useNavigate} from "react-router";
import ImagesService from "../../../services/ImagesService";

const EditImage = () => {
    const params = useParams();
    const imageId = Number(params.imageId);
    const nav = useNavigate();
    if (imageId === undefined) {
        nav('/images');
    }
    // const image = new Image(1, 'New', new Date(), 'Dog',
    //     ["red", "green", "blue"], 'jpg', 1);
    const [image, setImage] = useState<Image | null>(null);
    // const [name,setName] = useState<string|undefined>(image.name)
    const [name, setName] = useState('');
    const [tags, setTags] = useState<string[]>([]);

    useEffect(() => {
        ImagesService.getImageByIdAsync(imageId).then(i => {
            setImage(i);
            setName(i.name);
            setTags(i.tags);
        }).catch(err => {
            alert(err.toString())
        })
    }, []);

    const saveName = (newName: string) => {
        ImagesService.changeImageNameAsync(imageId, newName).then(_ => {
            setName(newName);
        }).catch(_ => {
            alert('Could not change image name. Try later.')
        })
    }

    const saveTags = (newTags: string[]) => {
        ImagesService.changeImageTagsAsync(imageId, newTags).then(_ => {
            setTags(newTags);
        }).catch(_ => {
            alert('Could not change image tags. Try later.')
        })
    }

    return (
        <div className='align-items-center justify-content-center shadow border col-6 mt-4 m-auto d-block rounded'>
            <form className='col-12 p-3'>
                <p className='h2 text-center'>{name}</p>

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

                <InputBlock fieldName={'Name'}
                            id={image?.id}
                            placeholder={'Image name'}
                            initial={name}
                            onSave={saveName}/>
                <InputBlock id={image?.id}
                            fieldName={'Tags'}
                            placeholder={'Image tags separated by whitespace'}
                            initial={tags.join(', ')}
                            onSave={t => saveTags(t.split(' ').filter(x => x !== ''))}/>
                <DeleteButton className='btn btn-danger justify-content-center my-2 hc-0 ms-4'></DeleteButton>
            </form>
        </div>
    );
};

export default EditImage;