/* eslint-disable no-unused-vars */
import React from 'react';
import UtilisateurTable from './components/UtilisateurTable';
import { useAuth } from './components/LoginForm/AuthContext';
import { useNavigate } from 'react-router-dom';

function Dashboard() {
    const { logout, user } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/');
    };

    return (
        <div className="container mt-4">
            <div className="d-flex justify-content-between align-items-center mb-4">
                <h1>Liste des Utilisateurs</h1>
                <div>
                    <span className="me-3">Connecté en tant que: {user?.email}</span>
                    <button
                        className="btn btn-danger"
                        onClick={handleLogout}
                    >
                        Déconnexion
                    </button>
                </div>
            </div>
            <UtilisateurTable />
        </div>
    );
}

export default Dashboard;