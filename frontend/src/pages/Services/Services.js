import React from 'react';
import './Services.css';

const Services = () => {
  const services = [
    {
      title: 'Plano Starter',
      description: 'Ideal para prestadores autônomos',
      features: ['Até 50 clientes', 'Orçamentos em PDF/Imagem', 'Cadastro via agenda telefônica', 'Gestão básica de relacionamento', 'Suporte por email'],
      price: 'Gratuito'
    },
    {
      title: 'Plano Profissional',
      description: 'Para pequenas e médias empresas',
      features: ['Clientes ilimitados', 'Múltiplos colaboradores', 'Agenda de serviços', 'Avaliações de clientes', 'CRM proativo', 'Dashboard completo', 'Suporte prioritário'],
      price: 'R$ 49,90/mês'
    },
    {
      title: 'Plano Enterprise',
      description: 'Para grandes prestadores de serviço',
      features: ['Todas as funcionalidades', 'Campos customizáveis', 'Integrações via API', 'Marca personalizada', 'Treinamento especializado', 'Suporte 24/7'],
      price: 'Sob consulta'
    }
  ];

  return (
    <div className="services">
      <div className="container">
        <div className="text-center mb-5">
          <h1 className="display-4 mb-3">Nossos Planos</h1>
          <p className="lead">Escolha o plano ideal para seu negócio de prestação de serviços</p>
        </div>

        <div className="row">
          {services.map((service, index) => (
            <div key={index} className="col-lg-4 col-md-6 mb-4">
              <div className="card service-card h-100">
                <div className="card-body">
                  <h5 className="card-title">{service.title}</h5>
                  <p className="card-text">{service.description}</p>
                  
                  <h6>O que inclui:</h6>
                  <ul className="service-features">
                    {service.features.map((feature, idx) => (
                      <li key={idx}>{feature}</li>
                    ))}
                  </ul>
                  
                  <div className="price-tag">
                    <strong>{service.price}</strong>
                  </div>
                </div>
                <div className="card-footer">
                  <button className="btn btn-success btn-block w-100">
                    Contratar Plano
                  </button>
                </div>
              </div>
            </div>
            ))}
        </div>

        {/* What's Included Section */}
        <div className="what-included-section py-5 mt-5 bg-light">
          <div className="text-center mb-5">
            <h2>O que está incluído em todos os planos</h2>
          </div>
          
          <div className="row">
            <div className="col-md-6 col-lg-3 mb-4">
              <div className="included-item text-center">
                <div className="included-icon">👥</div>
                <h6>Gestão de Clientes</h6>
                <p>Cadastro via agenda telefônica ou formulário</p>
              </div>
            </div>
            <div className="col-md-6 col-lg-3 mb-4">
              <div className="included-item text-center">
                <div className="included-icon">📄</div>
                <h6>Orçamentos Profissionais</h6>
                <p>Export em PDF e imagem com sua marca</p>
              </div>
            </div>
            <div className="col-md-6 col-lg-3 mb-4">
              <div className="included-item text-center">
                <div className="included-icon">📱</div>
                <h6>Interface Mobile</h6>
                <p>Otimizado para uso em smartphones</p>
              </div>
            </div>
            <div className="col-md-6 col-lg-3 mb-4">
              <div className="included-item text-center">
                <div className="included-icon">🔒</div>
                <h6>Segurança</h6>
                <p>Dados protegidos e backup automático</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};export default Services;
