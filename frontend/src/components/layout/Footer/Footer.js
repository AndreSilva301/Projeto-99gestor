import React from 'react';
import './Footer.css';

const Footer = () => {
  return (
    <footer className="footer bg-dark text-white mt-5">
      <div className="container">
        <div className="row py-4">
          <div className="col-md-6">
            <h5>99Gestor</h5>
            <p className="mb-0">
              CRM especializado para prestadores de serviços em geral. Organize clientes, crie orçamentos profissionais e fortaleça relacionamentos.
            </p>
          </div>
          <div className="col-md-6">
            <h6>Contato</h6>
            <p className="mb-1">📞 (11) 9999-9999</p>
            <p className="mb-1">📧 contato@99gestor.com.br</p>
            <p className="mb-0">📍 São Paulo, SP</p>
          </div>
        </div>
        <hr />
        <div className="text-center">
          <small>&copy; 2025 99Gestor. Todos os direitos reservados.</small>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
