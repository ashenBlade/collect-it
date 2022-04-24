import React, {useState} from 'react';

const SaveButton = (props:any) => {
    return (
        <button className='btn btn-primary justify-content-center my-2 col-2' {...props}>
            Save
        </button>
    );
};

export default SaveButton;