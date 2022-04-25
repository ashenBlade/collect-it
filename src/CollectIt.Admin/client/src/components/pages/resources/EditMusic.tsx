import React, {useEffect, useState} from 'react';
import Music from "../../entities/music";
import InputBlock from "../../editBlocksComponents/editInputBlock/InputBlock";
import DeleteButton from "../../UIComponents/deleteButtonComponent/DeleteButton";
import {useNavigate, useParams} from "react-router";
import MusicsService from "../../../services/MusicsService";


const EditMusic = () => {
    const params = useParams();
    const musicId = Number(params.musicId);
    const nav = useNavigate();
    if (musicId === undefined) nav('/musics');
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
            console.log('New name', newName);
            if (!music) return;
        MusicsService.changeMusicNameAsync(musicId, newName).then(_ => {
            setName(newName);
            setDisplayName(newName);
        }).catch(_ => {
            alert('Could not change image name. Try later.')
        })
    }

    const saveTags = (newTags: string[]) => {
        console.log('Tags', newTags)
        MusicsService.changeMusicTagsAsync(musicId, newTags).then(_ => {
            setTags(newTags);
        }).catch(_ => {
            alert('Could not change image tags. Try later.')
        })
    }

    return (
        <div className='align-items-center justify-content-center shadow border col-6 mt-4 m-auto d-block rounded'>
            {loaded
                ? <form className='col-12 p-3'>
                <p className='h2 text-center'>{displayName}</p>

                <div className='ms-4 row'>
                    <div className='h6 d-block' style={{width: "50%"}}>
                        ID: {music?.id}
                    </div>
                    <div className='h6 d-block' style={{width: "50%"}}>
                        Filename: {music?.filename}
                    </div>
                    <div className='h6 d-block' style={{width: "50%"}}>
                        Extension: {music?.extension}
                    </div>
                    <div className='h6 d-block' style={{width: "50%"}}>
                        Owner ID: {music?.ownerId}
                    </div>
                    <div className='h6 d-block' style={{width: "50%"}}>
                        Duration: {music?.duration} seconds
                    </div>
                </div>

                <InputBlock id={musicId}
                            fieldName={'Name'}
                            placeholder={"Music name"}
                            initial={name}
                            onSave={e => saveName(e)}/>
                <InputBlock id={musicId}
                            fieldName={'Tags'}
                            placeholder={"Music tags separated by whitespace"}
                            initial={tags.join(' ')}
                            onSave={e => saveTags(e.split(' ').filter(t => t !== ''))} />

                <DeleteButton className='btn btn-danger justify-content-center my-2 hc-0 ms-4'/>
            </form>
            : <p>Loading...</p>}
        </div>
    );
};

export default EditMusic;