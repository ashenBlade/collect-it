import React, {useEffect, useState} from 'react';
import Video from "../../entities/video";
import InputBlock from "../../editBlocksComponents/editInputBlock/InputBlock";
import DeleteButton from "../../UIComponents/deleteButtonComponent/DeleteButton";
import {useNavigate, useParams} from "react-router";
import VideosService from "../../../services/VideosService";


const EditVideo = () => {
    const params = useParams();
    const videoId = Number(params.videoId);
    const nav = useNavigate();
    if (videoId === undefined) nav('/videos');
    const [video, setVideo] = useState<Video | null>(null)
    const [displayName, setDisplayName] = useState('');
    const [name, setName] = useState('');
    const [tags, setTags] = useState<string[]>([]);
    const [loaded, setLoaded] = useState(false);

    useEffect(() => {
        VideosService.getVideoByIdAsync(videoId).then(m => {
            setName(m.name);
            setTags(m.tags);
            setDisplayName(m.name);
            setVideo(m);
            setLoaded(true);
        }).catch(err => {
            alert(err.toString())
        })
    }, []);

    const saveName = (newName: string) => {
        console.log('New name', newName);
        if (!video) return;
        VideosService.changeVideoNameAsync(videoId, newName).then(_ => {
            setName(newName);
            setDisplayName(newName);
        }).catch(_ => {
            alert('Could not change image name. Try later.')
        })
    }

    const saveTags = (newTags: string[]) => {
        console.log('Tags', newTags)
        VideosService.changeVideoTagsAsync(videoId, newTags).then(_ => {
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
                        <p className='h2 text-center'>Edit Resource {displayName}</p>

                        <div className='ms-4 row'>
                            <div className='h6 d-block' style={{width: "50%"}}>
                                ID: {video?.id}
                            </div>
                            <div className='h6 d-block' style={{width: "50%"}}>
                                Filename: {video?.filename}
                            </div>
                            <div className='h6 d-block' style={{width: "50%"}}>
                                Extension: {video?.extension}
                            </div>
                            <div className='h6 d-block' style={{width: "50%"}}>
                                Owner ID: {video?.ownerId}
                            </div>
                            <div className='h6 d-block' style={{width: "50%"}}>
                                Duration: {video?.duration}s
                            </div>
                        </div>

                        <InputBlock id={videoId}
                                    fieldName={'Name'}
                                    placeholder={"Video name"}
                                    initial={name}
                                    onSave={e => saveName(e)}/>
                        <InputBlock id={videoId}
                                    fieldName={'Tags'}
                                    placeholder={"Video tags separated by whitespace"}
                                    initial={tags.join(' ')}
                                    onSave={e => saveTags(e.split(' ').filter(t => t !== ''))} />

                        <DeleteButton className='btn btn-danger justify-content-center my-2 hc-0 ms-4'></DeleteButton>
                    </form>
                :<p>Loading...</p>
            }
        </div>
    );
};

export default EditVideo;