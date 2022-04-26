import React, {FC} from 'react';

interface ConfirmationWindowInterface
{
    text: string;
    onSubmit: () => void;
    onDecline: () => void | undefined;
}

const ConfirmationWindow :FC<ConfirmationWindowInterface> =({text, onDecline, onSubmit}) => {
    const submit = (e: React.MouseEvent<HTMLButtonElement>) => {
        e.preventDefault();
        onSubmit();
    }
    const decline = (e: React.MouseEvent<HTMLButtonElement>) => {
        e.preventDefault();
        onDecline();
    }
    return (
        <div>
            <div className="d-flex justify-content-center align-items-center">
                <div className="p-3 w-50 bg-white shadow rounded mt-5">
                    <h3 className="mb-5 mt-lg-3 text-center">{text}</h3>
                    <div className="mb-3 mt-lg-5 w-100 d-inline-flex justify-content-around">
                        <div className="w-100 mt-lg-4">
                            <button type="button" className="btn btn-outline-success btn-lg" onClick={submit}>Yes</button>
                        </div>
                        <div className="w-100 mt-lg-4">
                            <button type="button" className="btn btn-outline-danger btn-lg" onClick={decline}>No</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default ConfirmationWindow;