import React, {useEffect, useState} from 'react';
import Video from "../../entities/video";
import VideosService from "../../../services/VideosService";
import Pagination from "../../UI/pagination/Pagination";;

const VideoList = () => {
    let pageSize = 10;
    let pageNumber = 1;
    const [videos, setVideos] = useState<Video[]>([]);
    const [redirectID, SetID] = useState(0);
    useEffect(() => {
        VideosService.getVideosPagedAsync({pageSize, pageNumber}).then(x => {
            setVideos(x.videos);
        });
    });
    const [enteredText, setEnteredText] = useState("");
    const keyDownHandler = (event: React.KeyboardEvent<HTMLInputElement>) => {
        if (event.code === "Enter") {
            window.location.href=(`../videos/${enteredText}`);
        }
    };
    function redirect()
    {
        window.location.href=(`../videos/${redirectID}`)
    }
    const downloadPageNumber = (pageNumber: number) => {
        VideosService.getVideosPagedAsync({pageSize, pageNumber}).then(x => {setVideos(x.videos)})
    }
    return (
        <div>
            <div className='w-75 mt-5 mx-auto'>
                <input id='email' className='form-control my-2' type='text' placeholder='Enter login/e-mail'/>
                <input onKeyDown={keyDownHandler} value={enteredText} onChange={(e) => setEnteredText(e.target.value)} id='email' className='form-control my-2' type='text' placeholder='Enter id'/>
                <tbody className='usersTable mx-auto mt-5'>
                <tr className='firstRow usersRow'>
                    <td className ='idCell color-purple '>ID</td>
                    <td className= 'usersCell color-purple'>Name</td>
                    <td className= 'usersCell color-purple'>OwnerID</td>
                    <td className= 'usersCell color-purple'>FileName</td>
                    <td className= 'usersCell color-purple'>UploadTime</td>
                </tr>
                {videos?.map(i=>
                    <tr onClick={redirect} className ='usersRow'>
                        {SetID(i.id)}
                        <td className ='idCell'>{i.id}</td>
                        <td className ='usersCell'>{i.name}</td>
                        <td className ='usersCell'>{i.ownerId}</td>
                        <td className ='usersCell'>{i.filename}</td>
                        <td className ='usersCell'>{i.uploadDate}</td>
                    </tr>
                )}
                </tbody>
                <Pagination currentPage={1} totalPagesCount={10} onPageChange={downloadPageNumber}/>
            </div>
        </div>
    );
};

export default VideoList;