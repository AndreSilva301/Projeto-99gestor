import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { employeeService } from '../../services/employeeService';
import './EmployeeDetail.css';

const EmployeeDetail = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [employee, setEmployee] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    profile: 'Employee'
  });
  const [errors, setErrors] = useState({});
  const [successMessage, setSuccessMessage] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (id) {
      loadEmployee();
    }
  }, [id]);

  const loadEmployee = async () => {
    setLoading(true);
    setErrorMessage('');

    try {
      // Get all employees and find the one with matching ID
      const result = await employeeService.getAllEmployees(true);
      
      if (result.success && result.data) {
        const foundEmployee = result.data.find(emp => emp.id === parseInt(id));
        
        if (foundEmployee) {
          setEmployee(foundEmployee);
          setFormData({
            name: foundEmployee.name,
            email: foundEmployee.email,
            profile: foundEmployee.profile
          });
        } else {
          setErrorMessage('Colaborador não encontrado');
        }
      } else {
        setErrorMessage(result.message || 'Erro ao carregar colaborador');
      }
    } catch (error) {
      console.error('Error loading employee:', error);
      setErrorMessage('Erro ao carregar colaborador');
    } finally {
      setLoading(false);
    }
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
    
    // Reset form to original employee data
    if (employee) {
      setFormData({
        name: employee.name,
        email: employee.email,
        profile: employee.profile
      });
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    
    // Clear error for this field
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: undefined
      }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    setSuccessMessage('');
    setErrorMessage('');
    
    // Validate
    const validation = employeeService.validateEmployeeData(formData, true);
    if (!validation.isValid) {
      setErrors(validation.errors);
      setErrorMessage('Por favor, corrija os erros no formulário');
      return;
    }
    
    setSaving(true);
    
    try {
      const result = await employeeService.updateEmployee(employee.id, formData);
      
      if (result.success) {
        setEmployee(result.data);
        setFormData({
          name: result.data.name,
          email: result.data.email,
          profile: result.data.profile
        });
        setSuccessMessage('Colaborador atualizado com sucesso!');
        setIsEditing(false);
        setErrors({});
      } else {
        setErrorMessage(result.message || 'Erro ao atualizar colaborador');
        if (result.errors && result.errors.length > 0) {
          const backendErrors = {};
          result.errors.forEach(err => {
            const errorText = err.toLowerCase();
            if (errorText.includes('nome')) backendErrors.name = err;
            else if (errorText.includes('email') || errorText.includes('e-mail')) backendErrors.email = err;
            else if (errorText.includes('perfil')) backendErrors.profile = err;
          });
          setErrors(backendErrors);
        }
      }
    } catch (error) {
      console.error('Error updating employee:', error);
      setErrorMessage('Erro ao atualizar colaborador');
    } finally {
      setSaving(false);
    }
  };

  const handleDeactivate = async () => {
    if (!window.confirm(`Tem certeza que deseja desativar ${employee.name}?`)) {
      return;
    }

    try {
      const result = await employeeService.deactivateEmployee(employee.id);
      
      if (result.success) {
        setSuccessMessage(`Colaborador ${employee.name} desativado com sucesso`);
        await loadEmployee();
      } else {
        setErrorMessage(result.message || 'Erro ao desativar colaborador');
      }
    } catch (error) {
      console.error('Error deactivating employee:', error);
      setErrorMessage('Erro ao desativar colaborador');
    }
  };

  const handleReactivate = async () => {
    try {
      const result = await employeeService.reactivateEmployee(employee.id);
      
      if (result.success) {
        setSuccessMessage(`Colaborador ${employee.name} reativado com sucesso`);
        await loadEmployee();
      } else {
        setErrorMessage(result.message || 'Erro ao reativar colaborador');
      }
    } catch (error) {
      console.error('Error reactivating employee:', error);
      setErrorMessage('Erro ao reativar colaborador');
    }
  };

  const handleBack = () => {
    navigate('/portal/employees');
  };

  const getStatusBadgeClass = (isActive) => {
    return isActive ? 'badge bg-success' : 'badge bg-secondary';
  };

  const getStatusText = (isActive) => {
    return isActive ? 'Ativo' : 'Inativo';
  };

  if (loading) {
    return (
      <div className="employee-detail-page">
        <div className="loading-container">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">Carregando...</span>
          </div>
          <p className="mt-3">Carregando dados do colaborador...</p>
        </div>
      </div>
    );
  }

  if (!employee && errorMessage) {
    return (
      <div className="employee-detail-page">
        <div className="alert alert-danger" role="alert">
          <i className="bi bi-exclamation-triangle me-2"></i>
          {errorMessage}
        </div>
        <button className="btn btn-primary" onClick={handleBack}>
          <i className="bi bi-arrow-left me-2"></i>
          Voltar para Lista
        </button>
      </div>
    );
  }

  if (!employee) {
    return null;
  }

  return (
    <div className="employee-detail-page">
      <div className="employee-detail-header">
        <div className="d-flex align-items-center">
          <button className="btn btn-link text-decoration-none p-0 me-3" onClick={handleBack}>
            <i className="bi bi-arrow-left fs-4"></i>
          </button>
          <div>
            <h2>
              <i className="bi bi-person me-2"></i>
              {employee.name}
            </h2>
            <p className="text-muted mb-0">Detalhes do colaborador</p>
          </div>
        </div>
        <div className="d-flex gap-2">
          {!isEditing && (
            <>
              {employee.isActive ? (
                <button className="btn btn-outline-danger" onClick={handleDeactivate}>
                  <i className="bi bi-x-circle me-2"></i>
                  Desativar
                </button>
              ) : (
                <button className="btn btn-outline-success" onClick={handleReactivate}>
                  <i className="bi bi-check-circle me-2"></i>
                  Reativar
                </button>
              )}
              <button className="btn btn-primary" onClick={handleEdit}>
                <i className="bi bi-pencil me-2"></i>
                Editar
              </button>
            </>
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
        <div className="employee-detail-card">
          <h5 className="section-title">
            <i className="bi bi-info-circle me-2"></i>
            Informações do Colaborador
          </h5>
          
          <div className="row">
            <div className="col-md-6 mb-3">
              <label htmlFor="name" className="form-label">
                Nome *
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
              <label htmlFor="email" className="form-label">
                E-mail *
              </label>
              <input
                type="email"
                className={`form-control ${errors.email ? 'is-invalid' : ''}`}
                id="email"
                name="email"
                value={formData.email}
                onChange={handleInputChange}
                disabled={!isEditing}
              />
              {errors.email && <div className="invalid-feedback">{errors.email}</div>}
              {isEditing && (
                <div className="form-text">
                  Deixe em branco para manter o e-mail atual
                </div>
              )}
            </div>
          </div>

          <div className="row">
            <div className="col-md-6 mb-3">
              <label htmlFor="profile" className="form-label">
                Perfil *
              </label>
              <select
                className={`form-select ${errors.profile ? 'is-invalid' : ''}`}
                id="profile"
                name="profile"
                value={formData.profile}
                onChange={handleInputChange}
                disabled={!isEditing}
                required
              >
                {employeeService.getAvailableProfiles().map(prof => (
                  <option key={prof.value} value={prof.value}>
                    {prof.label}
                  </option>
                ))}
              </select>
              {errors.profile && <div className="invalid-feedback">{errors.profile}</div>}
            </div>

            <div className="col-md-6 mb-3">
              <label className="form-label">Status</label>
              <div>
                <span className={getStatusBadgeClass(employee.isActive)}>
                  {getStatusText(employee.isActive)}
                </span>
              </div>
            </div>
          </div>

          <div className="row">
            <div className="col-md-6 mb-3">
              <label className="form-label">Data de Criação</label>
              <div className="text-muted">
                {employeeService.formatDate(employee.createdDate)}
              </div>
            </div>

            <div className="col-md-6 mb-3">
              <label className="form-label">ID</label>
              <div className="text-muted">
                #{employee.id}
              </div>
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

export default EmployeeDetail;
