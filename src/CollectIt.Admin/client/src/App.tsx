import React from 'react';
import './App.css';
import {BrowserRouter, Navigate, Route, Routes} from "react-router-dom";
import Login from "./Login";
import {AdminAuthContext} from "./services/AuthService";
import MainApp from "./components/MainApp";

function App() {
    const auth = React.useContext(AdminAuthContext);
    const isAuthenticated = auth.isAuthenticated();

  return (

      <div>
          { isAuthenticated
          ? <MainApp/>
          : <Login/> }
      </div>
  );
}

export default App;
