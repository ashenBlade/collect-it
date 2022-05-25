import React, { FC, useState } from 'react';
import './Pagination.tsx.css'

export interface PaginationInterface {
    totalPagesCount: number;
    onPageChange: (redirectPageNumber: number) => void;
    initialPage?: number;
    maxVisibleButtonsCount?: number;
}

const Pagination: FC<PaginationInterface> = ({totalPagesCount,
                                                 onPageChange: onPageChangeUser}) => {
    const maxPagesCount = totalPagesCount;
    const [currentPage, setCurrentPage] = useState(1);

    const onPageButtonClick = (pageNumberToSet: number) => {
        if (pageNumberToSet === currentPage) {
            console.log(`Returning with to set = ${pageNumberToSet}, current = ${currentPage}`);
            return;
        }
        console.log(`Old current page ${currentPage}`);
        setCurrentPage(pageNumberToSet);
        console.log(`New current page ${currentPage}`);
        onPageChangeUser(pageNumberToSet);
    }

    const getPageButton = (pageNumber: number) => (
        <li className={'page-item cursor-pointer'} onClick={e => {
            e.preventDefault();
            onPageButtonClick(pageNumber);
        }}>
            <a className={'page-link'}>
                <span aria-hidden={true}>{pageNumber}</span>
            </a>
        </li>
    )

    return (
        <>
            <ul className={'pagination'}>
                <li className={'page-item cursor-pointer'} onClick={e => {
                    e.preventDefault();
                    if (currentPage > 1) onPageButtonClick(currentPage - 1);
                }}>
                    <a className={'page-link'}>
                        <span aria-hidden={true}>&laquo;</span>
                    </a>
                </li>

                {[...Array(maxPagesCount)]
                    .map((_, i) => getPageButton(i + 1))}

                <li className={'page-item cursor-pointer'} onClick={e => {
                    e.preventDefault();
                    if (currentPage < maxPagesCount)
                        onPageButtonClick(currentPage + 1);
                }}>
                    <a className={'page-link'}>
                        <span aria-hidden={true}>&raquo;</span>
                    </a>
                </li>
            </ul>
        </>
    );
};

export default Pagination;