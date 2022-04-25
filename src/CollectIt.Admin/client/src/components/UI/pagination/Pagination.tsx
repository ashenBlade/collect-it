import React, {useState} from 'react';
import './Pagination.tsx.css'

const Pagination = ({currentPage, totalPagesCount, onPageChange}: {currentPage: number, totalPagesCount: number, onPageChange: ((redirectPageNumber: number)=>(void))}) => {
    const [page, setPage] = useState(currentPage);
    const onPage = (pageNumber: number) => {
        if (pageNumber === page) return;
        setPage(pageNumber);
        onPageChange(pageNumber);
    }
    return (
        <ul className={'pagination'}>
            <li className={'page-item cursor-pointer'} onClick={() => page !== 1
                ? onPage(page - 1)
                : false}>
                <a className={'page-link'}>
                    <span aria-hidden={true}>&laquo;</span>
                </a>
            </li>
            <li className={'page-item cursor-pointer'}>
                <a className={'page-link'}>
                    1
                </a>
            </li>
            {
                totalPagesCount < 6
                    ? Array(totalPagesCount - 2).fill(1).map((_, i) => (
                        <li className={'page-item cursor-pointer'} onClick={() => onPage(i + 2)}>
                            <a className={'page-link'}>
                                {i + 2}
                            </a>
                        </li>
                    ))
                    : page < 4
                ? <>
                            <li className={'page-item cursor-pointer'}>
                                <a className={'page-link'}>
                                    2
                                </a>
                            </li>
                            <li className={'page-item cursor-pointer'}>
                                <a className={'page-link'}>
                                    3
                                </a>
                            </li>
                        </>
                    : <></>
            }
            <li className={'page-item cursor-pointer'}>
                <a className={'page-link'}>
                    {totalPagesCount}
                </a>
            </li>
            <li className={'page-item cursor-pointer'} onClick={() => page < totalPagesCount
                ? onPage(page + 1)
                : false}>
                <a className={'page-link'}>
                    <span aria-hidden={true}>&raquo;</span>
                </a>
            </li>
        </ul>
    );
};

export default Pagination;