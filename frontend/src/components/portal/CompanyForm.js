import React, { useState, useEffect } from 'react';
import Icon from '../common/Icon';

const CompanyForm = ({ 
  company = null, 
  onSubmit, 
  onCancel, 
  loading = false,
  mode = 'view', // 'edit', 'view'
  currentUserProfile = 'Admin',
  onEdit
}) => {
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
  const [touched, setTouched] = useState({});

  useEffect(() => {
    if (company) {
      setFormData({
        name: company.name || '',
        cnpj: company.cnpj || '',
        address: company.address || {
          street: '',
          number: '',
          complement: '',
          neighborhood: '',
          city: '',
          state: '',
          zipCode: ''
        },
        phone: company.phone || {
          mobile: '',
          landline: ''
        }
      });
    }
  }, [company]);

  const canEdit = () => {
    return mode === 'edit' && (currentUserProfile === 'Admin' || currentUserProfile === 'SystemAdmin');
  };

  const validateField = (field, value) => {
    const newErrors = { ...errors };

    switch (field) {
      case 'name':
        if (!value.trim()) {
          newErrors.name = 'Nome da empresa é obrigatório';
        } else {
          delete newErrors.name;
        }
        break;

      case 'cnpj':
        if (value.trim()) {
          const cnpjNumbers = value.replace(/\D/g, '');
          if (cnpjNumbers.length !== 14) {
            newErrors.cnpj = 'CNPJ deve ter 14 dígitos';
          } else {
            delete newErrors.cnpj;
          }
        } else {
          delete newErrors.cnpj;
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
        
        if (numbers.length <= 11) {
          if (numbers.length === 11) {
            const areaCode = numbers.slice(0, 2);
            const phoneNumber = numbers.slice(2);
            formattedValue = `(${areaCode}) ${phoneNumber.slice(0, 5)}-${phoneNumber.slice(5)}`;
          } else if (numbers.length === 9) {
            formattedValue = `${numbers.slice(0, 5)}-${numbers.slice(5)}`;
          } else {
            formattedValue = numbers;
          }
        } else {
          formattedValue = numbers.slice(0, 11);
        }
      } else if (name === 'phone.landline') {
        const numbers = value.replace(/\D/g, '');
        
        if (numbers.length <= 10) {
          if (numbers.length === 10) {
            const areaCode = numbers.slice(0, 2);
            const phoneNumber = numbers.slice(2);
            formattedValue = `(${areaCode}) ${phoneNumber.slice(0, 4)}-${phoneNumber.slice(4)}`;
          } else if (numbers.length === 8) {
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
      let formattedValue = value;
      if (name === 'cnpj') {
        const numbers = value.replace(/\D/g, '').slice(0, 14);
        formattedValue = numbers.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5');
      }
      
      setFormData(prev => ({ ...prev, [name]: formattedValue }));
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
    
    if (!canEdit()) return;
    
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
    
    // Add CNPJ validation only if it has value
    if (formData.cnpj.trim()) {
      fieldsToValidate.push('cnpj');
    }
    
    let isValid = true;
    
    fieldsToValidate.forEach(field => {
      let value;
      if (field.includes('.')) {
        const [parent, child] = field.split('.');
        value = formData[parent][child];
      } else {
        value = formData[field];
      }
      
      const fieldValid = validateField(field, value);
      if (!fieldValid) isValid = false;
    });

    setTouched(fieldsToValidate.reduce((acc, field) => ({ ...acc, [field]: true }), {}));

    if (isValid) {
      onSubmit(formData);
    }
  };

  const isViewMode = mode === 'view';
  const isEditMode = mode === 'edit';

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
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
      case 'edit': return 'Editar Empresa';
      case 'view': return 'Dados da Empresa';
      default: return 'Empresa';
    }
  };

  return (
    <div className="company-form">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h4 className="mb-0">
          <Icon name="building" className="me-2" />
          {getTitle()}
        </h4>
        <div className="btn-group">
          {isViewMode && canEdit() && (
            <button
              type="button"
              className="btn btn-primary"
              onClick={() => onEdit && onEdit()}
            >
              <Icon name="edit" size="sm" className="me-1" />
              Editar
            </button>
          )}
          <button
            type="button"
            className="btn btn-outline-primary"
            onClick={() => onCancel && onCancel()}
          >
            <Icon name="arrow-left" size="sm" className="me-1" />
            {isViewMode ? 'Voltar' : 'Cancelar'}
          </button>
        </div>
      </div>

      <form onSubmit={handleSubmit}>
        <div className="row">
          {/* Company Name */}
          <div className="col-md-8 mb-3">
            <label htmlFor="name" className="form-label">
              Nome da Empresa <span className="text-danger">*</span>
            </label>
            <div className={`input-group ${touched.name && errors.name ? 'has-validation' : ''}`}>
              <span className="input-group-text">
                <Icon name="building" size="sm" />
              </span>
              <input
                type="text"
                className={`form-control ${touched.name && errors.name ? 'is-invalid' : ''}`}
                id="name"
                name="name"
                value={formData.name}
                onChange={handleInputChange}
                onBlur={handleBlur}
                disabled={!canEdit() || loading}
                placeholder="Digite o nome da empresa"
              />
              {touched.name && errors.name && (
                <div className="invalid-feedback">{errors.name}</div>
              )}
            </div>
          </div>

          {/* CNPJ */}
          <div className="col-md-4 mb-3">
            <label htmlFor="cnpj" className="form-label">
              CNPJ
            </label>
            <div className={`input-group ${touched.cnpj && errors.cnpj ? 'has-validation' : ''}`}>
              <span className="input-group-text">
                <Icon name="card-text" size="sm" />
              </span>
              <input
                type="text"
                className={`form-control ${touched.cnpj && errors.cnpj ? 'is-invalid' : ''}`}
                id="cnpj"
                name="cnpj"
                value={formData.cnpj}
                onChange={handleInputChange}
                onBlur={handleBlur}
                disabled={!canEdit() || loading}
                placeholder="00.000.000/0000-00"
              />
              {touched.cnpj && errors.cnpj && (
                <div className="invalid-feedback">{errors.cnpj}</div>
              )}
            </div>
          </div>
        </div>

        {/* Address Section */}
        <div className="mb-4">
          <h5 className="mb-3">
            <Icon name="map-pin" className="me-2" />
            Endereço
          </h5>
          
          <div className="row">
            {/* Street */}
            <div className="col-md-8 mb-3">
              <label htmlFor="address.street" className="form-label">
                Logradouro <span className="text-danger">*</span>
              </label>
              <input
                type="text"
                className={`form-control ${touched['address.street'] && errors.addressStreet ? 'is-invalid' : ''}`}
                name="address.street"
                value={formData.address.street}
                onChange={handleInputChange}
                onBlur={handleBlur}
                disabled={!canEdit() || loading}
                placeholder="Rua, Avenida, etc."
              />
              {touched['address.street'] && errors.addressStreet && (
                <div className="invalid-feedback">{errors.addressStreet}</div>
              )}
            </div>

            {/* Number */}
            <div className="col-md-4 mb-3">
              <label htmlFor="address.number" className="form-label">
                Número <span className="text-danger">*</span>
              </label>
              <input
                type="text"
                className={`form-control ${touched['address.number'] && errors.addressNumber ? 'is-invalid' : ''}`}
                name="address.number"
                value={formData.address.number}
                onChange={handleInputChange}
                onBlur={handleBlur}
                disabled={!canEdit() || loading}
                placeholder="123"
              />
              {touched['address.number'] && errors.addressNumber && (
                <div className="invalid-feedback">{errors.addressNumber}</div>
              )}
            </div>

            {/* Complement */}
            <div className="col-md-6 mb-3">
              <label htmlFor="address.complement" className="form-label">
                Complemento
              </label>
              <input
                type="text"
                className="form-control"
                name="address.complement"
                value={formData.address.complement}
                onChange={handleInputChange}
                disabled={!canEdit() || loading}
                placeholder="Sala, Andar, Bloco, etc."
              />
            </div>

            {/* Neighborhood */}
            <div className="col-md-6 mb-3">
              <label htmlFor="address.neighborhood" className="form-label">
                Bairro <span className="text-danger">*</span>
              </label>
              <input
                type="text"
                className={`form-control ${touched['address.neighborhood'] && errors.addressNeighborhood ? 'is-invalid' : ''}`}
                name="address.neighborhood"
                value={formData.address.neighborhood}
                onChange={handleInputChange}
                onBlur={handleBlur}
                disabled={!canEdit() || loading}
                placeholder="Nome do bairro"
              />
              {touched['address.neighborhood'] && errors.addressNeighborhood && (
                <div className="invalid-feedback">{errors.addressNeighborhood}</div>
              )}
            </div>

            {/* City */}
            <div className="col-md-5 mb-3">
              <label htmlFor="address.city" className="form-label">
                Cidade <span className="text-danger">*</span>
              </label>
              <input
                type="text"
                className={`form-control ${touched['address.city'] && errors.addressCity ? 'is-invalid' : ''}`}
                name="address.city"
                value={formData.address.city}
                onChange={handleInputChange}
                onBlur={handleBlur}
                disabled={!canEdit() || loading}
                placeholder="Nome da cidade"
              />
              {touched['address.city'] && errors.addressCity && (
                <div className="invalid-feedback">{errors.addressCity}</div>
              )}
            </div>

            {/* State */}
            <div className="col-md-3 mb-3">
              <label htmlFor="address.state" className="form-label">
                Estado <span className="text-danger">*</span>
              </label>
              <input
                type="text"
                className={`form-control ${touched['address.state'] && errors.addressState ? 'is-invalid' : ''}`}
                name="address.state"
                value={formData.address.state}
                onChange={handleInputChange}
                onBlur={handleBlur}
                disabled={!canEdit() || loading}
                placeholder="SP"
                maxLength="2"
              />
              {touched['address.state'] && errors.addressState && (
                <div className="invalid-feedback">{errors.addressState}</div>
              )}
            </div>

            {/* ZIP Code */}
            <div className="col-md-4 mb-3">
              <label htmlFor="address.zipCode" className="form-label">
                CEP <span className="text-danger">*</span>
              </label>
              <input
                type="text"
                className={`form-control ${touched['address.zipCode'] && errors.addressZipCode ? 'is-invalid' : ''}`}
                name="address.zipCode"
                value={formData.address.zipCode}
                onChange={handleInputChange}
                onBlur={handleBlur}
                disabled={!canEdit() || loading}
                placeholder="00000-000"
              />
              {touched['address.zipCode'] && errors.addressZipCode && (
                <div className="invalid-feedback">{errors.addressZipCode}</div>
              )}
            </div>
          </div>
        </div>

        {/* Phone Section */}
        <div className="mb-4">
          <h5 className="mb-3">
            <Icon name="phone" className="me-2" />
            Telefones
          </h5>
          
          <div className="row">
            {/* Mobile */}
            <div className="col-md-6 mb-3">
              <label htmlFor="phone.mobile" className="form-label">
                Celular
              </label>
              <div className="input-group">
                <span className="input-group-text">
                  <Icon name="phone" size="sm" />
                </span>
                <input
                  type="text"
                  className="form-control"
                  name="phone.mobile"
                  value={formData.phone.mobile}
                  onChange={handleInputChange}
                  disabled={!canEdit() || loading}
                  placeholder="(11) 99999-9999"
                />
              </div>
            </div>

            {/* Landline */}
            <div className="col-md-6 mb-3">
              <label htmlFor="phone.landline" className="form-label">
                Telefone Fixo
              </label>
              <div className="input-group">
                <span className="input-group-text">
                  <Icon name="phone" size="sm" />
                </span>
                <input
                  type="text"
                  className="form-control"
                  name="phone.landline"
                  value={formData.phone.landline}
                  onChange={handleInputChange}
                  disabled={!canEdit() || loading}
                  placeholder="(11) 3333-4444"
                />
              </div>
            </div>
          </div>
        </div>

        {/* Company Info */}
        {company && (
          <div className="mb-4">
            <h5 className="mb-3">
              <Icon name="info" className="me-2" />
              Informações do Sistema
            </h5>
            <div className="row">
              <div className="col-md-6 mb-3">
                <label className="form-label">ID da Empresa</label>
                <input
                  type="text"
                  className="form-control"
                  value={company.id || 'N/A'}
                  disabled
                />
              </div>
              <div className="col-md-6 mb-3">
                <label className="form-label">Data de Cadastro</label>
                <input
                  type="text"
                  className="form-control"
                  value={formatDate(company.dateTime)}
                  disabled
                />
              </div>
            </div>
          </div>
        )}

        {/* Form Actions */}
        {isEditMode && canEdit() && (
          <div className="d-flex justify-content-end gap-2">
            <button
              type="button"
              className="btn btn-outline-secondary"
              onClick={() => onCancel && onCancel()}
              disabled={loading}
            >
              <Icon name="x" size="sm" className="me-1" />
              Cancelar
            </button>
            <button
              type="submit"
              className="btn btn-primary"
              disabled={loading}
            >
              {loading ? (
                <>
                  <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                  Salvando...
                </>
              ) : (
                <>
                  <Icon name="check" size="sm" className="me-1" />
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

export default CompanyForm;
