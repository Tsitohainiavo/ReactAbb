import { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom'; // Importez useNavigate

function UtilisateurTable() {
    const [utilisateurs, setUtilisateurs] = useState([]);
    const navigate = useNavigate(); // Utilisez useNavigate pour la redirection

    useEffect(() => {
        const token = localStorage.getItem('token');
        axios.get('https://localhost:7265/api/OracleData/utilisateurs', {
            headers: {
                Authorization: `Bearer ${token}`
            }
        })
            .then(response => response.data)
            .then(data => {
                setUtilisateurs(data);
            })
            .catch(error => {
                console.error('Erreur lors de la récupération des utilisateurs:', error);
                window.location.href = '/'; // Rediriger vers la page de connexion en cas d'erreur
            });
    }, []);

    const handleLogout = () => {
        localStorage.removeItem('token');
        window.location.href = '/';
    };

    const handleGoToAgences = () => {
        navigate('/agences'); // Redirige vers la page des agences
    };

    return (
        <div>
            <button onClick={handleLogout} style={{ float: 'right', margin: '10px' }}>
                Déconnexion
            </button>
            <button onClick={handleGoToAgences} style={{ float: 'right', margin: '10px' }}>
                Voir les agences
            </button>
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Email</th>
                        <th>Mot de passe</th>
                    </tr>
                </thead>
                <tbody>
                    {utilisateurs.map(utilisateur => (
                        <tr key={utilisateur.id}>
                            <td>{utilisateur.id}</td>
                            <td>{utilisateur.email}</td>
                            <td>{utilisateur.password}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default UtilisateurTable;