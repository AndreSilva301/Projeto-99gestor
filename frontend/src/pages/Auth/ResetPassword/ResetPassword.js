import React, { useState, useEffect } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import './ResetPassword.css';

const ResetPassword = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    password: '',
    confirmPassword: ''
  });
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [email, setEmail] = useState('');
  const [token, setToken] = useState('');
  const [isValidToken, setIsValidToken] = useState(false);
  const [isVerifying, setIsVerifying] = useState(true);

  useEffect(() => {
    // Get token and email from URL parameters
    const resetToken = searchParams.get('token');
    const userEmail = searchParams.get('email');
    
    if (!resetToken) {
      // Redirect to forgot password if no token
      navigate('/forgot-password');
      return;
    }

    setToken(resetToken);
    
    // Verify token with backend
    verifyResetToken(resetToken, userEmail);
  }, [searchParams, navigate]);

  const verifyResetToken = async (resetToken, userEmail) => {
    setIsVerifying(true);
    try {
      // TODO: Replace with actual API call
      console.log('Verifying token:', resetToken);
      
      // Simulate API call
      setTimeout(() => {
        // Mock validation - replace with real API call
        if (resetToken && resetToken.length > 10) {
          setIsValidToken(true);
          setEmail(userEmail || 'usuario@exemplo.com'); // Use email from API response
        } else {
          setIsValidToken(false);
        }
        setIsVerifying(false);
      }, 1000);
      
      /* Real API call would be:
      const response = await fetch(`/api/auth/verify-reset-token`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ token: resetToken })
      });
      
      if (response.ok) {
        const data = await response.json();
        setIsValidToken(true);
        setEmail(data.email);
      } else {
        setIsValidToken(false);
      }
      setIsVerifying(false);
      */
      
    } catch (error) {
      console.error('Error verifying token:', error);
      setIsValidToken(false);
      setIsVerifying(false);
    }
  };

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (formData.password.length < 6) {
      alert('A senha deve ter pelo menos 6 caracteres');
      return;
    }
    
    if (formData.password !== formData.confirmPassword) {
      alert('As senhas não coincidem!');
      return;
    }

    setIsLoading(true);
    
    try {
      // TODO: Replace with actual API call
      console.log('Resetting password for:', email, 'with token:', token);
      
      // Simulate API call
      setTimeout(() => {
        alert('Senha redefinida com sucesso!');
        navigate('/login', { 
          state: { 
            message: 'Senha redefinida com sucesso! Faça login com sua nova senha.',
            email: email 
          }
        });
      }, 2000);
      
      /* Real API call would be:
      const response = await fetch('/api/auth/reset-password', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          token: token,
          password: formData.password
        })
      });

      if (response.ok) {
        navigate('/login', { 
          state: { 
            message: 'Senha redefinida com sucesso! Faça login com sua nova senha.',
            email: email 
          }
        });
      } else {
        const error = await response.json();
        alert(error.message || 'Erro ao redefinir senha. Tente novamente.');
      }
      */
      
    } catch (error) {
      console.error('Error resetting password:', error);
      alert('Erro ao redefinir senha. Tente novamente.');
    } finally {
      setIsLoading(false);
    }
  };

  if (isVerifying) {
    return (
      <div className="reset-password-page">
        <div className="container">
          <div className="row justify-content-center">
            <div className="col-md-6 col-lg-5">
              <div className="reset-password-card">
                <div className="text-center">
                  <div className="spinner-border text-success mb-3" role="status">
                    <span className="visually-hidden">Carregando...</span>
                  </div>
                  <h4>Verificando link de recuperação...</h4>
                  <p className="text-muted">Aguarde um momento</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  if (!isValidToken) {
    return (
      <div className="reset-password-page">
        <div className="container">
          <div className="row justify-content-center">
            <div className="col-md-6 col-lg-5">
              <div className="reset-password-card">
                <div className="text-center mb-4">
                  <div className="error-icon mb-3">❌</div>
                  <h4>Link inválido ou expirado</h4>
                  <p className="text-muted">
                    Este link de recuperação de senha não é válido ou já expirou.
                  </p>
                </div>

                <div className="d-grid gap-2">
                  <Link to="/forgot-password" className="btn btn-success">
                    Solicitar novo link
                  </Link>
                  <Link to="/login" className="btn btn-outline-secondary">
                    Voltar ao login
                  </Link>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="reset-password-page">
      <div className="container">
        <div className="row justify-content-center">
          <div className="col-md-6 col-lg-5">
            <div className="reset-password-card">
              <div className="text-center mb-4">
                <h2 className="text-success">99Gestor</h2>
                <h4>Redefinir senha</h4>
                <p className="text-muted">
                  Criando nova senha para: <strong>{email}</strong>
                </p>
              </div>

              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label htmlFor="password" className="form-label">
                    Nova Senha *
                  </label>
                  <div className="password-input-wrapper">
                    <input
                      type={showPassword ? 'text' : 'password'}
                      className="form-control"
                      id="password"
                      name="password"
                      value={formData.password}
                      onChange={handleChange}
                      placeholder="Mínimo 6 caracteres"
                      minLength="6"
                      required
                      disabled={isLoading}
                    />
                    <button
                      type="button"
                      className="password-toggle"
                      onClick={() => setShowPassword(!showPassword)}
                      disabled={isLoading}
                    >
                      {showPassword ? '👁️' : '👁️‍🗨️'}
                    </button>
                  </div>
                </div>

                <div className="mb-4">
                  <label htmlFor="confirmPassword" className="form-label">
                    Confirmar Nova Senha *
                  </label>
                  <div className="password-input-wrapper">
                    <input
                      type={showConfirmPassword ? 'text' : 'password'}
                      className="form-control"
                      id="confirmPassword"
                      name="confirmPassword"
                      value={formData.confirmPassword}
                      onChange={handleChange}
                      placeholder="Repita a nova senha"
                      required
                      disabled={isLoading}
                    />
                    <button
                      type="button"
                      className="password-toggle"
                      onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                      disabled={isLoading}
                    >
                      {showConfirmPassword ? '👁️' : '👁️‍🗨️'}
                    </button>
                  </div>
                </div>

                <button 
                  type="submit" 
                  className="btn btn-success w-100 mb-3"
                  disabled={isLoading}
                >
                  {isLoading ? (
                    <>
                      <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                      Redefinindo...
                    </>
                  ) : (
                    'Redefinir Senha'
                  )}
                </button>
              </form>

              <div className="text-center">
                <Link to="/login" className="text-success">
                  ← Voltar ao login
                </Link>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ResetPassword;
