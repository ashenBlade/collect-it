import React from 'react';

const DeleteButton = (props: any) => {
    return (
        <button className='btn btn-danger justify-content-center my-2' {...props}>
            Delete
        </button>
    );
};

export default DeleteButton;