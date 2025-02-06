/* eslint-disable react/no-unescaped-entities */
///* eslint-disable no-unused-vars */
import { useState } from 'react';
import './LoginForm.css';
import { FaUser, FaLock } from 'react-icons/fa';
import axios from 'axios';

const LoginForm = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post('https://localhost:7265/api/oracledata/login', {
                Email: email,
                Password: password,
            });
            if (response.status === 200) {
                // Stocker le token dans le localStorage
                localStorage.setItem('token', response.data.token);
                window.location.href = '/utilisateurtable';
            }
        } catch (error) {
            if (error.response) {
                alert(error.response.data);
            } else {
                console.error('Erreur lors de la connexion:', error.message);
                alert('Une erreur s\'est produite lors de la connexion.');
            }
        }
    };

    return (
        <div className="login-form-container">
            <div className='wrapper'>
                <form onSubmit={handleSubmit}>
                    <h1>Login</h1>
                    <div className='input-box'>
                        <input
                            type='text'
                            placeholder='Email'
                            required
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                        />
                        <FaUser className='icon' />
                    </div>
                    <div className='input-box'>
                        <input
                            type='password'
                            placeholder='Mot de passe'
                            required
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                        />
                        <FaLock className='icon' />
                    </div>
                    <div className='remember-forgot'>
                        <label><input type='checkbox' /> Se souvenir de moi</label>
                        <a>Mot de passe oublie ?</a>
                    </div>
                    <button type='submit'>Login</button>
                    <div className='register-link'>
                        <p>Vous n'avez pas de compte?<a href='/signup' > Creez en un</a>
                        </p>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default LoginForm;