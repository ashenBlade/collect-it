import React from 'react';
import NavigationPanel from "./NavigationPanel";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import UsersList from "./pages/users/UsersList";
import SubscriptionsList from "./pages/subscriptions/SubscriptionsList";
import ResourcesList from "./pages/resources/ResourcesList";

const MainApp = () => {
    return (
        <div>
            <BrowserRouter>
                <NavigationPanel />
                <Routes>
                    <Route path='/resources' element={<ResourcesList/>}/>
                    <Route path='/subscriptions' element={<SubscriptionsList/>} />
                    <Route path='/users' element={<UsersList/>}/>
                    <Route path='*' element={<UsersList/>}/>
                </Routes>
            </BrowserRouter>
        </div>
    );
};

export default MainApp;