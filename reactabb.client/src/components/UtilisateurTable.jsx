import { useState, useEffect } from 'react';
import axios from 'axios';

function UtilisateurTable() {
    const [utilisateurs, setEmployees] = useState([]);
    //const [loading, setLoading] = useState(true);

    useEffect(() => {
        axios.get('https://localhost:7265/api/OracleData/utilisateurs')
            .then(response => response.data)
            .then(data => {
                setEmployees(data);
                //setLoading(false);
            });
    }, []);

    return (
        <div>
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
                            {/*<td>{new Date(utilisateur.hireDate).toLocaleDateString()}</td>*/}
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default UtilisateurTable;