import React from 'react';

const Comment = () => {
    return (
        <div className='border rounded my-2 col-9 me-4 p-0'>
            <div className='border-bottom row col-12 m-0 ps-2 pe-2'>
                <div className='col-6 text-start'>Author</div>
                <div className='col-6 text-end'>date</div>
            </div>
            <div className='col-12 m-0 ps-3 pe-3'>Comment</div>
        </div>
    );
};

export default Comment;