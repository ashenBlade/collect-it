import React, {FC} from 'react';

interface prop
{
    text:string;
}

const ConfirmWindow :FC<prop> =({text}) => {
    return (
        <div>
            <main className="d-flex justify-content-center align-items-center">
                <form className="p-3 w-50 bg-white shadow rounded mt-5">
                    <h3 className="mb-5 mt-lg-3 text-center">{text}</h3>
                    <form className="mb-3 mt-lg-5 w-100 d-inline-flex justify-content-around">
                        <form method="post">
                            <div className="w-100 mt-lg-4">
                                <button type="submit" className="btn btn-outline-success btn-lg">Да</button>
                            </div>
                        </form>
                        <form method="post">
                            <div className="w-100 mt-lg-4">
                                <button type="submit" className="btn btn-outline-danger btn-lg">Нет</button>
                            </div>
                        </form>
                    </form>
                </form>
            </main>
        </div>
    )
}
export default ConfirmWindow;