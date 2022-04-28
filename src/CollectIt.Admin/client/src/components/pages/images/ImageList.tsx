import React, {useEffect, useState} from 'react';
import Image from "../../entities/image";
import Pagination from "../../UI/pagination/Pagination";
import ImagesService from "../../../services/ImagesService";
import {useNavigate} from "react-router";
import SearchPanel from "../../UI/SearchPanel/SearchPanel";


const ImageList = () => {
    let pageSize = 10;
    let pageNumber = 1;
    const [images, setImages] = useState<Image[]>([]);
    useEffect(() => {
        ImagesService.getImagesPagedAsync({pageSize, pageNumber}).then(x => {
            setImages(x.images);
        })
    }, [])
    const downloadPageNumber = (pageNumber: number) => {
        ImagesService.getImagesPagedAsync({pageSize, pageNumber}).then(x => {setImages(x.images)})
    }
    const nav = useNavigate();
    const toEditImagePage = (id: number) => nav(`/images/${id}`);
    const onSearch = (q: string) => {
        const id = Number(q);
        if (!Number.isInteger(id)) {
            alert('Id must be an integer');
            return;
        }
        toEditImagePage(id);
    }
    return (
        <div className={'container mt-5'}>
            <SearchPanel onSearch={onSearch} placeholder={'Enter id of image'}/>
            <div className='mt-5 mx-auto'>
                <table className={'usersTable table table-borderless table-light'}>
                    <thead>
                    <th className='firstRow usersRow'>
                        <td className='Cell idCell color-purple'>ID</td>
                        <td className='Cell nameCell color-purple'>Name</td>
                        <td className='Cell idCell color-purple'>OwnerID</td>
                        <td className='Cell color-purple'>Filename</td>
                        <td className='Cell color-purple'>Upload time</td>
                    </th>
                    </thead>
                    <tbody className='mx-auto mt-5 table-hover'>
                    {images?.map(i =>
                        <tr onClick={() => toEditImagePage(i.id)} className='usersRow'>
                            <td className='Cell idCell'>{i.id}</td>
                            <td className='Cell nameCell'><div className={'bigtext'}> {i.name}</div></td>
                            <td className='Cell idCell'>{i.ownerId}</td>
                            <td className='Cell'>{i.filename}</td>
                            <td className='Cell'>{new Date(i.uploadDate).toLocaleString('ru')}</td>
                        </tr>
                    )}
                    </tbody>
                </table>
                <Pagination currentPage={1} totalPagesCount={10} onPageChange={downloadPageNumber}/>
            </div>
        </div>
    );
};

export default ImageList;