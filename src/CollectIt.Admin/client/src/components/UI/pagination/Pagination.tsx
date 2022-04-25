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
            <li className={'page-item cursor-pointer'} onClick={e => {
                e.preventDefault()
                if (page !== 1) onPage(page - 1);
            }}>
                <a className={'page-link'}>
                    <span aria-hidden={true}>&laquo;</span>
                </a>
            </li>
            <li className={'page-item cursor-pointer'} onClick={e => {
                e.preventDefault();
                if (page < totalPagesCount) onPage(page + 1);
            }}>
                <a className={'page-link'}>
                    <span aria-hidden={true}>&raquo;</span>
                </a>
            </li>
        </ul>
    );
};

export default Pagination;