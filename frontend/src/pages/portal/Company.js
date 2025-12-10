import React, { useState, useEffect } from 'react';
import { companyService } from '../../services/companyService';
import './Company.css';

const Company = () => {
  const [company, setCompany] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState({
    name: '',
    cnpj: '',
    address: {
      street: '',
      number: '',
      complement: '',
      neighborhood: '',
      city: '',
      state: '',
      zipCode: ''
    },
    phone: {
      mobile: '',
      landline: ''
    }
  });
  const [errors, setErrors] = useState({});
  const [successMessage, setSuccessMessage] = useState('');
  const [errorMessage, setErrorMessage] = useState('');

  useEffect(() => {
    loadCompanyData();
  }, []);

  const loadCompanyData = async () => {
    setLoading(true);
    setErrorMessage('');
    
    try {
      const result = await companyService.getCurrentCompany();
      
      if (result.success && result.data) {
        setCompany(result.data);
        setFormData({
          name: result.data.name || '',
          cnpj: result.data.cnpj || '',
          address: {
            street: result.data.address?.street || '',
            number: result.data.address?.number || '',
            complement: result.data.address?.complement || '',
            neighborhood: result.data.address?.neighborhood || '',
            city: result.data.address?.city || '',
            state: result.data.address?.state || '',
            zipCode: result.data.address?.zipCode || ''
          },
          phone: {
            mobile: result.data.phone?.mobile || '',
            landline: result.data.phone?.landline || ''
          }
        });
      } else {
        setErrorMessage(result.message || 'Erro ao carregar dados da empresa');
      }
    } catch (error) {
      console.error('Error loading company:', error);
      setErrorMessage('Erro ao carregar dados da empresa');
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    
    if (name.startsWith('address.')) {
      const addressField = name.split('.')[1];
      setFormData(prev => ({
        ...prev,
        address: {
          ...prev.address,
          [addressField]: value
        }
      }));
    } else if (name.startsWith('phone.')) {
      const phoneField = name.split('.')[1];
      setFormData(prev => ({
        ...prev,
        phone: {
          ...prev.phone,
          [phoneField]: value
        }
      }));
    } else {
      setFormData(prev => ({
        ...prev,
        [name]: value
      }));
    }
    
    // Clear error for this field
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: undefined
      }));
    }
  };

  const validateForm = () => {
    const validation = companyService.validateCompanyData(formData);
    setErrors(validation.errors);
    return validation.isValid;
  };

  const handleEdit = () => {
    setIsEditing(true);
    setSuccessMessage('');
    setErrorMessage('');
  };

  const handleCancel = () => {
    setIsEditing(false);
    setErrors({});
    setSuccessMessage('');
    setErrorMessage('');
    
    // Reset form to original company data
    if (company) {
      setFormData({
        name: company.name || '',
        cnpj: company.cnpj || '',
        address: {
          street: company.address?.street || '',
          number: company.address?.number || '',
          complement: company.address?.complement || '',
          neighborhood: company.address?.neighborhood || '',
          city: company.address?.city || '',
          state: company.address?.state || '',
          zipCode: company.address?.zipCode || ''
        },
        phone: {
          mobile: company.phone?.mobile || '',
          landline: company.phone?.landline || ''
        }
      });
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    setSuccessMessage('');
    setErrorMessage('');
    
    if (!validateForm()) {
      setErrorMessage('Por favor, corrija os erros no formulário');
      return;
    }
    
    setSaving(true);
    
    try {
      const result = await companyService.updateCompany(company.id, formData);
      
      if (result.success) {
        setCompany(result.data);
        setSuccessMessage('Empresa atualizada com sucesso!');
        setIsEditing(false);
        setErrors({});
      } else {
        setErrorMessage(result.message || 'Erro ao atualizar empresa');
        if (result.errors && result.errors.length > 0) {
          // Display backend validation errors
          const backendErrors = {};
          result.errors.forEach(err => {
            // Try to map backend errors to form fields
            const errorText = err.toLowerCase();
            if (errorText.includes('nome')) backendErrors.name = err;
            else if (errorText.includes('cnpj')) backendErrors.cnpj = err;
            else if (errorText.includes('rua')) backendErrors['address.street'] = err;
            else if (errorText.includes('número')) backendErrors['address.number'] = err;
            else if (errorText.includes('bairro')) backendErrors['address.neighborhood'] = err;
            else if (errorText.includes('cidade')) backendErrors['address.city'] = err;
            else if (errorText.includes('estado')) backendErrors['address.state'] = err;
            else if (errorText.includes('cep')) backendErrors['address.zipCode'] = err;
            else if (errorText.includes('celular')) backendErrors['phone.mobile'] = err;
            else if (errorText.includes('telefone') || errorText.includes('fixo')) backendErrors['phone.landline'] = err;
          });
          setErrors(backendErrors);
        }
      }
    } catch (error) {
      console.error('Error updating company:', error);
      setErrorMessage('Erro ao atualizar empresa');
    } finally {
      setSaving(false);
    }
  };

  const formatCNPJ = (value) => {
    if (!value) return '';
    const numbers = value.replace(/\D/g, '');
    if (numbers.length <= 14) {
      return numbers.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5');
    }
    return value;
  };

  const formatPhone = (value) => {
    if (!value) return '';
    const numbers = value.replace(/\D/g, '');
    if (numbers.length <= 10) {
      return numbers.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
    } else {
      return numbers.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
    }
  };

  const formatZipCode = (value) => {
    if (!value) return '';
    const numbers = value.replace(/\D/g, '');
    return numbers.replace(/(\d{5})(\d{3})/, '$1-$2');
  };

  if (loading) {
    return (
      <div className="company-page">
        <div className="loading-container">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">Carregando...</span>
          </div>
          <p className="mt-3">Carregando dados da empresa...</p>
        </div>
      </div>
    );
  }

  if (!company && errorMessage) {
    return (
      <div className="company-page">
        <div className="alert alert-danger" role="alert">
          <i className="bi bi-exclamation-triangle me-2"></i>
          {errorMessage}
        </div>
        <button className="btn btn-primary" onClick={loadCompanyData}>
          Tentar novamente
        </button>
      </div>
    );
  }

  return (
    <div className="company-page">
      <div className="company-header">
        <div>
          <h2>
            <i className="bi bi-building me-2"></i>
            Dados da Empresa
          </h2>
          <p className="text-muted">Gerencie as informações da sua empresa</p>
        </div>
        <div>
          {!isEditing && (
            <button className="btn btn-primary" onClick={handleEdit}>
              <i className="bi bi-pencil me-2"></i>
              Editar
            </button>
          )}
        </div>
      </div>

      {successMessage && (
        <div className="alert alert-success alert-dismissible fade show" role="alert">
          <i className="bi bi-check-circle me-2"></i>
          {successMessage}
          <button type="button" className="btn-close" onClick={() => setSuccessMessage('')}></button>
        </div>
      )}

      {errorMessage && (
        <div className="alert alert-danger alert-dismissible fade show" role="alert">
          <i className="bi bi-exclamation-triangle me-2"></i>
          {errorMessage}
          <button type="button" className="btn-close" onClick={() => setErrorMessage('')}></button>
        </div>
      )}

      <form onSubmit={handleSubmit}>
        <div className="company-card">
          <h5 className="section-title">
            <i className="bi bi-info-circle me-2"></i>
            Informações Básicas
          </h5>
          
          <div className="row">
            <div className="col-md-6 mb-3">
              <label htmlFor="name" className="form-label">
                Nome da Empresa *
              </label>
              <input
                type="text"
                className={`form-control ${errors.name ? 'is-invalid' : ''}`}
                id="name"
                name="name"
                value={formData.name}
                onChange={handleInputChange}
                disabled={!isEditing}
                required
              />
              {errors.name && <div className="invalid-feedback">{errors.name}</div>}
            </div>

            <div className="col-md-6 mb-3">
              <label htmlFor="cnpj" className="form-label">
                CNPJ
              </label>
              <input
                type="text"
                className={`form-control ${errors.cnpj ? 'is-invalid' : ''}`}
                id="cnpj"
                name="cnpj"
                value={isEditing ? formData.cnpj : formatCNPJ(formData.cnpj)}
                onChange={handleInputChange}
                disabled={!isEditing}
                placeholder="00.000.000/0000-00"
              />
              {errors.cnpj && <div className="invalid-feedback">{errors.cnpj}</div>}
            </div>
          </div>
        </div>

        <div className="company-card">
          <h5 className="section-title">
            <i className="bi bi-geo-alt me-2"></i>
            Endereço
          </h5>
          
          <div className="row">
            <div className="col-md-8 mb-3">
              <label htmlFor="address.street" className="form-label">
                Logradouro *
              </label>
              <input
                type="text"
                className={`form-control ${errors.addressStreet ? 'is-invalid' : ''}`}
                id="address.street"
                name="address.street"
                value={formData.address.street}
                onChange={handleInputChange}
                disabled={!isEditing}
                required
              />
              {errors.addressStreet && <div className="invalid-feedback">{errors.addressStreet}</div>}
            </div>

            <div className="col-md-4 mb-3">
              <label htmlFor="address.number" className="form-label">
                Número *
              </label>
              <input
                type="text"
                className={`form-control ${errors.addressNumber ? 'is-invalid' : ''}`}
                id="address.number"
                name="address.number"
                value={formData.address.number}
                onChange={handleInputChange}
                disabled={!isEditing}
                required
              />
              {errors.addressNumber && <div className="invalid-feedback">{errors.addressNumber}</div>}
            </div>
          </div>

          <div className="row">
            <div className="col-md-4 mb-3">
              <label htmlFor="address.complement" className="form-label">
                Complemento
              </label>
              <input
                type="text"
                className="form-control"
                id="address.complement"
                name="address.complement"
                value={formData.address.complement}
                onChange={handleInputChange}
                disabled={!isEditing}
              />
            </div>

            <div className="col-md-4 mb-3">
              <label htmlFor="address.neighborhood" className="form-label">
                Bairro *
              </label>
              <input
                type="text"
                className={`form-control ${errors.addressNeighborhood ? 'is-invalid' : ''}`}
                id="address.neighborhood"
                name="address.neighborhood"
                value={formData.address.neighborhood}
                onChange={handleInputChange}
                disabled={!isEditing}
                required
              />
              {errors.addressNeighborhood && <div className="invalid-feedback">{errors.addressNeighborhood}</div>}
            </div>

            <div className="col-md-4 mb-3">
              <label htmlFor="address.zipCode" className="form-label">
                CEP *
              </label>
              <input
                type="text"
                className={`form-control ${errors.addressZipCode ? 'is-invalid' : ''}`}
                id="address.zipCode"
                name="address.zipCode"
                value={isEditing ? formData.address.zipCode : formatZipCode(formData.address.zipCode)}
                onChange={handleInputChange}
                disabled={!isEditing}
                placeholder="00000-000"
                required
              />
              {errors.addressZipCode && <div className="invalid-feedback">{errors.addressZipCode}</div>}
            </div>
          </div>

          <div className="row">
            <div className="col-md-6 mb-3">
              <label htmlFor="address.city" className="form-label">
                Cidade *
              </label>
              <input
                type="text"
                className={`form-control ${errors.addressCity ? 'is-invalid' : ''}`}
                id="address.city"
                name="address.city"
                value={formData.address.city}
                onChange={handleInputChange}
                disabled={!isEditing}
                required
              />
              {errors.addressCity && <div className="invalid-feedback">{errors.addressCity}</div>}
            </div>

            <div className="col-md-6 mb-3">
              <label htmlFor="address.state" className="form-label">
                Estado *
              </label>
              <input
                type="text"
                className={`form-control ${errors.addressState ? 'is-invalid' : ''}`}
                id="address.state"
                name="address.state"
                value={formData.address.state}
                onChange={handleInputChange}
                disabled={!isEditing}
                maxLength={2}
                placeholder="SP"
                required
              />
              {errors.addressState && <div className="invalid-feedback">{errors.addressState}</div>}
            </div>
          </div>
        </div>

        <div className="company-card">
          <h5 className="section-title">
            <i className="bi bi-telephone me-2"></i>
            Contato
          </h5>
          
          <div className="row">
            <div className="col-md-6 mb-3">
              <label htmlFor="phone.mobile" className="form-label">
                Celular
              </label>
              <input
                type="text"
                className={`form-control ${errors['phone.mobile'] ? 'is-invalid' : ''}`}
                id="phone.mobile"
                name="phone.mobile"
                value={isEditing ? formData.phone.mobile : formatPhone(formData.phone.mobile)}
                onChange={handleInputChange}
                disabled={!isEditing}
                placeholder="(00) 00000-0000"
              />
              {errors['phone.mobile'] && <div className="invalid-feedback">{errors['phone.mobile']}</div>}
            </div>

            <div className="col-md-6 mb-3">
              <label htmlFor="phone.landline" className="form-label">
                Telefone Fixo
              </label>
              <input
                type="text"
                className={`form-control ${errors['phone.landline'] ? 'is-invalid' : ''}`}
                id="phone.landline"
                name="phone.landline"
                value={isEditing ? formData.phone.landline : formatPhone(formData.phone.landline)}
                onChange={handleInputChange}
                disabled={!isEditing}
                placeholder="(00) 0000-0000"
              />
              {errors['phone.landline'] && <div className="invalid-feedback">{errors['phone.landline']}</div>}
            </div>
          </div>
        </div>

        {isEditing && (
          <div className="form-actions">
            <button 
              type="button" 
              className="btn btn-secondary" 
              onClick={handleCancel}
              disabled={saving}
            >
              <i className="bi bi-x-circle me-2"></i>
              Cancelar
            </button>
            <button 
              type="submit" 
              className="btn btn-primary"
              disabled={saving}
            >
              {saving ? (
                <>
                  <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                  Salvando...
                </>
              ) : (
                <>
                  <i className="bi bi-check-circle me-2"></i>
                  Salvar Alterações
                </>
              )}
            </button>
          </div>
        )}
      </form>
    </div>
  );
};

export default Company;
