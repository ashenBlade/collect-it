import React from 'react';
import '../../NavbarStyle.css'

const iterations = ['1','2','3','4'];
const Names = ['Бронзовая','Серебряная','Обычная','Кардбланш']
const Descriptions = ['Обычная подписка','Подписка для любителей качать','Не для пиратов','Скачивай, что хочешь']
const Duration = ['1','1','1','0']
const Downloads = ['50','100','200','0']
const Price = ['200','350','500','0']

const SubscriptionsList = () => {
    return (
        <div>
            <div className='w-75 mt-5 mx-auto'>
                <input id='email' className='form-control my-2' type='text' placeholder='Enter subscription`s name'/>
                <input id='email' className='form-control my-2' type='text' placeholder='Enter id'/>
                <tbody className='usersTable mx-auto mt-5'>
                <tr className="usersRow firstRow">
                    <td className='idCell color-purple'>ID</td>
                    <td className='usersCell color-purple'>Name</td>
                    <td className='usersCell color-purple'>Description</td>
                    <td className='usersCell color-purple'>Duration</td>
                    <td className='usersCell color-purple'>Downloads</td>
                    <td className='usersCell color-purple'>Price</td>
                </tr>
                {iterations.map(i => (
                    <tr className="usersRow">
                        <td className='idCell'>{i}</td>
                        <td className='usersCell'>{Names[+i-1]}</td>
                        <td className='usersCell'>{Descriptions[+i-1]}</td>
                        <td className='usersCell'>{Duration[+i-1]}</td>
                        <td className='usersCell'>{Downloads[+i-1]}</td>
                        <td className='usersCell'>{Price[+i-1]}</td>
                    </tr>
                ))}
                </tbody>
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