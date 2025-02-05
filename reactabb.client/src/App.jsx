/* eslint-disable no-unused-vars */
import React from 'react';
import UtilisateurTable from './components/UtilisateurTable';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Login from './Login.jsx';
//import Dashboard from './Dashboard';
function App() {
    return (
        <Router>
            <div className="container mt-4">
                <Routes>
                    <Route path='/' element={<Login />} />
                    <Route path='/utilisateurtable' element={<UtilisateurTable />} />
                    {/*<App path='/dashboard' element={<Dashboard />} />*/}
                </Routes>
            </div>
        </Router>
    );
}

export default App;