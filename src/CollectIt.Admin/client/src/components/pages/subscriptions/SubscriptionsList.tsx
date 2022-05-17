import React, { useEffect, useState } from 'react';
import { useNavigate } from "react-router";
import subscription from "../../entities/subscription";
import SearchPanel from "../../UI/SearchPanel/SearchPanel";
import Pagination from "../../UI/Pagination/Pagination";
import SubscriptionsService from "../../../services/SubscriptionsService";
import { ResourceType } from "../../entities/resource-type";
import { Link } from "react-router-dom";

const SubscriptionsList = () => {
    let pageSize = 10;

    const [subs, setSubs] = useState<subscription[]>([]);
    const [maxPages, setMaxPages] = useState(0);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        SubscriptionsService.getSubscriptionsPagedAsync({pageSize, pageNumber: 1, type: ResourceType.Image}).then(x => {
            setSubs(x.subscriptions);
            setMaxPages(Math.ceil(x.totalCount / pageSize));
            setLoading(false);
        }).catch(_ => setLoading(false))
    }, [])

    const downloadPageNumber = (pageNumber: number) => {
        setLoading(true);
        SubscriptionsService.getSubscriptionsPagedAsync({pageNumber, pageSize, type: ResourceType.Image})
            .then(x => {
                setSubs(x.subscriptions.sort((a, b) => a.id > b.id ? 1 : a.id == b.id ? 0: -1));
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
                    <div className='ms-2 mb-3'><Link to='/subscriptions/create'>
                        <button className='btn btn-primary'>Create subscription</button>
                    </Link></div>
                    <SearchPanel onSearch={onSearch} placeholder={'Enter subscription id'}/>
                    <div className='mt-5 mx-auto'>
                        <table className={'usersTable table table-borderless table-light'}>
                            <thead>
                            <th className='firstRow usersRow'>
                                <td className='Cell idCell'>ID</td>
                                <td className='Cell'>Name</td>
                                <td className='Cell nameCell'>Description</td>
                                <td className='Cell idCell'>Duration</td>
                                <td className='Cell'>Price</td>
                            </th>
                            </thead>
                            <tbody className='mx-auto mt-5 table-hover'>
                            {subs
                            .sort((a, b) => a.id > b.id ? 1 : a.id ==b.id ? 0: -1)
                            .map(i =>
                                <tr onClick={() => toEditSubscriptionPage(i.id)} className='usersRow'>
                                    <td className='Cell idCell'>{i.id}</td>
                                    <td className='Cell'>{i.name}</td>
                                    <td className='Cell nameCell'><div className={'bigtext'}></div>{i.description}</td>
                                    <td className='Cell idCell'>{i.monthDuration}</td>
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