import React, {FC, useState} from 'react';
import 'bootstrap/dist/css/bootstrap.css';

export interface SearchPanelInterface {
    onSearch: (query: string) => void;
    placeholder?: string;
}

const SearchPanel: FC<SearchPanelInterface> = ({onSearch, placeholder}) => {
    const [query, setQuery] = useState('');
    return (
        <div className='d-flex w-100 m-1'>
            <input type='text'
                   className='form-control mx-1'
                   defaultValue={query}
                   placeholder={placeholder ?? 'Enter search query'}
                   onChange={e => setQuery(e.currentTarget.value)} />
            <button type={'button'}
                    className='btn btn-light mx-1'
                    onClick={e => {
                        e.preventDefault();
                        onSearch(query);
                    }} >
                Search
            </button>
        </div>
    );
};

export default SearchPanel;