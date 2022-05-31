import React, { useEffect, useState } from 'react';
import { useNavigate } from "react-router";
import subscription from "../../entities/subscription";
import SearchPanel from "../../UI/SearchPanel/SearchPanel";
import Pagination from "../../UI/Pagination/Pagination";
import SubscriptionsService from "../../../services/SubscriptionsService";
import { ResourceType } from "../../entities/resource-type";
import { Link } from "react-router-dom";
import ReactLoading from "react-loading";

const SubscriptionsList = () => {
    let pageSize = 5;

    const [subs, setSubs] = useState<subscription[]>([]);
    const [maxPages, setMaxPages] = useState(0);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        SubscriptionsService.getSubscriptionsPagedAsync(1, pageSize).then(x => {
            setSubs(x.subscriptions.sort((a, b) => a.id > b.id ? 1 : a.id == b.id ? 0: -1));
            setMaxPages(Math.ceil(x.totalCount / pageSize));
            setLoading(false);
        }).catch(_ => setLoading(false))
    }, [])

    const downloadPageNumber = (pageNumber: number) => {
        setLoading(true);
        SubscriptionsService.getSubscriptionsByResourceTypePagedAsync({pageNumber, pageSize, type: ResourceType.Image})
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
                ? <><ReactLoading className={'mx-auto'} type={'spinningBubbles'} color={'black'} height='200px' width='200px' /></>
                : <>
                    <div className='ms-2 mb-3'><Link to='/subscriptions/create'>
                        <button className='btn btn-primary'>Create subscription</button>
                    </Link></div>
                    <SearchPanel onSearch={onSearch} placeholder={'Enter subscription id'}/>
                    <div className='mt-5 mx-auto'>
                        <table className='table table-hover table-striped table-w-100'>
                            <thead>
                            <tr>
                                <td>ID</td>
                                <td>Name</td>
                                <td>Description</td>
                                <td>Duration</td>
                                <td>Price</td>
                                <td>Type</td>
                                <td>Active</td>
                            </tr>
                            </thead>
                            <tbody>
                            {subs?.map(u =>
                                <tr onClick={() => toEditSubscriptionPage(u.id)} style={{cursor: "pointer"}}>
                                    <td>{u.id}</td>
                                    <td className="cell-overflow">{u.name}</td>
                                    <td className="cell-overflow">{u.description}</td>
                                    <td>{u.monthDuration}</td>
                                    <td>{u.price}</td>
                                    <td>{u.appliedResourceType}</td>
                                    <td>{u.active ? <>Active</> : <>Disabled</>}</td>
                                </tr>
                            )}
                            </tbody>
                        </table>
                    </div>
                </>
            }
            <footer className={'footer fixed-bottom d-flex mb-0 justify-content-center'}>
                <Pagination totalPagesCount={maxPages} onPageChange={downloadPageNumber}/>
            </footer>
        </div>
    );
};

export default SubscriptionsList;