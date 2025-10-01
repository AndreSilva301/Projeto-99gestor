import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import './ForgotPassword.css';

const ForgotPassword = () => {
  const [email, setEmail] = useState('');
  const [isSubmitted, setIsSubmitted] = useState(false);

  const handleSubmit = (e) => {
    e.preventDefault();
    // TODO: Implement forgot password logic
    console.log('Password reset request for:', email);
    setIsSubmitted(true);
  };

  if (isSubmitted) {
    return (
      <div className="forgot-password-page">
        <div className="container">
          <div className="row justify-content-center">
            <div className="col-md-6 col-lg-5">
              <div className="forgot-password-card">
                <div className="text-center mb-4">
                  <div className="success-icon mb-3">✅</div>
                  <h4>E-mail enviado!</h4>
                  <p className="text-muted">
                    Enviamos as instruções para redefinir sua senha para{' '}
                    <strong>{email}</strong>
                  </p>
                  <p className="text-muted">
                    Verifique sua caixa de entrada e siga as instruções.
                  </p>
                </div>

                <div className="d-grid gap-2">
                  <Link to="/login" className="btn btn-success">
                    Voltar ao Login
                  </Link>
                  <button
                    type="button"
                    className="btn btn-outline-secondary"
                    onClick={() => setIsSubmitted(false)}
                  >
                    Enviar novamente
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="forgot-password-page">
      <div className="container">
        <div className="row justify-content-center">
          <div className="col-md-6 col-lg-5">
            <div className="forgot-password-card">
              <div className="text-center mb-4">
                <h2 className="text-success">99Gestor</h2>
                <h4>Esqueceu sua senha?</h4>
                <p className="text-muted">
                  Digite seu e-mail e enviaremos instruções para redefinir sua senha
                </p>
              </div>

              <form onSubmit={handleSubmit}>
                <div className="mb-3 text-start">
                  <label htmlFor="email" className="form-label">
                    E-mail cadastrado
                  </label>
                  <input
                    type="email"
                    className="form-control"
                    id="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="seu@email.com"
                    required
                  />
                </div>

                <button type="submit" className="btn btn-success w-100 mb-3">
                  Enviar instruções
                </button>
              </form>

              <div className="text-center">
                <Link to="/login" className="text-success">
                  ← Voltar ao login
                </Link>
              </div>

              <hr className="my-4" />

              <div className="text-center">
                <p className="mb-0 text-muted">
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

export default ForgotPassword;
