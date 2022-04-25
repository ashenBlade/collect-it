import React, {useEffect, useState} from 'react';
import MusicsService from '../../../services/MusicsService'
import Music from "../../entities/music";

const MusicList = () => {
    let pageSize = 10;
    let pageNumber = 1;
    const [music, setMusic] = useState<Music[]>([]);
    useEffect(() => {
        MusicsService.getMusicsPagedAsync({pageSize,pageNumber}).then(x => {
            setMusic(x.musics);
        });
    }, []);
    return (
        <div>
            <div className='w-75 mt-5 mx-auto'>
                <input id='email' className='form-control my-2' type='text' placeholder='Enter login/e-mail'/>
                <input id='email' className='form-control my-2' type='text' placeholder='Enter id'/>
                <tbody className='usersTable mx-auto mt-5'>
                <tr className='firstRow usersRow'>
                    <td className ='idCell color-purple '>ID</td>
                    <td className= 'usersCell color-purple'>Name</td>
                    <td className= 'usersCell color-purple'>OwnerID</td>
                    <td className= 'usersCell color-purple'>FileName</td>
                    <td className= 'usersCell color-purple'>UploadTime</td>
                </tr>
                {music?.map(i=>
                        <tr className ='usersRow'>
                            <td className ='idCell'>{i.id}</td>
                            <td className ='usersCell'>{i.name}</td>
                            <td className ='usersCell'>{i.ownerId}</td>
                            <td className ='usersCell'>{i.filename}</td>
                            <td className ='usersCell'>{i.uploadDate}</td>
                        </tr>
                )}
                </tbody>
                <ul className="pagination">
                    <li className="page-item">
                        <button className="page-link" type="button">1</button>
                    </li>
                    <li className="page-item">
                        <button className="page-link" type="button">2</button>
                    </li>
                    <li className="page-item">
                        <button className="page-link" type="button">3</button>
                    </li>
                    <li>
                        <h2 className='text-center mx-1 mb-0'>...</h2>
                    </li>
                    <li className="page-item">
                        <button className="page-link" type="button">9</button>
                    </li>
                </ul>
            </div>
        </div>
    );
};


export default MusicList;