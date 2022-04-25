import React, {FC} from 'react';

interface EditInputParams {
    children?: string;
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
}

const EditInput: FC<EditInputParams> = ({children, onChange}) => {
    return (
        <input className='border rounded my-2 col-9 me-4' type='text' placeholder='Resource`s tags'
               value={children} onChange={onChange}/>
    );
};

export default EditInput;