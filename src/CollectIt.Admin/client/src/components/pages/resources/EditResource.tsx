import React from 'react';
import DeleteButton from "../../UI/deleteButton/DeleteButton";
import Comment from "../../UI/comment/Comment";
import InputBlock from "../../editBlockes/editInputBlock/InputBlock";

const EditResource = () => {
    return (
        <div className='align-items-center justify-content-center shadow border col-6 mt-4 m-auto d-block rounded'>
            <form className='col-12 p-3'>
                <p className='h2 text-center'>Edit Resource 0</p>

                <InputBlock></InputBlock>
                <InputBlock></InputBlock>

                <div className='row m-0 ms-4'>
                    <Comment></Comment>
                    <div className='col-2 p-0'>
                        <span className='before'></span>
                        <button className='btn btn-danger justify-content-center my-2 hc-0 col-12 inner'>
                            Delete
                        </button>
                    </div>
                </div>

                <div className='justify-content-start d-flex ms-4'>
                    <DeleteButton></DeleteButton>
                </div>
            </form>
        </div>
    );
};

export default EditResource;