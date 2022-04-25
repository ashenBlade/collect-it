import React, {FC} from 'react';

interface EditInputParams {
    children?: string;
}

const EditInput: FC<EditInputParams> = ({children}) => {
    return (
        <input className='border rounded my-2 col-9 me-4' type='text' placeholder='Resource`s tags'
               value={children}/>
    );
};

export default EditInput;