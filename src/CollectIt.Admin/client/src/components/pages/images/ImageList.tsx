import React, {useEffect, useState} from 'react';
import Image from "../../entities/image";
import Pagination from "../../UI/pagination/Pagination";
import ImagesService from "../../../services/ImagesService";

const ImageList = () => {
    let pageSize = 10;
    let pageNumber = 1;
    const [images, setImages] = useState<Image[]>([]);
    const [redirectID, SetID] = useState(0);
    useEffect(() => {
        ImagesService.getImagesPagedAsync({pageSize, pageNumber}).then(x => {
            setImages(x.images);
        })
    }, [])
    const [enteredText, setEnteredText] = useState("");
    const keyDownHandler = (event: React.KeyboardEvent<HTMLInputElement>) => {
        if (event.code === "Enter") {
            window.location.href=(`../images/${enteredText}`);
        }
    };
    function redirect()
    {
        window.location.href=(`../images/${redirectID}`)
    }
    const downloadPageNumber = (pageNumber: number) => {
        ImagesService.getImagesPagedAsync({pageSize, pageNumber}).then(x => {setImages(x.images)})
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
                {images?.map(i=>
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

export default ImageList;