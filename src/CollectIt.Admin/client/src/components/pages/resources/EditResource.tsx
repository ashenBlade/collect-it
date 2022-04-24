import React, {useEffect, useState} from 'react';
import Image from "../../entities/image";
import DeleteButton from "../../UI/deleteButton/DeleteButton";
import InputBlock from "../../editBlockes/editInputBlock/InputBlock";
import CommentBlock from "../../editBlockes/editCommentBlock/CommentBlock";
import {useParams} from "react-router";
import ImagesService from "../../../services/ImagesService";

const EditResource = () => {
    const [image, setImage] = useState(new Image(1, 'New', new Date(), 'Dog',
        ["red", "green", "blue"], 'jpg', 1));
    const x = useParams();
    const resourceId: number = Number(x.resourceId);
    useEffect(() => {
        ImagesService.getImageByIdAsync(resourceId).then(i => setImage(i))
    }, [])
    return (
        <div className='align-items-center justify-content-center shadow border col-6 mt-4 m-auto d-block rounded'>
            <form className='col-12 p-3'>
                <p className='h2 text-center'>Edit Resource {image.id}</p>

                <InputBlock>{image.name}</InputBlock>
                <InputBlock>{image.tags.join(' ')}</InputBlock>

                <p className='h4 text-center'>Comments</p>

                <CommentBlock className='btn btn-danger justify-content-center my-2 hc-0 col-12 inner'></CommentBlock>

                <DeleteButton className='btn btn-danger justify-content-center my-2 hc-0 ms-4'></DeleteButton>
            </form>
        </div>
    );
};

export default EditResource;