import React, {FC, useState} from 'react';
import EditInput from "../../UIComponents/inputComponent/EditInput";
import SaveButton from "../../UIComponents/saveButtonComponent/SaveButton";

interface InputBlockParams {
    id?: number;
    label?: string;
    children?: string;
    onClickSaveButton: (e: React.ChangeEvent) => void;
    change: (val?: string) => void;
}

const InputBlock: FC<InputBlockParams> = (
    {
        id,
        label,
        children,
        onClickSaveButton,
        change
    }) =>
{
    const [value,setValue] = useState(children)
    
    const changeHandler = (e: React.ChangeEvent<HTMLInputElement>) => {
      setValue(e.target.value);
      change(value)
    }

    return (
        <div className='row m-0 ms-4'>
            <label>{label}</label>
            <EditInput onChange={changeHandler}>{value}</EditInput>
            <SaveButton onClick={onClickSaveButton}></SaveButton>
        </div>
    );
};

export default InputBlock;