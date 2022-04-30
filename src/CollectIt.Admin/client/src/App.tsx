import React from 'react';
import {BrowserRouter, Navigate, Route, Routes} from "react-router-dom";
import Login from "./components/pages/login/Login";
import {AuthService} from "./services/AuthService";
import NavigationPanel from "./components/UI/NavBar/NavigationPanel";
import SubscriptionsList from "./components/pages/subscriptions/SubscriptionsList";
import EditSubscription from "./components/pages/subscriptions/EditSubscription";
import UsersList from "./components/pages/users/UsersList";
import EditUser from "./components/pages/users/EditUser";
import './styles/main.css'
import EditImage from "./components/pages/images/EditImage";
import EditMusic from "./components/pages/musics/EditMusic";
import EditVideo from "./components/pages/videos/EditVideo";
import CreateSubscription from "./components/pages/subscriptions/CreateSubscription";
import MusicList from "./components/pages/musics/MusicList";
import ImageList from "./components/pages/images/ImageList";
import VideoList from "./components/pages/videos/VideoList";

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
                                <Route path='' element={<ImageList/>}/>
                                <Route path=':imageId' element={<EditImage/>}/>
                            </Route>
                            <Route path='/musics'>
                                <Route path='' element={<MusicList/>}/>
                                <Route path=':musicId' element={<EditMusic/>}/>
                            </Route>
                            <Route path='/videos'>
                                <Route path='' element={<VideoList/>}/>
                                <Route path=':videoId' element={<EditVideo/>}/>
                            </Route>
                            <Route path='/subscriptions'>
                                <Route path='' element={<SubscriptionsList/>}/>
                                <Route path=':subscriptionId' element={<EditSubscription/>}/>
                                <Route path='create' element={<CreateSubscription/>}/>
                            </Route>
                            <Route path='/users'>
                                <Route path='' element={<UsersList/>}/>
                                <Route path=':userId' element={<EditUser/>}/>
                            </Route>
                            { /* Fallback */}1
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
