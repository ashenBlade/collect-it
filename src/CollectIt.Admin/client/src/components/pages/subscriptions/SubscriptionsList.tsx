import React, {useEffect, useState} from 'react';
import {useNavigate} from "react-router";
import subscription from "../../entities/subscription";
import SearchPanel from "../../UI/SearchPanel/SearchPanel";
import Pagination from "../../UI/Pagination/Pagination";
import SubscriptionsService from "../../../services/SubscriptionsService";
import {ResourceType} from "../../entities/resource-type";

const SubscriptionsList = () => {
    let pageSize = 10;

    const [subs, setSubs] = useState<subscription[]>([]);
    const [maxPages, setMaxPages] = useState(0);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        SubscriptionsService.getSubscriptionsPagedAsync({pageSize, pageNumber:1}).then(x => {
            setSubs(x.subscriptions);
            setMaxPages(Math.ceil(x.totalCount / pageSize));
            setLoading(false);
        }).catch(_ => setLoading(false))
    }, [])

    const downloadPageNumber = (pageNumber: number) => {
        setLoading(true);
        SubscriptionsService.getSubscriptionsPagedAsync({pageSize, pageNumber}).then(x => {
            setSubs(x.subscriptions);
            setLoading(false);
        }).catch(_ => setLoading(false))
    }

    const nav = useNavigate();
    const toEditSubscriptionPage = (id: number) => nav(`/subscriptions/${id}`);

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
            {loading
                ? <>Loading...</>
                : <>
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
                    </div>
                    <footer className={'footer fixed-bottom d-flex mb-0 justify-content-center'}>
                        <Pagination totalPagesCount={maxPages} onPageChange={downloadPageNumber}/>
                    </footer>
                </>
            }
        </div>
    );
};

export default SubscriptionsList;