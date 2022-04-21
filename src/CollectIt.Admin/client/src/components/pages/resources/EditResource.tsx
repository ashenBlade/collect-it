import React from 'react';
import Image from "../../entities/image";
import DeleteButton from "../../UI/deleteButton/DeleteButton";
import InputBlock from "../../editBlockes/editInputBlock/InputBlock";
import CommentBlock from "../../editBlockes/editCommentBlock/CommentBlock";

const EditResource = () => {
    const img = new Image(1, 'New', new Date(), 'Dog',
        ["red", "green", "blue"], 'jpg', 1);
    return (
        <div className='align-items-center justify-content-center shadow border col-6 mt-4 m-auto d-block rounded'>
            <form className='col-12 p-3'>
                <p className='h2 text-center'>Edit Resource {img.id}</p>

                <InputBlock>{img.name}</InputBlock>
                <InputBlock>{img.tags.join(' ')}</InputBlock>

                <p className='h4 text-center'>Comments</p>

                <CommentBlock className='btn btn-danger justify-content-center my-2 hc-0 col-12 inner'></CommentBlock>

                <DeleteButton className='btn btn-danger justify-content-center my-2 hc-0 ms-4'></DeleteButton>
            </form>
        </div>
    );
};

export default EditResource;