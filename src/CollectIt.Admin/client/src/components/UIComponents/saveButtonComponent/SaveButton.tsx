import React, {FC} from 'react';

interface SaveButtonProps {
    onSave: () => void;
}

const SaveButton: FC<SaveButtonProps> = ({onSave}) => {
    return (
        <button className='btn btn-primary justify-content-center my-2 col-2' onClick={e => {
            e.preventDefault();
            onSave();
        }}>
            Save
        </button>
    );
};

export default SaveButton;