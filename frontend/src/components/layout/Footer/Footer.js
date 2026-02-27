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
            <p className="mb-1">📞 (12) 34567-8901</p>
            <p className="mb-1">📧 maniadelimpeza@gmail.com</p>
            <p className="mb-0">📍 Brasil, ES</p>
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
