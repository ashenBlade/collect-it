import React, {useEffect, useState} from 'react';
import MusicsService from '../../../services/MusicsService'
import Music from "../../entities/music";
import Pagination from "../../UI/pagination/Pagination";

const MusicList = () => {
    let pageSize = 10;
    let pageNumber = 1;
    const [musics, setMusics] = useState<Music[]>([]);
    useEffect(() => {
        MusicsService.getMusicsPagedAsync({pageSize,pageNumber}).then(x => {
            console.log(x.musics);
            console.log(x.totalCount);
            setMusics(x.musics);
        }).catch(x => alert(x.message));
    }, []);
    const downloadPageNumber = (pageNumber: number) => {
        MusicsService.getMusicsPagedAsync({pageSize, pageNumber}).then(x => {setMusics(x.musics)})
    }
    return (
        <div>
            <div className='w-75 mt-5 mx-auto'>
                <tbody className='usersTable mx-auto mt-5'>
                <tr className='firstRow usersRow'>
                    <td className ='idCell color-purple '>ID</td>
                    <td className= 'usersCell color-purple'>Name</td>
                    <td className= 'usersCell color-purple'>OwnerID</td>
                    <td className= 'usersCell color-purple'>FileName</td>
                    <td className= 'usersCell color-purple'>UploadTime</td>
                </tr>
                {musics?.map(i=>
                        <tr className ='usersRow'>
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


export default MusicList;