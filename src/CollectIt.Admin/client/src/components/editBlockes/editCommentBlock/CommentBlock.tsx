import React from 'react';
import Comment from "../../UI/comment/Comment";
import DeleteButton from "../../UI/deleteButton/DeleteButton";

const CommentBlock = (props: any) => {
    return (
        <div className='row m-0 ms-4'>
            <Comment></Comment>
            <div className='col-2 p-0'>
                <span className='before'></span>
                <DeleteButton {...props}></DeleteButton>
            </div>
        </div>
    );
};

export default CommentBlock;