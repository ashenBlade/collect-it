import React from 'react';
import '../../NavbarStyle.css'

const iterations = ['1','2','3','4','5','6','7'];
const Names = ['Мониторы с аниме','Птица замородок','Машина на дороге', 'Котенок на одеяле',
    'Стандартный американский дом','Осенний лес в природе','Дети за партами в школе'];
const Authors = ['Admin','TechSupport','User1','User2'];

const ResourcesList = () => {
    return (
        <div>
            <div className='w-75 mt-5 mx-auto'>
                <input id='email' className='form-control my-2' type='text' placeholder='Enter resource`s name'/>
                <input id='email' className='form-control my-2' type='text' placeholder='Enter id'/>
                <tbody className='usersTable mx-auto mt-5'>
                <tr className="usersRow firstRow">
                    <td className='idCell color-purple'>ID</td>
                    <td className='usersCell color-purple'>Name</td>
                    <td className='usersCell color-purple'>Type</td>
                    <td className='usersCell color-purple'>Author</td>
                    <td className='usersCell color-purple'>Upload time</td>
                </tr>
                {iterations.map(i => (
                    <tr className="usersRow">
                        <td className='idCell'>{i}</td>
                        <td className='usersCell'>{Names[+i-1]}</td>
                        <td className='usersCell'>Image</td>
                        <td className='usersCell'>{Authors[+i%4]}</td>
                        <td className='usersCell'>27-03-2022</td>
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

export default ResourcesList;