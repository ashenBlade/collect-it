import React from 'react';

const EditInput = (props: any) => {
    return (
        <input className='border rounded my-2 col-9 me-4' type='text' placeholder='Resource`s tags' value={props.children}/>
    );
};

export default EditInput;