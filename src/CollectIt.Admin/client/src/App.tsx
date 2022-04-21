import React from 'react';
import {BrowserRouter, Navigate, Route, Routes} from "react-router-dom";
import Login from "./components/pages/login/Login";
import {AdminAuthContext} from "./services/AuthService";
import NavigationPanel from "./components/NavigationPanel";
import ResourcesList from "./components/pages/resources/ResourcesList";
import SubscriptionsList from "./components/pages/subscriptions/SubscriptionsList";
import EditSubscription from "./components/pages/subscriptions/EditSubscription";
import UsersList from "./components/pages/users/UsersList";
import EditUser from "./components/pages/users/EditUser";
import EditResource from "./components/pages/resources/EditResource";
import './styles/main.css'

function App() {
    const auth = React.useContext(AdminAuthContext);
    const isAuthenticated = auth.isAuthenticated();
  return (
      <div className={'h-100'}>
          <BrowserRouter>
              {isAuthenticated
                  ? <>
                      <NavigationPanel/>
                      <Routes>
                          <Route path='/resources'>
                              <Route path='' element={<ResourcesList/>}/>
                              <Route path=':resourceId' element={<EditResource/>}/>
                          </Route>

                          <Route path='/subscriptions'>
                              <Route path='' element={<SubscriptionsList/>}/>
                              <Route path=':subscriptionId' element={<EditSubscription/>}/>
                          </Route>
                          <Route path='/users'>
                              <Route path='' element={<UsersList/>}/>
                              <Route path=':userId' element={<EditUser/>}/>
                          </Route>
                          { /* Fallback */}
                          <Route path='*' element={<UsersList/>}/>
                          <Route path='/login' element={<Login/>}/>
                      </Routes>
                  </>
                  : <Routes>
                      <Route path={auth.loginPath()} element={<Login/>}/>
                      <Route path='*' element={<Navigate to={auth.loginPath()}/>}/>
                  </Routes>}
          </BrowserRouter>
      </div>
  );
}

export default App;
