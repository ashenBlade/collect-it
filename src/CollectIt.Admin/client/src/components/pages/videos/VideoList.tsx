import React, {useEffect, useState} from 'react';
import Video from "../../entities/video";
import VideosService from "../../../services/VideosService";
import Pagination from "../../UI/Pagination/Pagination";
import {useNavigate} from "react-router";
import SearchPanel from "../../UI/SearchPanel/SearchPanel";

const VideoList = () => {
    let pageSize = 10;
    let pageNumber = 1;
    const [loading, setLoading] = useState(true);
    const [maxPages, setMaxPages] = useState(0);
    const [videos, setVideos] = useState<Video[]>([]);
    useEffect(() => {
        VideosService.getVideosPagedAsync({pageSize, pageNumber}).then(x => {
            setVideos(x.videos);
            const number = Math.ceil(x.totalCount / pageSize);
            console.log('Fucking number', number);
            setMaxPages(number)
            setLoading(false);
        });
    }, []);
    const downloadPageNumber = (pageNumber: number) => {
        setLoading(true);
        VideosService.getVideosPagedAsync({pageSize, pageNumber}).then(x => {
            setVideos(x.videos);
            setLoading(false);
        })
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
        <div className={'container mt-5'}>
            {loading
                ? <>Loading...</>
                : <>
                    <SearchPanel onSearch={onSearch} placeholder={'Enter id of video'}/>
                    <div className='mt-5 mx-auto'>
                        <table className={'table table-borderless table-light'}>
                            <thead className='mx-auto mt-5 table-hover'>
                            <th className='firstRow usersRow'>
                                <td className='Cell idCell color-purple '>ID</td>
                                <td className='Cell nameCell color-purple'>Name</td>
                                <td className='Cell idCell color-purple'>OwnerID</td>
                                <td className='Cell color-purple'>Filename</td>
                                <td className='Cell color-purple'>Upload time</td>
                            </th>
                            </thead>
                            <tbody className='mx-auto mt-5 table-hover'>
                            {videos?.map(i =>
                                <tr onClick={() => toEditVideoPage(i.id)} className='usersRow'>
                                    <td className='Cell idCell'>{i.id}</td>
                                    <td className='Cell nameCell'>
                                        <div className={'bigtext'}> {i.name}</div>
                                    </td>
                                    <td className='Cell idCell'>{i.ownerId}</td>
                                    <td className='Cell'>{i.filename}</td>
                                    <td className='Cell'>{new Date(i.uploadDate).toLocaleString('ru')}</td>
                                </tr>
                            )}
                            </tbody>
                        </table>
                        <Pagination initialPage={1}
                                    totalPagesCount={maxPages}
                                    maxVisibleButtonsCount={10}
                                    onPageChange={downloadPageNumber}/>
                    </div>
                </>
            }
        </div>
    );
};

export default VideoList;