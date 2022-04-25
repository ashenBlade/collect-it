import React from 'react';
import {BrowserRouter, Navigate, Route, Routes} from "react-router-dom";
import Login from "./components/pages/login/Login";
import {AuthService} from "./services/AuthService";
import NavigationPanel from "./components/NavigationPanel";
import ResourcesList from "./components/pages/resources/ResourcesList";
import SubscriptionsList from "./components/pages/subscriptions/SubscriptionsList";
import EditSubscription from "./components/pages/subscriptions/EditSubscription";
import UsersList from "./components/pages/users/UsersList";
import EditUser from "./components/pages/users/EditUser";
import './styles/main.css'
import EditImage from "./components/pages/resources/EditImage";
import EditMusic from "./components/pages/resources/EditMusic";
import EditVideo from "./components/pages/resources/EditVideo";
import CreateSubscription from "./components/pages/subscriptions/CreateSubscription";

function App() {
    const isAuthenticated = AuthService.isAuthenticated();
    return (
        <div className={'h-100'}>
            <BrowserRouter>
                {isAuthenticated
                    ? <>
                        <NavigationPanel/>
                        <Routes>
                            <Route path='/images'>
                                <Route path=':imageId' element={<EditImage/>}/>
                            </Route>
                            <Route path='/musics'>
                                <Route path=':musicId' element={<EditMusic/>}/>

                            </Route>
                            <Route path='/videos'>
                                <Route path=':videoId' element={<EditVideo/>}/>
                            </Route>

                            {/* Deprecated */}
                            <Route path='/resources'>
                                <Route path='' element={<ResourcesList/>}/>
                            </Route>

                            <Route path='/subscriptions'>
                                <Route path='' element={<SubscriptionsList/>}/>
                                <Route path=':subscriptionId' element={<EditSubscription/>}/>
                            </Route>
                            <Route path='/users'>
                                <Route path='' element={<UsersList/>}/>
                                <Route path=':userId' element={<EditUser/>}/>
                            </Route>
                            <Route path='/create'>
                                <Route path='' element={<CreateSubscription/>}/>
                            </Route>
                            { /* Fallback */}
                            <Route path='*' element={<UsersList/>}/>
                            <Route path='/login' element={<Login/>}/>
                        </Routes>
                    </>
                    : <Routes>
                        <Route path={AuthService.loginPath()} element={<Login/>}/>
                        <Route path='*' element={<Navigate to={AuthService.loginPath()}/>}/>
                    </Routes>}
            </BrowserRouter>
        </div>
    );
}

export default App;
