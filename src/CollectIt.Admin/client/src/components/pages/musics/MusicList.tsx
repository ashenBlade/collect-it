import React, {useEffect, useState} from 'react';
import MusicsService from '../../../services/MusicsService'
import Music from "../../entities/music";
import Pagination from "../../UI/Pagination/Pagination";
import {useNavigate} from "react-router";
import SearchPanel from "../../UI/SearchPanel/SearchPanel";

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
                ? <>Loading...</>
                : <>
                    <SearchPanel onSearch={onSearch} placeholder={'Enter id of music'}/>
                    <div className='mt-5 mx-auto'>
                        <table className={'table table-borderless table-light'}>
                            <thead>
                            <th className='usersRow'>
                                <td className='Cell w-10'>ID</td>
                                <td className='Cell w-35'>Name</td>
                                <td className='Cell w-10'>OwnerID</td>
                                <td className='Cell'>Filename</td>
                                <td className='Cell'>Upload time</td>
                            </th>
                            </thead>
                            <tbody className='mx-auto mt-5 table-hover'>
                            {musics?.map(i =>
                                <tr onClick={() => toEditMusicPage(i.id)} className='usersRow'>
                                    <td className='Cell w-10'>{i.id}</td>
                                    <td className='Cell w-35'>
                                        <div className={'bigtext'}>{i.name}</div>
                                    </td>
                                    <td className='Cell w-10'>{i.ownerId}</td>
                                    <td className='Cell'>{i.filename}</td>
                                    <td className='Cell'>{new Date(i.uploadDate).toLocaleString('ru')}</td>
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