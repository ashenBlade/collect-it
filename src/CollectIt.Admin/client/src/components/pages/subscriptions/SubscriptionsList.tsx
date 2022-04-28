import React from 'react';
import '../../UI/NavBar/NavbarStyle.css'

const iterations = ['1','2','3','4'];
const Names = ['Бронзовая','Серебряная','Обычная','Кардбланш']
const Descriptions = ['Обычная подписка','Подписка для любителей качать','Не для пиратов','Скачивай, что хочешь']
const Duration = ['1','1','1','0']
const Downloads = ['50','100','200','0']
const Price = ['200','350','500','0']

const SubscriptionsList = () => {
    return (
            <div className={'container mt-5'}>
                <input id='email' className='form-control my-2' type='text' placeholder='Enter subscription`s name'/>
                <input id='email' className='form-control my-2' type='text' placeholder='Enter id'/>
                <div className='mt-5 mx-auto'>
                    <table className={'table table-borderless table-light'}>
                        <thead className='mx-auto mt-5 table-hover'>
                        <th className='firstRow usersRow'>
                            <td className ='Cell idCell color-purple '>ID</td>
                            <td className= 'Cell nameCell color-purple'>Name</td>
                            <td className= 'Cell idCell color-purple'>OwnerID</td>
                            <td className= 'Cell color-purple'>Filename</td>
                            <td className= 'Cell color-purple'>Upload time</td>
                        </th>
                        </thead>
                        <tbody className='mx-auto mt-5 table-hover'>
                        {iterations.map(i => (
                            <tr className="usersRow">
                                <td className='idCell Cell'>{i}</td>
                                <td className='nameCell Cell'>{Names[+i-1]}</td>
                                <td className='Cell'>{Descriptions[+i-1]}</td>
                                <td className='Cell'>{Duration[+i-1]}</td>
                                <td className='Cell'>{Downloads[+i-1]}</td>
                                <td className='Cell'>{Price[+i-1]}</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                <ul className="pagination">
                    <li className="page-item">
                        <button className="page-link" type="button">1</button>
                    </li>
                    <li className="page-item">
                        <button className="page-link" type="button">2</button>
                    </li>
                    <li className="page-item">
                        <button className="page-link" type="button">3</button>
                    </li>
                    <li>
                        <h2 className='text-center mx-1 mb-0'>...</h2>
                    </li>
                    <li className="page-item">
                        <button className="page-link" type="button">9</button>
                    </li>
                </ul>
            </div>
        </div>
    );
};

export default SubscriptionsList;