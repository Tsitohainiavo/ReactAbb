/* eslint-disable react/no-unescaped-entities */
/* eslint-disable no-unused-vars */

import React from 'react';
import './LoginForm.css';
import { FaUser, FaLock } from 'react-icons/fa';

const LoginForm = () => {
    return (
        <div className='wrapper'>
            <form>
                <h1>Login</h1>
                <div className='input-box'>
                    <input type='text' placeholder='Email' required />
                    <FaUser className='icon' />
                </div>
                <div className='input-box'>
                    <input type='password' placeholder='Mot de passe' required />
                    <FaLock className='icon' />
                </div>

                <div className='remember-forgot'>
                    <label><input type='checkbox'/> Se souvenir de moi</label>
                    <a>Mot de passe oublié ?</a>
                </div>

                <button type='submit'>Login</button>

                <div className='register-link'>
                    <p>Vous n'avez pas de compte?<a> Créez en un</a> 
                    </p>
                </div>
            </form>
        </div>
    );
};

export default LoginForm;