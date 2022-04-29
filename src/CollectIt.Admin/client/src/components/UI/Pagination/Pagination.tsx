import React, {FC, useState} from 'react';
import './Pagination.tsx.css'

export interface PaginationInterface {
    totalPagesCount: number;
    onPageChange?: (redirectPageNumber: number) => void;
    initialPage: number;
    maxVisibleButtonsCount?: number;
}

const Pagination: FC<PaginationInterface> = ({initialPage: i,
                                                 totalPagesCount,
                                                 onPageChange: opc,
                                                 maxVisibleButtonsCount: mvbc}) => {
    const [page, setPage] = useState(i ?? 1);
    const maxPagesCount = totalPagesCount;
    const maxVisibleButtonsCount = mvbc === undefined ? 5 : mvbc;
    const onPageChangeInternal = opc === undefined
        ? (_: number) => { }
        : (pageNumber: number) => {
            if (pageNumber === page) return;
            setPage(pageNumber);
            opc(pageNumber);
        }

    const getPageButton = ({value, pageNumber}: {value?: string | number, pageNumber: number}) => (
        <li className={'page-item cursor-pointer'} onClick={e => {
            e.preventDefault();
            onPageChangeInternal(pageNumber);
        }}>
            <a className={'page-link'}>
                <span aria-hidden={true}>{value === undefined ? pageNumber : value}</span>
            </a>
        </li>
    )

    const getDotsButton = () => (
        <li className={'page-number cursor-pointer'}>
            <a className={'page-link'}>
                <span aria-hidden={true}>...</span>
            </a>
        </li>
    )

    const getLeftBorderButtons = () => (
        <>
            {getPageButton({pageNumber: 1})}
            {getPageButton({pageNumber: 2})}
            {getDotsButton()}
        </>
    )

    const getRightBorderButtons = () => (
        <>
            {getDotsButton()}
            {getPageButton({pageNumber: maxPagesCount - 1})}
            {getPageButton({pageNumber: maxPagesCount})}
        </>
    )

    const getMiddleButtons = (p: number) => (
        <>
            {getDotsButton()}
            {getPageButton({pageNumber: p - 1})}
            {getPageButton({pageNumber: p})}
            {getPageButton({pageNumber: p + 1})}
            {getDotsButton()}
        </>
    )

    return (
        <ul className={'pagination'}>
            <li className={'page-item cursor-pointer'} onClick={e => {
                e.preventDefault()
                if (page !== 1) onPageChangeInternal(page - 1);
            }}>
                <a className={'page-link'}>
                    <span aria-hidden={true}>&laquo;</span>
                </a>
            </li>
            {maxPagesCount <= maxVisibleButtonsCount
                ? (new Array(maxPagesCount).fill(null).map((_, i) => getPageButton({pageNumber: i + 1})))
                : page <= 2
                    ? getLeftBorderButtons()
                    : page >= maxPagesCount - 1
                        ? getRightBorderButtons()
                        : getMiddleButtons(page)}

            <li className={'page-item cursor-pointer'} onClick={e => {
                e.preventDefault();
                if (page < maxPagesCount) onPageChangeInternal(page + 1);
            }}>
                <a className={'page-link'}>
                    <span aria-hidden={true}>&raquo;</span>
                </a>
            </li>
        </ul>
    );
};

export default Pagination;