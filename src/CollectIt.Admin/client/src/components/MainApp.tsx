import React from 'react';
import NavigationPanel from "./NavigationPanel";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import UsersList from "./pages/users/UsersList";
import SubscriptionsList from "./pages/subscriptions/SubscriptionsList";
import ResourcesList from "./pages/resources/ResourcesList";
import EditSubscription from "./pages/subscriptions/EditSubscription";
import EditUser from "./pages/users/EditUser";

const MainApp = () => {
    return (
        <div>
            <BrowserRouter>
                <NavigationPanel />
                <Routes>
                    <Route path='/resources'>
                        <Route path='' element={<ResourcesList/>}/>
                        <Route path=':resourceId' element={null}/>
                    </Route>

                    <Route path='/subscriptions'>
                        <Route path='' element={<SubscriptionsList/>}/>
                        <Route path=':subscriptionId' element={<EditSubscription/>}/>
                    </Route>
                    <Route path='/users'>
                        <Route path='' element={<UsersList/>}/>
                        <Route path=':userId' element={<EditUser/>}/>
                    </Route>
                    { /* Fallback */ }
                    <Route path='*' element={<UsersList/>}/>
                </Routes>
            </BrowserRouter>
        </div>
    );
};

export default MainApp;