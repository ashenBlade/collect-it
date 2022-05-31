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
                        {/*<table className={'table table-borderless table-light'}>*/}
                        {/*    <thead>*/}
                        {/*    <th className='firstRow usersRow'>*/}
                        {/*        <td className='Cell w-10'>ID</td>*/}
                        {/*        <td className='Cell w-35'>Name</td>*/}
                        {/*        <td className='Cell w-10'>OwnerID</td>*/}
                        {/*        <td className='Cell w-10'>Filename</td>*/}
                        {/*        <td className='Cell w-10'>Upload time</td>*/}
                        {/*    </th>*/}
                        {/*    </thead>*/}
                        {/*    <tbody className='mx-auto mt-5 table-hover'>*/}
                        {/*    {videos?.map(i =>*/}
                        {/*        <tr onClick={() => toEditVideoPage(i.id)} className='usersRow'>*/}
                        {/*            <td className='Cell w-10'>{i.id}</td>*/}
                        {/*            <td className='Cell w-35'>*/}
                        {/*                <span className='bigtext'>{i.name}</span>*/}
                        {/*            </td>*/}
                        {/*            <td className='Cell w-10'>{i.ownerId}</td>*/}
                        {/*            <td className='Cell w-10'>{i.filename}</td>*/}
                        {/*            <td className='Cell w-10'>{new Date(i.uploadDate).toLocaleString('ru')}</td>*/}
                        {/*        </tr>*/}
                        {/*    )}*/}
                        {/*    </tbody>*/}
                        {/*</table>*/}
                        <table style={{
                            tableLayout: 'auto',
                            width: '100%'
                        }} className='table table-hover table-striped'>
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
                            { videos.map(v =>
                                <tr>
                                    <td>{v.id}</td>
                                    <td style={{
                                        maxWidth: '25vw',
                                        textOverflow: 'ellipsis',
                                        overflow: 'hidden',
                                        whiteSpace: 'nowrap'
                                    }}>{v.name}</td>
                                    <td>{v.ownerId}</td>
                                    <td style={{
                                        maxWidth: '25vw',
                                        textOverflow: 'ellipsis',
                                        overflow: 'hidden',
                                        whiteSpace: 'nowrap'
                                    }}>{v.filename}</td>
                                    <td>{new Date(v.uploadDate).toLocaleDateString('ru')}</td>
                                </tr>
                            ) }
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