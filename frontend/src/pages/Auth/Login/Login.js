import React, { useState, useEffect } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { authService } from '../../../services/authService';
import './Login.css';

const Login = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });
  const [showPassword, setShowPassword] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');

  useEffect(() => {
    // Check for success message from password reset
    if (location.state?.message) {
      setSuccessMessage(location.state.message);
      // Pre-fill email if provided
      if (location.state.email) {
        setFormData(prev => ({ ...prev, email: location.state.email }));
      }
      // Clear the message after 10 seconds
      setTimeout(() => setSuccessMessage(''), 10000);
    }
  }, [location.state]);

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    // Implement login logic
    var response = await authService.login(formData);
    if(response.success){
      localStorage.setItem('user', JSON.stringify({
      id: response.data.id,
      name: response.data.name,
      email: response.data.email,
      role: response.data.role,
      companyId: response.data.companyId,
      avatar: response.data.avatar
    }));

    // Navega para o portal
    navigate('/portal/', {
      state: {
        message: 'Bem-vindo ao 99Gestor!'
      }
    });

  } else {
    setSuccessMessage(
      response.errors?.join(", ") || 'Erro ao fazer login. Tente novamente.'
    );
  }
};

  return (
    <div className="login-page">
      <div className="container">
        <div className="row justify-content-center">
          <div className="col-md-6 col-lg-5">
            <div className="login-card">
              <div className="text-center mb-4">
                <h2 className="text-success">99Gestor</h2>
                <h4>Entrar na sua conta</h4>
                <p className="text-muted">Acesse seu painel de gestão</p>
              </div>

              {successMessage && (
                <div className="alert alert-success alert-dismissible fade show" role="alert">
                  {successMessage}
                  <button 
                    type="button" 
                    className="btn-close" 
                    aria-label="Close"
                    onClick={() => setSuccessMessage('')}
                  ></button>
                </div>
              )}

              <form onSubmit={handleSubmit}>
                <div className="mb-3 text-start">
                  <label htmlFor="email" className="form-label">
                    E-mail
                  </label>
                  <input
                    type="email"
                    className="form-control"
                    id="email"
                    name="email"
                    value={formData.email}
                    onChange={handleChange}
                    placeholder="seu@email.com"
                    required
                  />
                </div>

                <div className="mb-3 text-start">
                  <label htmlFor="password" className="form-label">
                    Senha
                  </label>
                  <div className="password-input-wrapper">
                    <input
                      type={showPassword ? 'text' : 'password'}
                      className="form-control"
                      id="password"
                      name="password"
                      value={formData.password}
                      onChange={handleChange}
                      placeholder="Sua senha"
                      required
                    />
                    <button
                      type="button"
                      className="password-toggle"
                      onClick={() => setShowPassword(!showPassword)}
                    >
                      {showPassword ? '👁️' : '👁️‍🗨️'}
                    </button>
                  </div>
                </div>

                <div className="mb-3 d-flex justify-content-between align-items-center">
                  <div className="form-check">
                    <input
                      className="form-check-input"
                      type="checkbox"
                      id="remember"
                    />
                    <label className="form-check-label" htmlFor="remember">
                      Lembrar-me
                    </label>
                  </div>
                  <Link to="/forgot-password" className="text-success">
                    Esqueceu a senha?
                  </Link>
                </div>

                <button type="submit" className="btn btn-success w-100 mb-3">
                  Entrar
                </button>
              </form>

              <div className="text-center">
                <p className="mb-0">
                  Ainda não tem conta?{' '}
                  <Link to="/register" className="text-success fw-bold">
                    Cadastre-se gratuitamente
                  </Link>
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;
