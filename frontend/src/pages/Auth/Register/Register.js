import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { authService } from '../../../services/authService';
import './Register.css';

const Register = () => {
  const navigate = useNavigate();
  const [currentStep, setCurrentStep] = useState(1);
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState([]);
  const [formData, setFormData] = useState({
    // Step 1 - Contact Info (Essential)
    name: '',
    phone: '',
    // Step 2 - Company Info
    companyName: '',
    email: '',
    // Step 3 - Account Security
    password: '',
    confirmPassword: '',
    acceptTerms: false
  });

  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({
      ...formData,
      [name]: type === 'checkbox' ? checked : value
    });
  };

  const nextStep = () => {
    if (validateCurrentStep()) {
      if (currentStep === 1) {
        // Save lead data early for follow-up
        console.log('Lead captured:', { name: formData.name, phone: formData.phone });
        // TODO: Send lead data to backend
      }
      setCurrentStep(currentStep + 1);
    }
  };

  const prevStep = () => {
    setCurrentStep(currentStep - 1);
  };

  const validateCurrentStep = () => {
    const newErrors = [];
    
    switch (currentStep) {
      case 1:
        if (!formData.name.trim()) {
          newErrors.push('Por favor, digite seu nome');
        }
        break;
      case 2:
        if (!formData.companyName.trim()) {
          newErrors.push('Por favor, digite o nome da empresa');
        }
        if (!formData.email.trim()) {
          newErrors.push('Por favor, digite seu e-mail');
        } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
          newErrors.push('Por favor, digite um e-mail válido');
        }
        break;
      case 3:
        if (formData.password.length < 6) {
          newErrors.push('A senha deve ter pelo menos 6 caracteres');
        }
        if (formData.password !== formData.confirmPassword) {
          newErrors.push('As senhas não coincidem!');
        }
        if (!formData.acceptTerms) {
          newErrors.push('Você deve aceitar os termos de uso!');
        }
        break;
      default:
        break;
    }

    setErrors(newErrors);
    return newErrors.length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!validateCurrentStep()) {
      return;
    }

    setIsLoading(true);
    setErrors([]);

    try {
      const result = await authService.register(formData);

      if (result.success) {
        // Registration successful - redirect to dashboard or home
        console.log('Registration successful:', result);
        navigate('/dashboard', { 
          state: { 
            message: 'Conta criada com sucesso! Bem-vindo ao 99Gestor!' 
          }
        });
      } else {
        // Handle registration errors
        setErrors(result.errors || [result.message]);
      }
    } catch (error) {
      console.error('Registration error:', error);
      setErrors(['Erro inesperado. Tente novamente mais tarde.']);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="register-page">
      <div className="container">
        <div className="row justify-content-center">
          <div className="col-md-8 col-lg-6">
            <div className="register-card">
              <div className="text-center mb-4">
                <h2 className="text-success">99Gestor</h2>
                <h4>Criar sua conta</h4>
                <p className="text-muted">Comece gratuitamente hoje mesmo</p>
                
                {/* Error Messages */}
                {errors.length > 0 && (
                  <div className="alert alert-danger alert-dismissible fade show" role="alert">
                    <ul className="mb-0">
                      {errors.map((error, index) => (
                        <li key={index}>{error}</li>
                      ))}
                    </ul>
                    <button 
                      type="button" 
                      className="btn-close" 
                      onClick={() => setErrors([])}
                      aria-label="Close"
                    ></button>
                  </div>
                )}
                
                {/* Progress Steps */}
                <div className="progress-steps mb-4">
                  <div className={`step ${currentStep >= 1 ? 'active' : ''} ${currentStep > 1 ? 'completed' : ''}`}>
                    <span className="step-number">{currentStep > 1 ? '' : '1'}</span>
                    <span className="step-label">Contato</span>
                  </div>
                  <div className={`step ${currentStep >= 2 ? 'active' : ''} ${currentStep > 2 ? 'completed' : ''}`}>
                    <span className="step-number">{currentStep > 2 ? '' : '2'}</span>
                    <span className="step-label">Empresa</span>
                  </div>
                  <div className={`step ${currentStep >= 3 ? 'active' : ''} ${currentStep > 3 ? 'completed' : ''}`}>
                    <span className="step-number">{currentStep > 3 ? '' : '3'}</span>
                    <span className="step-label">Segurança</span>
                  </div>
                </div>
              </div>

              {currentStep === 1 && (
                <form onSubmit={(e) => { e.preventDefault(); nextStep(); }}>
                  <div className="step-content">
                    <h5 className="mb-3">Vamos nos conhecer!</h5>
                    <p className="text-muted mb-4">Como podemos te chamar e entrar em contato?</p>
                    
                    <div className="mb-3 text-start">
                      <label htmlFor="name" className="form-label">
                        Seu Nome *
                      </label>
                      <input
                        type="text"
                        className="form-control"
                        id="name"
                        name="name"
                        value={formData.name}
                        onChange={handleChange}
                        placeholder="João Silva"
                        required
                      />
                    </div>

                    <div className="mb-4 text-start">
                      <label htmlFor="phone" className="form-label">
                        Telefone
                      </label>
                      <input
                        type="tel"
                        className="form-control"
                        id="phone"
                        name="phone"
                        value={formData.phone}
                        onChange={handleChange}
                        placeholder="(11) 99999-9999"                        
                      />
                      <div className="form-text text-center">
                        Usaremos apenas para te ajudar com dúvidas sobre o sistema
                      </div>
                    </div>

                    <button 
                      type="submit" 
                      className="btn btn-success w-100"
                      disabled={isLoading}
                    >
                      Continuar
                    </button>
                  </div>
                </form>
              )}

              {currentStep === 2 && (
                <form onSubmit={(e) => { e.preventDefault(); nextStep(); }}>
                  <div className="step-content">
                    <h5 className="mb-3">Sobre sua empresa</h5>
                    <p className="text-muted mb-4">Precisamos dessas informações para configurar seu painel</p>
                    
                    <div className="mb-3">
                      <label htmlFor="companyName" className="form-label">
                        Nome da Empresa *
                      </label>
                      <input
                        type="text"
                        className="form-control"
                        id="companyName"
                        name="companyName"
                        value={formData.companyName}
                        onChange={handleChange}
                        placeholder="Ex: Limpeza Total Ltda"
                        required
                      />
                    </div>

                    <div className="mb-4">
                      <label htmlFor="email" className="form-label">
                        E-mail Profissional *
                      </label>
                      <input
                        type="email"
                        className="form-control"
                        id="email"
                        name="email"
                        value={formData.email}
                        onChange={handleChange}
                        placeholder="contato@suaempresa.com"
                        required
                      />
                      <div className="form-text">
                        Este será seu login no sistema
                      </div>
                    </div>

                    <div className="d-grid gap-2">
                      <button 
                        type="submit" 
                        className="btn btn-success"
                        disabled={isLoading}
                      >
                        Continuar
                      </button>
                      <button 
                        type="button" 
                        className="btn btn-outline-secondary" 
                        onClick={prevStep}
                        disabled={isLoading}
                      >
                        Voltar
                      </button>
                    </div>
                  </div>
                </form>
              )}

              {currentStep === 3 && (
                <form onSubmit={handleSubmit}>
                  <div className="step-content">
                    <h5 className="mb-3">Proteja sua conta</h5>
                    <p className="text-muted mb-4">Crie uma senha segura para acessar seu painel</p>
                    
                    <div className="row">
                      <div className="col-md-6 mb-3">
                        <label htmlFor="password" className="form-label">
                          Senha *
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
                      <div className="col-md-6 mb-3">
                        <label htmlFor="confirmPassword" className="form-label">
                          Confirmar Senha *
                        </label>
                        <div className="password-input-wrapper">
                          <input
                            type={showConfirmPassword ? 'text' : 'password'}
                            className="form-control"
                            id="confirmPassword"
                            name="confirmPassword"
                            value={formData.confirmPassword}
                            onChange={handleChange}
                            placeholder="Repita a senha"
                            required
                          />
                          <button
                            type="button"
                            className="password-toggle"
                            onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                          >
                            {showConfirmPassword ? '👁️' : '👁️‍🗨️'}
                          </button>
                        </div>
                      </div>
                    </div>

                    <div className="mb-4">
                      <div className="form-check">
                        <input
                          className="form-check-input"
                          type="checkbox"
                          id="acceptTerms"
                          name="acceptTerms"
                          checked={formData.acceptTerms}
                          onChange={handleChange}
                          required
                        />
                        <label className="form-check-label" htmlFor="acceptTerms">
                          Eu aceito os{' '}
                          <Link to="/terms" className="text-success">
                            Termos de Uso
                          </Link>{' '}
                          e{' '}
                          <Link to="/privacy" className="text-success">
                            Política de Privacidade
                          </Link>
                        </label>
                      </div>
                    </div>

                    <div className="d-grid gap-2">
                      <button 
                        type="submit" 
                        className="btn btn-success" 
                        disabled={isLoading}
                      >
                        {isLoading ? (
                          <>
                            <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                            Criando conta...
                          </>
                        ) : (
                          '🎉 Criar Conta Gratuitamente'
                        )}
                      </button>
                      <button 
                        type="button" 
                        className="btn btn-outline-secondary" 
                        onClick={prevStep}
                        disabled={isLoading}
                      >
                        Voltar
                      </button>
                    </div>
                  </div>
                </form>
              )}

              <div className="text-center">
                <p className="mt-3 mb-0">
                  Já tem uma conta?{' '}
                  <Link to="/login" className="text-success fw-bold">
                    Faça login
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

export default Register;
