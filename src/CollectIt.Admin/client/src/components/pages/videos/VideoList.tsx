import React, { useEffect, useState } from 'react';
import Video from "../../entities/video";
import VideosService from "../../../services/VideosService";
import Pagination from "../../UI/Pagination/Pagination";
import { useNavigate } from "react-router";
import SearchPanel from "../../UI/SearchPanel/SearchPanel";
import ReactLoading from "react-loading";

const VideoList = () => {
    let pageSize = 10;

    const [loading, setLoading] = useState(true);
    const [maxPages, setMaxPages] = useState(0);
    const [videos, setVideos] = useState<Video[]>([]);

    useEffect(() => {
        VideosService.getVideosPagedAsync({pageSize, pageNumber:1}).then(x => {
            setVideos(x.videos);
            setMaxPages(Math.ceil(x.totalCount / pageSize))
            setLoading(false);
        }).catch(_ => setLoading(false))
    }, [])

    const downloadPageNumber = (pageNumber: number) => {
        setLoading(true);
        VideosService.getVideosPagedAsync({pageSize, pageNumber}).then(x => {
            setVideos(x.videos);
            setLoading(false);
        }).catch(_ => setLoading(false))
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
                ? <ReactLoading className={'mx-auto'} type={'spinningBubbles'} color={'black'} height='200px' width='200px' />
                : <>
                    <SearchPanel onSearch={onSearch} placeholder={'Enter id of video'}/>
                    <div className='mt-5 mx-auto'>
                        <table className='table table-hover table-striped table-w-100'>
                            <thead>
                            <tr>
                                <td>Id</td>
                                <td>Name</td>
                                <td>OwnerId</td>
                                <td>Filename</td>
                                <td>Upload date</td>
                            </tr>
                            </thead>
                            <tbody>
                            {videos.map(v =>
                                <tr onClick={() => toEditVideoPage(v.id)} style={{cursor: "pointer"}}>
                                    <td>{v.id}</td>
                                    <td className="cell-overflow">{v.name}</td>
                                    <td>{v.ownerId}</td>
                                    <td className="cell-overflow">{v.filename}</td>
                                    <td>{new Date(v.uploadDate).toLocaleDateString('ru')}</td>
                                </tr>
                            )}
                            </tbody>
                        </table>
                    </div>
                </>
            }
            <footer className={'footer fixed-bottom d-flex mb-0 justify-content-center'}>
                <Pagination totalPagesCount={maxPages} onPageChange={downloadPageNumber}/>
            </footer>
        </div>
    );
};

export default VideoList;