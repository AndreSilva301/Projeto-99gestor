import React from 'react';
import './Home.css';

const Home = () => {
  return (
    <div className="home">
      <div className="container">
        {/* Hero Section */}
        <div className="hero-section text-center py-5">
          <h1 className="display-4 mb-4">
            Bem-vindo ao <span className="text-success">99Gestor</span>
          </h1>
          <p className="lead mb-4">
            CRM especializado para prestadores de serviços em geral
          </p>
          <button className="btn btn-success btn-lg">
            Comece Gratuitamente
          </button>
        </div>

        {/* Services Preview */}
        <div className="row py-5">
          <div className="col-md-4 text-center mb-4">
            <div className="card h-100">
              <div className="card-body">
                <div className="service-icon mb-3">
                  🏠
                </div>
                <h5 className="card-title">Gestão de Clientes</h5>
                <p className="card-text">
                  Cadastre clientes via agenda telefônica, gerencie informações de relacionamento e histórico completo.
                </p>
              </div>
            </div>
          </div>
          <div className="col-md-4 text-center mb-4">
            <div className="card h-100">
              <div className="card-body">
                <div className="service-icon mb-3">
                  📈
                </div>
                <h5 className="card-title">Orçamentos Profissionais</h5>
                <p className="card-text">
                  Crie orçamentos detalhados e exporte em PDF ou imagem com sua marca personalizada.
                </p>
              </div>
            </div>
          </div>
          <div className="col-md-4 text-center mb-4">
            <div className="card h-100">
              <div className="card-body">
                <div className="service-icon mb-3">
                  📄
                </div>
                <h5 className="card-title">CRM Proativo</h5>
                <p className="card-text">
                  Receba insights automáticos para fortalecer relacionamentos e gerar recorrência.
                </p>
              </div>
            </div>
          </div>
        </div>

        {/* Features Section */}
        <div className="features-section py-5 bg-light">
          <div className="text-center mb-5">
            <h2>Perfeito para diversos segmentos de serviços</h2>
            <p className="lead">Limpeza, manutenção, estética, consultorias e muito mais</p>
          </div>
          
          <div className="row">
            <div className="col-md-6 mb-4">
              <div className="feature-item">
                <h5>📱 Mobile-First</h5>
                <p>Interface otimizada para celular, perfeita para quem trabalha em campo</p>
              </div>
            </div>
            <div className="col-md-6 mb-4">
              <div className="feature-item">
                <h5>👥 Gestão de Equipes</h5>
                <p>Administradores podem cadastrar colaboradores e gerenciar permissões</p>
              </div>
            </div>
            <div className="col-md-6 mb-4">
              <div className="feature-item">
                <h5>⭐ Avaliações</h5>
                <p>Colete feedback dos clientes e acompanhe a qualidade dos serviços</p>
              </div>
            </div>
            <div className="col-md-6 mb-4">
              <div className="feature-item">
                <h5>📊 Dashboard Inteligente</h5>
                <p>Acompanhe serviços em andamento, orçamentos e relacionamento com clientes</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Home;
