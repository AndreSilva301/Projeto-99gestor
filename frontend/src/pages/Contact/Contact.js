import React, { useState } from 'react';
import './Contact.css';

const Contact = () => {
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    phone: '',
    service: '',
    message: ''
  });

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    // TODO: Implement form submission
    console.log('Form submitted:', formData);
    alert('Mensagem enviada! Entraremos em contato em breve.');
  };

  return (
    <div className="contact">
      <div className="container">
        <div className="text-center mb-5">
          <h1 className="display-4 mb-3">Entre em Contato</h1>
          <p className="lead">Solicite uma demonstração ou tire suas dúvidas</p>
        </div>

        <div className="row">
          <div className="col-lg-8 mx-auto">
            <div className="card">
              <div className="card-body">
                <form onSubmit={handleSubmit}>
                  <div className="row">
                    <div className="col-md-6 mb-3">
                      <label htmlFor="name" className="form-label">Nome *</label>
                      <input
                        type="text"
                        className="form-control"
                        id="name"
                        name="name"
                        value={formData.name}
                        onChange={handleChange}
                        required
                      />
                    </div>
                    <div className="col-md-6 mb-3">
                      <label htmlFor="email" className="form-label">E-mail *</label>
                      <input
                        type="email"
                        className="form-control"
                        id="email"
                        name="email"
                        value={formData.email}
                        onChange={handleChange}
                        required
                      />
                    </div>
                  </div>
                  
                  <div className="row">
                    <div className="col-md-6 mb-3">
                      <label htmlFor="phone" className="form-label">Telefone</label>
                      <input
                        type="tel"
                        className="form-control"
                        id="phone"
                        name="phone"
                        value={formData.phone}
                        onChange={handleChange}
                      />
                    </div>
                    <div className="col-md-6 mb-3">
                      <label htmlFor="service" className="form-label">Interesse em</label>
                      <select
                        className="form-control"
                        id="service"
                        name="service"
                        value={formData.service}
                        onChange={handleChange}
                      >
                        <option value="">Selecione uma opção</option>
                        <option value="starter">Plano Starter (Gratuito)</option>
                        <option value="profissional">Plano Profissional</option>
                        <option value="enterprise">Plano Enterprise</option>
                        <option value="demonstracao">Solicitar Demonstração</option>
                        <option value="duvidas">Tirar Dúvidas</option>
                      </select>
                    </div>
                  </div>

                  <div className="mb-3">
                    <label htmlFor="message" className="form-label">Mensagem *</label>
                    <textarea
                      className="form-control"
                      id="message"
                      name="message"
                      rows="5"
                      value={formData.message}
                      onChange={handleChange}
                      placeholder="Conte-nos sobre sua empresa e suas necessidades..."
                      required
                    ></textarea>
                  </div>

                  <div className="text-center">
                    <button type="submit" className="btn btn-success btn-lg">
                      Solicitar Contato
                    </button>
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>

        {/* Contact Info */}
        <div className="row mt-5">
          <div className="col-md-4 text-center mb-4">
            <div className="contact-info">
              <div className="contact-icon">📞</div>
              <h5>Telefone</h5>
              <p>(27) 99984-8700</p>
            </div>
          </div>
          <div className="col-md-4 text-center mb-4">
            <div className="contact-info">
              <div className="contact-icon">📧</div>
              <h5>E-mail</h5>
              <p>welber91@hotmail.com</p>
            </div>
          </div>
          <div className="col-md-4 text-center mb-4">
            <div className="contact-info">
              <div className="contact-icon">📍</div>
              <h5>Localização</h5>
              <p>Linhares, ES</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Contact;
