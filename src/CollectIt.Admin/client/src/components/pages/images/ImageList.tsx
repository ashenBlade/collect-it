import React, {useEffect, useState} from 'react';
import Image from "../../entities/image";
import Pagination from "../../UI/Pagination/Pagination";
import ImagesService from "../../../services/ImagesService";
import {useNavigate} from "react-router";
import SearchPanel from "../../UI/SearchPanel/SearchPanel";
import ReactLoading from 'react-loading'

const ImageList = () => {
    const pageSize = 10;

    const [images, setImages] = useState<Image[]>([]);
    const [maxPages, setMaxPages] = useState(0);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        ImagesService.getImagesPagedAsync({pageSize, pageNumber: 1}).then(x => {
            setImages(x.images);
            setMaxPages(Math.ceil(x.totalCount / pageSize));
            setLoading(false);
        }).catch(_ => setLoading(false))
    }, [])

    const downloadPageNumber = (pageNumber: number) => {
        setLoading(true);
        ImagesService.getImagesPagedAsync({pageSize, pageNumber}).then(x => {
            setImages(x.images);
            setLoading(false);
        }).catch(_ => setLoading(false))
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
            {loading
                ? <><ReactLoading className={'mx-auto'} type={'spinningBubbles'} color={'black'} height='200px' width='200px' /></>
                : <>
                    <SearchPanel onSearch={onSearch} placeholder={'Enter id of image'}/>
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
                            {images?.map(u =>
                                <tr onClick={() => toEditImagePage(u.id)} style={{cursor: "pointer"}}>
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

// @ts-ignore
export default ImageList;