import React from 'react';
import './App.css';
import {BrowserRouter, Navigate, Route, Routes} from "react-router-dom";
import Login from "./Login";
import {AuthContext} from "./AuthService";

function App() {
    const auth = React.useContext(AuthContext);
    const isAuthenticated = auth.isAuthenticated();

  return (
      <html>
      <head>
          <link
              rel="stylesheet"
              href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css"
              integrity="sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3"
              crossOrigin="anonymous"
          />
      </head>
      <body>
      <div>
          <BrowserRouter>
              <Routes>
                  <Route path="/" element={
                  isAuthenticated
                      ? <div className='text-danger'>This is authorized page</div>
                      : <Navigate to='/login'/>
              }/>
                  <Route path="/login" element={<Login/>}/>
              </Routes>
          </BrowserRouter>
      </div>
      </body>
      </html>
  );
}

export default App;
