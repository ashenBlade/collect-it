import React, {FC, useState} from 'react';
import SaveButton from "../../UIComponents/saveButtonComponent/SaveButton";

interface InputBlockParams {
    id?: number;
    placeholder?: string;
    initial?: string;
    onSave: (e: string) => void;
    fieldName: string;
}

const InputBlock: FC<InputBlockParams> = (
    {
        id,
        placeholder,
        initial,
        onSave,
        fieldName,
    }) =>
{
    const [value, setValue] = useState(initial)
    return (
        <div className='row m-0 ms-4'>
            <label>{fieldName}: </label>
            <input className='border rounded my-2 col-9 me-4'
                   type='text'
                   placeholder={placeholder}
                   defaultValue={initial}
                   onChange={e => {
                setValue(e.currentTarget.value)
            }}/>
            <SaveButton onSave={() => {
                if (!value) return;
                onSave(value);
            }}/>
        </div>
    );
};

export default InputBlock;