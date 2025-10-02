import React, { useState, useEffect } from 'react';
import Icon from '../common/Icon';

const CustomerForm = ({ 
  customer = null, 
  onSubmit, 
  onCancel, 
  loading = false,
  mode = 'create' // 'create', 'edit', 'view'
}) => {
  const [formData, setFormData] = useState({
    name: '',
    phone: '',
    email: '',
    address: ''
  });
  
  const [errors, setErrors] = useState({});
  const [touched, setTouched] = useState({});

  useEffect(() => {
    if (customer && mode !== 'create') {
      setFormData({
        name: customer.name || '',
        phone: customer.phone || '',
        email: customer.email || '',
        address: customer.address || ''
      });
    }
  }, [customer, mode]);

  const validateField = (name, value) => {
    const newErrors = { ...errors };

    switch (name) {
      case 'name':
        if (!value.trim()) {
          newErrors.name = 'Nome é obrigatório';
        } else if (value.trim().length < 2) {
          newErrors.name = 'Nome deve ter pelo menos 2 caracteres';
        } else {
          delete newErrors.name;
        }
        break;

      case 'phone':
        if (!value.trim()) {
          newErrors.phone = 'Telefone é obrigatório';
        } else {
          const phoneNumbers = value.replace(/\D/g, '');
          if (phoneNumbers.length < 10 || phoneNumbers.length > 11) {
            newErrors.phone = 'Telefone deve ter 10 ou 11 dígitos';
          } else {
            delete newErrors.phone;
          }
        }
        break;

      case 'email':
        if (!value.trim()) {
          newErrors.email = 'E-mail é obrigatório';
        } else {
          const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
          if (!emailRegex.test(value)) {
            newErrors.email = 'E-mail inválido';
          } else {
            delete newErrors.email;
          }
        }
        break;

      case 'address':
        if (!value.trim()) {
          newErrors.address = 'Endereço é obrigatório';
        } else if (value.trim().length < 10) {
          newErrors.address = 'Endereço deve ter pelo menos 10 caracteres';
        } else {
          delete newErrors.address;
        }
        break;

      default:
        break;
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    
    // Format phone as user types
    if (name === 'phone') {
      const numbers = value.replace(/\D/g, '');
      let formatted = numbers;
      
      if (numbers.length >= 2) {
        formatted = `(${numbers.slice(0, 2)}) `;
        if (numbers.length >= 7) {
          formatted += `${numbers.slice(2, 7)}-${numbers.slice(7, 11)}`;
        } else if (numbers.length > 2) {
          formatted += numbers.slice(2);
        }
      }
      
      setFormData(prev => ({ ...prev, [name]: formatted }));
    } else {
      setFormData(prev => ({ ...prev, [name]: value }));
    }

    if (touched[name]) {
      validateField(name, value);
    }
  };

  const handleBlur = (e) => {
    const { name, value } = e.target;
    setTouched(prev => ({ ...prev, [name]: true }));
    validateField(name, value);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    
    // Validate all fields
    const fieldsToValidate = ['name', 'phone', 'email', 'address'];
    let isValid = true;
    
    fieldsToValidate.forEach(field => {
      const fieldValid = validateField(field, formData[field]);
      if (!fieldValid) isValid = false;
    });

    setTouched(fieldsToValidate.reduce((acc, field) => ({ ...acc, [field]: true }), {}));

    if (isValid) {
      onSubmit(formData);
    }
  };

  const isViewMode = mode === 'view';
  const isEditMode = mode === 'edit';
  const isCreateMode = mode === 'create';

  const getTitle = () => {
    switch (mode) {
      case 'create': return 'Novo Cliente';
      case 'edit': return 'Editar Cliente';
      case 'view': return 'Detalhes do Cliente';
      default: return 'Cliente';
    }
  };

  return (
    <div className="customer-form">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h4 className="mb-0">
          <Icon name={isCreateMode ? 'user-plus' : isEditMode ? 'user-edit' : 'user'} className="me-2" />
          {getTitle()}
        </h4>
        {isViewMode && (
          <div className="btn-group">
            <button
              type="button"
              className="btn btn-outline-primary"
              onClick={() => onCancel && onCancel()}
            >
              <Icon name="arrow-left" size="sm" className="me-1" />
              Voltar
            </button>
          </div>
        )}
      </div>

      <form onSubmit={handleSubmit}>
        <div className="row">
          {/* Name Field */}
          <div className="col-md-6 mb-3">
            <label htmlFor="name" className="form-label">
              Nome Completo <span className="text-danger">*</span>
            </label>
            <div className={`input-group ${touched.name && errors.name ? 'has-validation' : ''}`}>
              <span className="input-group-text">
                <Icon name="user" size="sm" />
              </span>
              <input
                type="text"
                className={`form-control ${touched.name && errors.name ? 'is-invalid' : touched.name ? 'is-valid' : ''}`}
                id="name"
                name="name"
                value={formData.name}
                onChange={handleInputChange}
                onBlur={handleBlur}
                disabled={isViewMode || loading}
                placeholder="Digite o nome completo"
              />
              {touched.name && errors.name && (
                <div className="invalid-feedback">
                  {errors.name}
                </div>
              )}
            </div>
          </div>

          {/* Phone Field */}
          <div className="col-md-6 mb-3">
            <label htmlFor="phone" className="form-label">
              Telefone <span className="text-danger">*</span>
            </label>
            <div className={`input-group ${touched.phone && errors.phone ? 'has-validation' : ''}`}>
              <span className="input-group-text">
                <Icon name="phone" size="sm" />
              </span>
              <input
                type="tel"
                className={`form-control ${touched.phone && errors.phone ? 'is-invalid' : touched.phone ? 'is-valid' : ''}`}
                id="phone"
                name="phone"
                value={formData.phone}
                onChange={handleInputChange}
                onBlur={handleBlur}
                disabled={isViewMode || loading}
                placeholder="(11) 99999-9999"
              />
              {touched.phone && errors.phone && (
                <div className="invalid-feedback">
                  {errors.phone}
                </div>
              )}
            </div>
          </div>

          {/* Email Field */}
          <div className="col-md-6 mb-3">
            <label htmlFor="email" className="form-label">
              E-mail <span className="text-danger">*</span>
            </label>
            <div className={`input-group ${touched.email && errors.email ? 'has-validation' : ''}`}>
              <span className="input-group-text">
                <Icon name="envelope" size="sm" />
              </span>
              <input
                type="email"
                className={`form-control ${touched.email && errors.email ? 'is-invalid' : touched.email ? 'is-valid' : ''}`}
                id="email"
                name="email"
                value={formData.email}
                onChange={handleInputChange}
                onBlur={handleBlur}
                disabled={isViewMode || loading}
                placeholder="cliente@email.com"
              />
              {touched.email && errors.email && (
                <div className="invalid-feedback">
                  {errors.email}
                </div>
              )}
            </div>
          </div>

          {/* Registration Date (View mode only) */}
          {isViewMode && customer && (
            <div className="col-md-6 mb-3">
              <label className="form-label">Data de Cadastro</label>
              <div className="input-group">
                <span className="input-group-text">
                  <Icon name="calendar" size="sm" />
                </span>
                <input
                  type="text"
                  className="form-control"
                  value={new Intl.DateTimeFormat('pt-BR', {
                    year: 'numeric',
                    month: '2-digit',
                    day: '2-digit',
                    hour: '2-digit',
                    minute: '2-digit'
                  }).format(new Date(customer.registrationDate))}
                  disabled
                />
              </div>
            </div>
          )}

          {/* Address Field */}
          <div className="col-12 mb-3">
            <label htmlFor="address" className="form-label">
              Endereço Completo <span className="text-danger">*</span>
            </label>
            <div className={`input-group ${touched.address && errors.address ? 'has-validation' : ''}`}>
              <span className="input-group-text">
                <Icon name="location-dot" size="sm" />
              </span>
              <textarea
                className={`form-control ${touched.address && errors.address ? 'is-invalid' : touched.address ? 'is-valid' : ''}`}
                id="address"
                name="address"
                rows="3"
                value={formData.address}
                onChange={handleInputChange}
                onBlur={handleBlur}
                disabled={isViewMode || loading}
                placeholder="Rua, número, complemento, bairro, cidade - estado, CEP"
              />
              {touched.address && errors.address && (
                <div className="invalid-feedback">
                  {errors.address}
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Form Actions */}
        {!isViewMode && (
          <div className="d-flex justify-content-end gap-2 pt-3 border-top">
            <button
              type="button"
              className="btn btn-outline-secondary"
              onClick={onCancel}
              disabled={loading}
            >
              <Icon name="times" size="sm" className="me-1" />
              Cancelar
            </button>
            <button
              type="submit"
              className="btn btn-primary"
              disabled={loading || Object.keys(errors).length > 0}
            >
              {loading ? (
                <>
                  <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                  {isEditMode ? 'Atualizando...' : 'Salvando...'}
                </>
              ) : (
                <>
                  <Icon name={isEditMode ? 'save' : 'plus'} size="sm" className="me-1" />
                  {isEditMode ? 'Atualizar' : 'Criar Cliente'}
                </>
              )}
            </button>
          </div>
        )}
      </form>
    </div>
  );
};

export default CustomerForm;
