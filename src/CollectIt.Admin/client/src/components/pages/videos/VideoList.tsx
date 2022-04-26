import React, {useEffect, useState} from 'react';
import Video from "../../entities/video";
import VideosService from "../../../services/VideosService";
import Pagination from "../../UI/pagination/Pagination";
import {useNavigate} from "react-router";
import SearchPanel from "../../UI/SearchPanel/SearchPanel";

;

const VideoList = () => {
    let pageSize = 10;
    let pageNumber = 1;
    const [videos, setVideos] = useState<Video[]>([]);
    useEffect(() => {
        VideosService.getVideosPagedAsync({pageSize, pageNumber}).then(x => {
            setVideos(x.videos);
        });
    });
    const downloadPageNumber = (pageNumber: number) => {
        VideosService.getVideosPagedAsync({pageSize, pageNumber}).then(x => {setVideos(x.videos)})
    }
    const nav = useNavigate();
    const toEditVideoPage = (id: number) => nav(`/videos/${id}`);
    const onSearch = (q: string) => {
        const id = Number(q);
        if (!Number.isInteger(id)) {
            alert('Id must be an integer');
            return;
        }
        toEditVideoPage(id);
    }
    return (
        <div  className={'container mt-5'}>
            <SearchPanel onSearch={onSearch} placeholder={'Enter id of video'}/>
            <div className='mt-5 mx-auto'>
                <table className={'table table-borderless table-light'}>
                    <thead>
                    <th className='firstRow usersRow'>
                        <td className ='idCell color-purple '>ID</td>
                        <td className= 'usersCell color-purple'>Name</td>
                        <td className= 'usersCell color-purple'>OwnerID</td>
                        <td className= 'usersCell color-purple'>Filename</td>
                        <td className= 'usersCell color-purple'>Upload time</td>
                    </th>
                    </thead>
                    <tbody className='mx-auto mt-5 table-hover'>
                    {videos?.map(i=>
                        <tr onClick={() => toEditVideoPage(i.id)} className ='usersRow'>
                            <td className ='idCell'>{i.id}</td>
                            <td className ='usersCell'>{i.name}</td>
                            <td className ='usersCell'>{i.ownerId}</td>
                            <td className ='usersCell'>{i.filename}</td>
                            <td className ='usersCell'>{new Date(i.uploadDate).toLocaleString('ru')}</td>
                        </tr>
                    )}
                    </tbody>
                </table>
                <Pagination currentPage={1} totalPagesCount={10} onPageChange={downloadPageNumber}/>
            </div>
        </div>
    );
};

export default VideoList;