import React, { useState, useEffect } from 'react';
import Icon from '../common/Icon';

const CustomerForm = ({ 
  customer = null, 
  onSubmit, 
  onCancel, 
  loading = false,
  mode = 'create', // 'create', 'edit', 'view'
  onAddRelationship,
  onDeleteRelationship,
  onEdit
}) => {
  const [formData, setFormData] = useState({
    name: '',
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
    },
    email: '',
    observations: ''
  });
  
  const [errors, setErrors] = useState({});
  const [touched, setTouched] = useState({});
  const [newRelationship, setNewRelationship] = useState('');
  const [addingRelationship, setAddingRelationship] = useState(false);

  useEffect(() => {
    if (customer && mode !== 'create') {
      setFormData({
        name: customer.name || '',
        address: customer.address || {
          street: '',
          number: '',
          complement: '',
          neighborhood: '',
          city: '',
          state: '',
          zipCode: ''
        },
        phone: customer.phone || {
          mobile: '',
          landline: ''
        },
        email: customer.email || '',
        observations: customer.observations || ''
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

      case 'phone.mobile':
        if (value.trim()) {
          const phoneNumbers = value.replace(/\D/g, '');
          // Mobile: 9 digits (without area code) or 11 digits (with area code)
          if (phoneNumbers.length === 9 || phoneNumbers.length === 11) {
            delete newErrors.phoneMobile;
          } else {
            newErrors.phoneMobile = 'Celular deve ter 9 ou 11 dígitos';
          }
        } else {
          delete newErrors.phoneMobile;
        }
        break;

      case 'phone.landline':
        if (value.trim()) {
          const phoneNumbers = value.replace(/\D/g, '');
          // Landline: 8 digits (without area code) or 10 digits (with area code)
          if (phoneNumbers.length === 8 || phoneNumbers.length === 10) {
            delete newErrors.phoneLandline;
          } else {
            newErrors.phoneLandline = 'Telefone fixo deve ter 8 ou 10 dígitos';
          }
        } else {
          delete newErrors.phoneLandline;
        }
        break;

      case 'email':
        if (value.trim()) {
          const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
          if (!emailRegex.test(value)) {
            newErrors.email = 'E-mail inválido';
          } else {
            delete newErrors.email;
          }
        } else {
          delete newErrors.email;
        }
        break;

      case 'address.street':
        if (!value.trim()) {
          newErrors.addressStreet = 'Logradouro é obrigatório';
        } else {
          delete newErrors.addressStreet;
        }
        break;

      case 'address.number':
        if (!value.trim()) {
          newErrors.addressNumber = 'Número é obrigatório';
        } else {
          delete newErrors.addressNumber;
        }
        break;

      case 'address.neighborhood':
        if (!value.trim()) {
          newErrors.addressNeighborhood = 'Bairro é obrigatório';
        } else {
          delete newErrors.addressNeighborhood;
        }
        break;

      case 'address.city':
        if (!value.trim()) {
          newErrors.addressCity = 'Cidade é obrigatória';
        } else {
          delete newErrors.addressCity;
        }
        break;

      case 'address.state':
        if (!value.trim()) {
          newErrors.addressState = 'Estado é obrigatório';
        } else if (value.length !== 2) {
          newErrors.addressState = 'Estado deve ter 2 letras (ex: SP)';
        } else {
          delete newErrors.addressState;
        }
        break;

      case 'address.zipCode':
        if (!value.trim()) {
          newErrors.addressZipCode = 'CEP é obrigatório';
        } else {
          const zipCodeNumbers = value.replace(/\D/g, '');
          if (zipCodeNumbers.length !== 8) {
            newErrors.addressZipCode = 'CEP deve ter 8 dígitos';
          } else {
            delete newErrors.addressZipCode;
          }
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
    
    // Handle nested object properties
    if (name.includes('.')) {
      const [parent, child] = name.split('.');
      
      // Format specific fields
      let formattedValue = value;
      if (name === 'phone.mobile') {
        const numbers = value.replace(/\D/g, '');
        
        // Format mobile number based on length
        if (numbers.length <= 11) {
          if (numbers.length === 11) {
            // Mobile with area code: (11) 99999-9999
            const areaCode = numbers.slice(0, 2);
            const phoneNumber = numbers.slice(2);
            formattedValue = `(${areaCode}) ${phoneNumber.slice(0, 5)}-${phoneNumber.slice(5)}`;
          } else if (numbers.length === 9) {
            // Mobile without area code: 99999-9999
            formattedValue = `${numbers.slice(0, 5)}-${numbers.slice(5)}`;
          } else {
            formattedValue = numbers;
          }
        } else {
          formattedValue = numbers.slice(0, 11);
        }
      } else if (name === 'phone.landline') {
        const numbers = value.replace(/\D/g, '');
        
        // Format landline number based on length
        if (numbers.length <= 10) {
          if (numbers.length === 10) {
            // Landline with area code: (11) 9999-9999
            const areaCode = numbers.slice(0, 2);
            const phoneNumber = numbers.slice(2);
            formattedValue = `(${areaCode}) ${phoneNumber.slice(0, 4)}-${phoneNumber.slice(4)}`;
          } else if (numbers.length === 8) {
            // Landline without area code: 9999-9999
            formattedValue = `${numbers.slice(0, 4)}-${numbers.slice(4)}`;
          } else {
            formattedValue = numbers;
          }
        } else {
          formattedValue = numbers.slice(0, 10);
        }
      } else if (name === 'address.zipCode') {
        const numbers = value.replace(/\D/g, '').slice(0, 8);
        formattedValue = numbers.replace(/(\d{5})(\d{3})/, '$1-$2');
      } else if (name === 'address.state') {
        formattedValue = value.toUpperCase().slice(0, 2);
      }
      
      setFormData(prev => ({
        ...prev,
        [parent]: {
          ...prev[parent],
          [child]: formattedValue
        }
      }));
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
    
    // Validate all required fields
    const fieldsToValidate = [
      'name',
      'address.street',
      'address.number',
      'address.neighborhood', 
      'address.city',
      'address.state',
      'address.zipCode'
    ];
    
    // Add phone validation only if they have values
    if (formData.phone.mobile.trim()) {
      fieldsToValidate.push('phone.mobile');
    }
    if (formData.phone.landline.trim()) {
      fieldsToValidate.push('phone.landline');
    }
    
    // At least one phone number is required
    if (!formData.phone.mobile.trim() && !formData.phone.landline.trim()) {
      setErrors(prev => ({ ...prev, phoneRequired: 'Pelo menos um telefone é obrigatório' }));
      // Mark phone fields as touched to show validation
      setTouched(prev => ({ ...prev, 'phone.mobile': true, 'phone.landline': true }));
      
      // Scroll to the error
      const phoneSection = document.getElementById('phone.mobile');
      if (phoneSection) {
        phoneSection.scrollIntoView({ behavior: 'smooth', block: 'center' });
      }
      return;
    } else {
      setErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors.phoneRequired;
        return newErrors;
      });
    }
    
    // Add email validation only if it has value
    if (formData.email.trim()) {
      fieldsToValidate.push('email');
    }
    
    let isValid = true;
    let firstErrorField = null;
    
    fieldsToValidate.forEach(field => {
      let value;
      if (field.includes('.')) {
        const [parent, child] = field.split('.');
        value = formData[parent][child];
      } else {
        value = formData[field];
      }
      
      const fieldValid = validateField(field, value);
      if (!fieldValid && !firstErrorField) {
        firstErrorField = field;
        isValid = false;
      } else if (!fieldValid) {
        isValid = false;
      }
    });

    // Mark all fields as touched to show validation errors
    const touchedFields = fieldsToValidate.reduce((acc, field) => ({ ...acc, [field]: true }), {});
    setTouched(touchedFields);

    if (!isValid) {
      // Scroll to first error field
      if (firstErrorField) {
        const errorElement = document.getElementById(firstErrorField);
        if (errorElement) {
          errorElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
          errorElement.focus();
        }
      }
      
      // Show alert with validation errors
      const errorMessages = Object.values(errors).filter(msg => msg).join('\n');
      if (errorMessages) {
        alert('Por favor, corrija os seguintes erros:\n\n' + errorMessages);
      }
      
      return;
    }

    // If all valid, submit the form
    onSubmit(formData);
  };

  const isViewMode = mode === 'view';
  const isEditMode = mode === 'edit';
  const isCreateMode = mode === 'create';

  const handleAddRelationship = async () => {
    if (!newRelationship.trim() || !onAddRelationship) return;

    try {
      setAddingRelationship(true);
      await onAddRelationship(newRelationship.trim());
      setNewRelationship('');
    } catch (error) {
      console.error('Erro ao adicionar relacionamento:', error);
      alert('Erro ao adicionar relacionamento. Tente novamente.');
    } finally {
      setAddingRelationship(false);
    }
  };

  const handleDeleteRelationship = async (relationshipId) => {
    if (!window.confirm('Tem certeza que deseja excluir este relacionamento?') || !onDeleteRelationship) {
      return;
    }

    try {
      await onDeleteRelationship(relationshipId);
    } catch (error) {
      console.error('Erro ao excluir relacionamento:', error);
      alert('Erro ao excluir relacionamento. Tente novamente.');
    }
  };

  const formatDate = (dateString) => {
    return new Intl.DateTimeFormat('pt-BR', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    }).format(new Date(dateString));
  };

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

          {/* Email Field */}
          <div className="col-md-6 mb-3">
            <label htmlFor="email" className="form-label">
              E-mail
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

          {/* Phone Fields */}
          <div className="col-md-3 mb-3">
            <label htmlFor="phone.mobile" className="form-label">
              Celular
            </label>
            <div className={`input-group ${touched['phone.mobile'] && errors.phoneMobile ? 'has-validation' : ''}`}>
              <span className="input-group-text">
                <Icon name="mobile-alt" size="sm" />
              </span>
              <input
                type="tel"
                className={`form-control ${touched['phone.mobile'] && errors.phoneMobile ? 'is-invalid' : touched['phone.mobile'] ? 'is-valid' : ''}`}
                id="phone.mobile"
                name="phone.mobile"
                value={formData.phone.mobile}
                onChange={handleInputChange}
                onBlur={handleBlur}
                disabled={isViewMode || loading}
                placeholder="(11) 99999-9999 ou 99999-9999"
              />
              {touched['phone.mobile'] && errors.phoneMobile && (
                <div className="invalid-feedback">
                  {errors.phoneMobile}
                </div>
              )}
            </div>
            <div className="form-text">
              9 ou 11 dígitos (com/sem DDD)
            </div>
          </div>

          <div className="col-md-3 mb-3">
            <label htmlFor="phone.landline" className="form-label">
              Telefone Fixo
            </label>
            <div className={`input-group ${touched['phone.landline'] && errors.phoneLandline ? 'has-validation' : ''}`}>
              <span className="input-group-text">
                <Icon name="phone" size="sm" />
              </span>
              <input
                type="tel"
                className={`form-control ${touched['phone.landline'] && errors.phoneLandline ? 'is-invalid' : touched['phone.landline'] ? 'is-valid' : ''}`}
                id="phone.landline"
                name="phone.landline"
                value={formData.phone.landline}
                onChange={handleInputChange}
                onBlur={handleBlur}
                disabled={isViewMode || loading}
                placeholder="(11) 9999-9999 ou 9999-9999"
              />
              {touched['phone.landline'] && errors.phoneLandline && (
                <div className="invalid-feedback">
                  {errors.phoneLandline}
                </div>
              )}
            </div>
            <div className="form-text">
              8 ou 10 dígitos (com/sem DDD)
            </div>
          </div>

          {/* Phone Required Error */}
          {errors.phoneRequired && (
            <div className="col-12 mb-3">
              <div className="alert alert-danger" role="alert">
                <Icon name="exclamation-triangle" size="sm" className="me-2" />
                {errors.phoneRequired}
              </div>
            </div>
          )}

          {/* Registration Date (View mode only) */}
          {isViewMode && customer && customer.registrationDate && (
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

          {/* Address Fields */}
          <div className="col-12 mb-3">
            <h6 className="text-muted mb-3">
              <Icon name="location-dot" size="sm" className="me-2" />
              Endereço
            </h6>
          </div>

          <div className="col-md-6 mb-3">
            <label htmlFor="address.street" className="form-label">
              Logradouro <span className="text-danger">*</span>
            </label>
            <input
              type="text"
              className={`form-control ${touched['address.street'] && errors.addressStreet ? 'is-invalid' : touched['address.street'] ? 'is-valid' : ''}`}
              id="address.street"
              name="address.street"
              value={formData.address.street}
              onChange={handleInputChange}
              onBlur={handleBlur}
              disabled={isViewMode || loading}
              placeholder="Rua, Avenida, etc."
            />
            {touched['address.street'] && errors.addressStreet && (
              <div className="invalid-feedback">
                {errors.addressStreet}
              </div>
            )}
          </div>

          <div className="col-md-3 mb-3">
            <label htmlFor="address.number" className="form-label">
              Número <span className="text-danger">*</span>
            </label>
            <input
              type="text"
              className={`form-control ${touched['address.number'] && errors.addressNumber ? 'is-invalid' : touched['address.number'] ? 'is-valid' : ''}`}
              id="address.number"
              name="address.number"
              value={formData.address.number}
              onChange={handleInputChange}
              onBlur={handleBlur}
              disabled={isViewMode || loading}
              placeholder="123"
            />
            {touched['address.number'] && errors.addressNumber && (
              <div className="invalid-feedback">
                {errors.addressNumber}
              </div>
            )}
          </div>

          <div className="col-md-3 mb-3">
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
              onBlur={handleBlur}
              disabled={isViewMode || loading}
              placeholder="Apto, Bloco, etc."
            />
          </div>

          <div className="col-md-4 mb-3">
            <label htmlFor="address.neighborhood" className="form-label">
              Bairro <span className="text-danger">*</span>
            </label>
            <input
              type="text"
              className={`form-control ${touched['address.neighborhood'] && errors.addressNeighborhood ? 'is-invalid' : touched['address.neighborhood'] ? 'is-valid' : ''}`}
              id="address.neighborhood"
              name="address.neighborhood"
              value={formData.address.neighborhood}
              onChange={handleInputChange}
              onBlur={handleBlur}
              disabled={isViewMode || loading}
              placeholder="Centro, Vila, etc."
            />
            {touched['address.neighborhood'] && errors.addressNeighborhood && (
              <div className="invalid-feedback">
                {errors.addressNeighborhood}
              </div>
            )}
          </div>

          <div className="col-md-4 mb-3">
            <label htmlFor="address.city" className="form-label">
              Cidade <span className="text-danger">*</span>
            </label>
            <input
              type="text"
              className={`form-control ${touched['address.city'] && errors.addressCity ? 'is-invalid' : touched['address.city'] ? 'is-valid' : ''}`}
              id="address.city"
              name="address.city"
              value={formData.address.city}
              onChange={handleInputChange}
              onBlur={handleBlur}
              disabled={isViewMode || loading}
              placeholder="São Paulo"
            />
            {touched['address.city'] && errors.addressCity && (
              <div className="invalid-feedback">
                {errors.addressCity}
              </div>
            )}
          </div>

          <div className="col-md-2 mb-3">
            <label htmlFor="address.state" className="form-label">
              Estado <span className="text-danger">*</span>
            </label>
            <input
              type="text"
              className={`form-control ${touched['address.state'] && errors.addressState ? 'is-invalid' : touched['address.state'] ? 'is-valid' : ''}`}
              id="address.state"
              name="address.state"
              value={formData.address.state}
              onChange={handleInputChange}
              onBlur={handleBlur}
              disabled={isViewMode || loading}
              placeholder="SP"
              maxLength="2"
            />
            {touched['address.state'] && errors.addressState && (
              <div className="invalid-feedback">
                {errors.addressState}
              </div>
            )}
          </div>

          <div className="col-md-2 mb-3">
            <label htmlFor="address.zipCode" className="form-label">
              CEP <span className="text-danger">*</span>
            </label>
            <input
              type="text"
              className={`form-control ${touched['address.zipCode'] && errors.addressZipCode ? 'is-invalid' : touched['address.zipCode'] ? 'is-valid' : ''}`}
              id="address.zipCode"
              name="address.zipCode"
              value={formData.address.zipCode}
              onChange={handleInputChange}
              onBlur={handleBlur}
              disabled={isViewMode || loading}
              placeholder="00000-000"
            />
            {touched['address.zipCode'] && errors.addressZipCode && (
              <div className="invalid-feedback">
                {errors.addressZipCode}
              </div>
            )}
          </div>

          {/* Observations Field */}
          <div className="col-12 mb-3">
            <label htmlFor="observations" className="form-label">
              Observações
            </label>
            <textarea
              className="form-control"
              id="observations"
              name="observations"
              rows="3"
              value={formData.observations}
              onChange={handleInputChange}
              onBlur={handleBlur}
              disabled={isViewMode || loading}
              placeholder="Informações adicionais sobre o cliente..."
            />
          </div>
        </div>

        {/* Customer Relationships Section - Only show in view/edit mode */}        
        <div className="mt-4">
          <div className="border-top pt-4">
            <h5 className="mb-4">
              <Icon name="comments" className="me-2" />
              Relacionamentos e Observações
            </h5>

            {/* Add New Relationship */}
            <div className="mb-4">
              <div className="row">
                <div className="col-md-8">
                  <div className="form-floating">
                    <textarea
                      className="form-control"
                      id="newRelationship"
                      style={{ height: '80px' }}
                      value={newRelationship}
                      onChange={(e) => setNewRelationship(e.target.value)}
                      placeholder="Digite informações sobre o cliente, preferências, observações..."
                      disabled={loading}
                    />
                    <label htmlFor="newRelationship">Nova Observação</label>
                  </div>
                </div>
                <div className="col-md-4 d-flex align-items-start">
                  <button
                    type="button"
                    className="btn btn-success w-100"
                    onClick={handleAddRelationship}
                    disabled={!newRelationship.trim() || addingRelationship || loading}
                    style={{ height: '80px' }}
                  >
                    {addingRelationship ? (
                      <>
                        <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                        Adicionando...
                      </>
                    ) : (
                      <>
                        <Icon name="plus" className="me-1" />
                        Adicionar
                      </>
                    )}
                  </button>
                </div>
              </div>
            </div>

            {/* Existing Relationships */}
            {customer?.relationships && customer.relationships.length > 0 ? (
              <div className="relationships-list">
                <h6 className="text-muted mb-3">Observações Existentes ({customer.relationships.length})</h6>
                {customer.relationships.map((relationship, index) => (
                  <div key={relationship.id} className="card mb-3">
                    <div className="card-body">
                      <div className="d-flex justify-content-between align-items-start">
                        <div className="flex-grow-1">
                          <p className="mb-2">{relationship.description}</p>
                          <small className="text-muted">
                            <Icon name="clock" size="sm" className="me-1" />
                            Adicionado em {formatDate(relationship.registrationDate)}
                          </small>
                        </div>
                        <button
                          type="button"
                          className="btn btn-outline-danger btn-sm ms-2"
                          onClick={() => handleDeleteRelationship(relationship.id)}
                          title="Excluir observação"
                          disabled={loading}
                        >
                          <Icon name="trash" size="sm" />
                        </button>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <div className="text-center py-5">
                <Icon name="comments" size="3x" className="text-muted mb-3" />
                <h6 className="text-muted">Nenhuma observação cadastrada</h6>
                <p className="text-muted small">
                  Adicione informações sobre preferências, características do cliente ou observações importantes.
                </p>
              </div>
            )}
          </div>
        </div>

        {/* Form Actions */}
        {!isViewMode && (
          <div className="d-flex justify-content-end gap-2 pt-3 border-top mt-4">
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
                  {isEditMode ? 'Atualizar Cliente' : 'Criar Cliente'}
                </>
              )}
            </button>
          </div>
        )}

        {/* View Mode Actions */}
        {isViewMode && (
          <div className="d-flex justify-content-end gap-2 pt-3 border-top mt-4">
            <button
              type="button"
              className="btn btn-outline-secondary"
              onClick={onCancel}
              disabled={loading}
            >
              <Icon name="arrow-left" size="sm" className="me-1" />
              Voltar à Lista
            </button>
            {onEdit && (
              <button
                type="button"
                className="btn btn-primary"
                onClick={onEdit}
                disabled={loading}
              >
                <Icon name="edit" size="sm" className="me-1" />
                Editar Cliente
              </button>
            )}
          </div>
        )}
      </form>
    </div>
  );
};

export default CustomerForm;
