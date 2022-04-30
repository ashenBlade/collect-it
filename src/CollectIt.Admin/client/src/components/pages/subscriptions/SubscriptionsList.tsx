import React, {useEffect, useState} from 'react';
import {useNavigate} from "react-router";
import subscription from "../../entities/subscription";
import SearchPanel from "../../UI/SearchPanel/SearchPanel";
import Pagination from "../../UI/Pagination/Pagination";
import SubscriptionsService from "../../../services/SubscriptionsService";

const SubscriptionsList = () => {
    let pageSize = 10;
    let pageNumber = 1;
    const [subs, setSubs] = useState<subscription[]>([]);
    useEffect(() => {
        SubscriptionsService.getSubscriptionsPagedAsync({pageSize, pageNumber}).then(x => {
            setSubs(x.subscriptions);
        })
    }, [])
    const downloadPageNumber = (pageNumber: number) => {
        SubscriptionsService.getSubscriptionsPagedAsync({pageSize, pageNumber}).then(x => {setSubs(x.subscriptions);})
    }
    const nav = useNavigate();
    const toEditSubscriptionPage = (id: number) => nav(`/subscription/${id}`);
    const onSearch = (q: string) => {
        const id = Number(q);
        if (!Number.isInteger(id)) {
            alert('Id must be an integer');
            return;
        }
        toEditSubscriptionPage(id);
    }
    return (
        <div className={'container mt-5'}>
            <SearchPanel onSearch={onSearch} placeholder={'Enter subscription id'}/>
            <div className='mt-5 mx-auto'>
                <table className={'usersTable table table-borderless table-light'}>
                    <thead>
                    <th className='firstRow usersRow'>
                        <td className='Cell idCell color-purple'>ID</td>
                        <td className='Cell nameCell color-purple'>Name</td>
                        <td className='Cell idCell color-purple'>Description</td>
                        <td className='Cell color-purple'>Duration</td>
                        <td className='Cell color-purple'>Price</td>
                    </th>
                    </thead>
                    <tbody className='mx-auto mt-5 table-hover'>
                    {subs?.map(i =>
                        <tr onClick={() => toEditSubscriptionPage(i.id)} className='usersRow'>
                            <td className='Cell idCell'>{i.id}</td>
                            <td className='Cell nameCell'>{i.name}</td>
                            <td className='Cell idCell'><div className={'bigtext'}></div>{i.description}</td>
                            <td className='Cell'>{i.monthDuration}</td>
                            <td className='Cell'>{i.price}</td>
                        </tr>
                    )}
                    </tbody>
                </table>
                <Pagination totalPagesCount={10} onPageChange={downloadPageNumber}/>
            </div>
        </div>
    );
};

export default SubscriptionsList;