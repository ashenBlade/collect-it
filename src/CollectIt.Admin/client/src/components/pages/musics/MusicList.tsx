import React, {useEffect, useState} from 'react';
import MusicsService from '../../../services/MusicsService'
import Music from "../../entities/music";
import Pagination from "../../UI/pagination/Pagination";
import {useNavigate} from "react-router";
import SearchPanel from "../../UI/SearchPanel/SearchPanel";

const MusicList = () => {
    let pageSize = 10;
    let pageNumber = 1;
    const [musics, setMusics] = useState<Music[]>([]);
    useEffect(() => {
        MusicsService.getMusicsPagedAsync({pageSize,pageNumber}).then(x => {
            setMusics(x.musics);
        }).catch(x => alert(x.message));
    }, []);
    const downloadPageNumber = (pageNumber: number) => {
        MusicsService.getMusicsPagedAsync({pageSize, pageNumber}).then(x => {setMusics(x.musics)})
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
            <SearchPanel onSearch={onSearch} placeholder={'Enter id of music'}/>
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
                    {musics?.map(i=>
                        <tr onClick={() => toEditMusicPage(i.id)} className ='usersRow'>
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


export default MusicList;