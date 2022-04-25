import React, {FC} from 'react';

export interface DeleteButtonInterface {
    onDeleteClick: () => void;
}

const DeleteButton: FC<DeleteButtonInterface> = ({onDeleteClick}) => {
    return (
        <button type='button' className='btn btn-danger my-2' onClick={e => {
            e.preventDefault();
            onDeleteClick();
        }}>
            Delete
        </button>
    );
};

export default DeleteButton;