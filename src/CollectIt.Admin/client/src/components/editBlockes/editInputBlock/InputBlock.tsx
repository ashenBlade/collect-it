import React from 'react';
import EditInput from "../../UI/input/EditInput";
import SaveButton from "../../UI/saveButton/SaveButton";

const InputBlock = () => {
    return (
        <div className='row m-0 ms-4'>
            <EditInput></EditInput>
            <SaveButton></SaveButton>
        </div>
    );
};

export default InputBlock;