/* eslint-disable react/no-unescaped-entities */
/* eslint-disable no-unused-vars */
import  { useEffect, useState } from 'react';
import axios from 'axios';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css'; // Importez le CSS de Leaflet

const AgenceMap = () => {
    const [agences, setAgences] = useState([]);

    useEffect(() => {
        const token = localStorage.getItem('token');
        axios.get('https://localhost:7265/api/Agence', {
            headers: {
                Authorization: `Bearer ${token}`
            }
        })
            .then(response => {
                setAgences(response.data);
                initMap(response.data);
            })
            .catch(error => {
                console.error('Erreur lors de la récupération des agences:', error);
            });
    }, []);

    const initMap = (agences) => {
        // Créez une carte centrée sur Madagascar
        const map = L.map('map').setView([-18.766947, 46.869107], 6);

        // Ajoutez une couche de tuiles OpenStreetMap
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© OpenStreetMap contributors'
        }).addTo(map);

        // Ajoutez des marqueurs pour chaque agence
        agences.forEach(agence => {
            L.marker([agence.latitude, agence.longitude])
                .addTo(map)
                .bindPopup(agence.nom); // Affichez le nom de l'agence dans un popup
        });
    };

    return (
        <div>
            <h1>Agences Caisse d'Épargne de Madagascar</h1>
            <div id="map" style={{ height: '500px', width: '100%' }}></div>
        </div>
    );
};

export default AgenceMap;