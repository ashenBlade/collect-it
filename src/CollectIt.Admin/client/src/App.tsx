import React from 'react';
import './App.css';
import {BrowserRouter, Navigate, Route, Routes} from "react-router-dom";
import Login from "./Login";
import {AdminAuthContext} from "./services/AuthService";
import NavigationPanel from "./components/NavigationPanel";
import ResourcesList from "./components/pages/resources/ResourcesList";
import SubscriptionsList from "./components/pages/subscriptions/SubscriptionsList";
import EditSubscription from "./components/pages/subscriptions/EditSubscription";
import UsersList from "./components/pages/users/UsersList";
import EditUser from "./components/pages/users/EditUser";
import EditResource from "./components/pages/resources/EditResource";

function App() {
    const auth = React.useContext(AdminAuthContext);
    const isAuthenticated = auth.isAuthenticated();

  return (

      <div>
          <BrowserRouter>
              {isAuthenticated && <NavigationPanel/>}
              <Routes>
                  <Route path='/login' element={<Login/>}/>
                  {isAuthenticated
                      ? <>
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
                      </>
                      : <Route path='*' element={<Navigate to='/login'/>}/> }
              </Routes>
          </BrowserRouter>
      </div>
  );
}

export default App;
