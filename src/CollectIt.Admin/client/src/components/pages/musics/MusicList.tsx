import React, {useEffect, useState} from 'react';
import MusicsService from '../../../services/MusicsService'
import Music from "../../entities/music";
import Pagination from "../../UI/Pagination/Pagination";
import {useNavigate} from "react-router";
import SearchPanel from "../../UI/SearchPanel/SearchPanel";
import ReactLoading from "react-loading";


const MusicList = () => {
    const pageSize = 10;

    const [musics, setMusics] = useState<Music[]>([]);
    const [maxPages, setMaxPages] = useState(0);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        MusicsService.getMusicsPagedAsync({pageSize,pageNumber:1}).then(x => {
            setMusics(x.musics);
            setMaxPages(Math.ceil(x.totalCount / pageSize));
            setLoading(false)
        }).catch(_ => setLoading(false))
    }, [])

    const downloadPageNumber = (pageNumber: number) => {
        setLoading(true)
        MusicsService.getMusicsPagedAsync({pageSize, pageNumber}).then(x => {
            setMusics(x.musics);
            setLoading(false);
        }).catch(_ => setLoading(false))
    }

    const nav = useNavigate();
    const toEditMusicPage = (id: number) => nav(`/musics/${id}`);

    const onSearch = (q: string) => {
        const id = Number(q);
        if (!Number.isInteger(id)) {
            alert('Id must be an integer');
            return;
        }
        toEditMusicPage(id);
    }

    return (
        <div className={'container mt-5'}>
            {loading
                ? <><ReactLoading className={'mx-auto'} type={'spinningBubbles'} color={'black'} height='200px' width='200px' /></>
                : <>
                    <SearchPanel onSearch={onSearch} placeholder={'Enter id of music'}/>
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
                            {musics?.map(u =>
                                <tr onClick={() => toEditMusicPage(u.id)} style={{cursor: "pointer"}}>
                                    <td>{u.id}</td>
                                    <td className="cell-overflow">{u.name}</td>
                                    <td>{u.ownerId}</td>
                                    <td className="cell-overflow">{u.filename}</td>
                                    <td>{new Date(u.uploadDate).toLocaleDateString('ru')}</td>
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


export default MusicList;