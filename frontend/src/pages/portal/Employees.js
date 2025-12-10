import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { employeeService } from '../../services/employeeService';
import DataTable from '../../components/common/DataTable';
import './Employees.css';

const Employees = () => {
  const navigate = useNavigate();
  const [employees, setEmployees] = useState([]);
  const [filteredEmployees, setFilteredEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchValue, setSearchValue] = useState('');
  const [includeInactive, setIncludeInactive] = useState(false);
  const [showModal, setShowModal] = useState(false);
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
    loadEmployees();
  }, [includeInactive]);

  useEffect(() => {
    filterEmployees();
  }, [employees, searchValue]);

  const loadEmployees = async () => {
    setLoading(true);
    setErrorMessage('');
    
    try {
      const result = await employeeService.getAllEmployees(includeInactive);
      
      if (result.success && result.data) {
        setEmployees(result.data);
      } else {
        setErrorMessage(result.message || 'Erro ao carregar colaboradores');
      }
    } catch (error) {
      console.error('Error loading employees:', error);
      setErrorMessage('Erro ao carregar colaboradores');
    } finally {
      setLoading(false);
    }
  };

  const filterEmployees = () => {
    if (!searchValue.trim()) {
      setFilteredEmployees(employees);
      return;
    }

    const search = searchValue.toLowerCase();
    const filtered = employees.filter(emp => 
      emp.name?.toLowerCase().includes(search) ||
      emp.email?.toLowerCase().includes(search) ||
      employeeService.getProfileDisplayName(emp.profile).toLowerCase().includes(search)
    );
    setFilteredEmployees(filtered);
  };

  const handleSearchChange = (value) => {
    setSearchValue(value);
  };

  const handleCreateClick = () => {
    setFormData({
      name: '',
      email: '',
      profile: 'Employee'
    });
    setErrors({});
    setSuccessMessage('');
    setErrorMessage('');
    setShowModal(true);
  };

  const handleViewClick = (employee) => {
    navigate(`/portal/employees/${employee.id}`);
  };

  const handleEditClick = (employee) => {
    navigate(`/portal/employees/${employee.id}`);
  };

  const handleDeleteClick = async (employee) => {
    if (!window.confirm(`Tem certeza que deseja desativar ${employee.name}?`)) {
      return;
    }

    try {
      const result = await employeeService.deactivateEmployee(employee.id);
      
      if (result.success) {
        setSuccessMessage(`Colaborador ${employee.name} desativado com sucesso`);
        await loadEmployees();
      } else {
        setErrorMessage(result.message || 'Erro ao desativar colaborador');
      }
    } catch (error) {
      console.error('Error deactivating employee:', error);
      setErrorMessage('Erro ao desativar colaborador');
    }
  };

  const handleReactivateClick = async (employee) => {
    try {
      const result = await employeeService.reactivateEmployee(employee.id);
      
      if (result.success) {
        setSuccessMessage(`Colaborador ${employee.name} reativado com sucesso`);
        await loadEmployees();
      } else {
        setErrorMessage(result.message || 'Erro ao reativar colaborador');
      }
    } catch (error) {
      console.error('Error reactivating employee:', error);
      setErrorMessage('Erro ao reativar colaborador');
    }
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setFormData({ name: '', email: '', profile: 'Employee' });
    setErrors({});
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
    const validation = employeeService.validateEmployeeData(formData, false);
    if (!validation.isValid) {
      setErrors(validation.errors);
      setErrorMessage('Por favor, corrija os erros no formulário');
      return;
    }
    
    setSaving(true);
    
    try {
      const result = await employeeService.createEmployee(formData);
      
      if (result.success) {
        setSuccessMessage(result.message || 'Colaborador criado com sucesso');
        await loadEmployees();
        handleCloseModal();
      } else {
        setErrorMessage(result.message || 'Erro ao criar colaborador');
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
      console.error('Error creating employee:', error);
      setErrorMessage('Erro ao criar colaborador');
    } finally {
      setSaving(false);
    }
  };

  const getStatusBadgeClass = (isActive) => {
    return isActive ? 'badge bg-success' : 'badge bg-secondary';
  };

  const getStatusText = (isActive) => {
    return isActive ? 'Ativo' : 'Inativo';
  };

  const columns = [
    {
      header: 'Nome',
      accessor: 'name',
      width: '30%'
    },
    {
      header: 'E-mail',
      accessor: 'email',
      width: '30%'
    },
    {
      header: 'Perfil',
      accessor: 'profile',
      width: '20%',
      render: (employee) => employeeService.getProfileDisplayName(employee.profile)
    },
    {
      header: 'Status',
      accessor: 'isActive',
      width: '15%',
      render: (employee) => (
        <span className={getStatusBadgeClass(employee.isActive)}>
          {getStatusText(employee.isActive)}
        </span>
      )
    }
  ];

  return (
    <div className="employees-page">
      <div className="employees-header">
        <div>
          <h2>
            <i className="bi bi-people me-2"></i>
            Colaboradores
          </h2>
          <p className="text-muted">Gerencie os colaboradores da sua empresa</p>
        </div>
        <div>
          <button className="btn btn-primary" onClick={handleCreateClick}>
            <i className="bi bi-plus-circle me-2"></i>
            Novo Colaborador
          </button>
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

      <div className="employees-filters mb-3 col-3">
        <div className="form-check">
          <input
            className="form-check-input"
            type="checkbox"
            id="includeInactive"
            checked={includeInactive}
            onChange={(e) => setIncludeInactive(e.target.checked)}
          />
          <label className="form-check-label" htmlFor="includeInactive">
            Incluir colaboradores inativos
          </label>
        </div>
      </div>

      <DataTable
        data={filteredEmployees}
        columns={columns}
        loading={loading}
        searchValue={searchValue}
        onSearchChange={handleSearchChange}
        searchPlaceholder="Buscar por nome, e-mail ou perfil..."
        emptyMessage="Nenhum colaborador encontrado"
        onView={handleViewClick}
        onEdit={handleEditClick}
        onDelete={(employee) => {
          if (employee.isActive) {
            handleDeleteClick(employee);
          } else {
            handleReactivateClick(employee);
          }
        }}
      />

      {/* Create Employee Modal */}
      {showModal && (
        <div className="modal d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog modal-dialog-centered modal-lg">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">
                  <i className="bi bi-person-plus me-2"></i>
                  Novo Colaborador
                </h5>
                <button type="button" className="btn-close" onClick={handleCloseModal}></button>
              </div>
              
              <form onSubmit={handleSubmit}>
                <div className="modal-body">
                  {errorMessage && (
                    <div className="alert alert-danger" role="alert">
                      <i className="bi bi-exclamation-triangle me-2"></i>
                      {errorMessage}
                    </div>
                  )}

                  <div className="mb-3">
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
                      required
                    />
                    {errors.name && <div className="invalid-feedback">{errors.name}</div>}
                  </div>

                  <div className="mb-3">
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
                      required
                    />
                    {errors.email && <div className="invalid-feedback">{errors.email}</div>}
                  </div>

                  <div className="mb-3">
                    <label htmlFor="profile" className="form-label">
                      Perfil *
                    </label>
                    <select
                      className={`form-select ${errors.profile ? 'is-invalid' : ''}`}
                      id="profile"
                      name="profile"
                      value={formData.profile}
                      onChange={handleInputChange}
                      disabled
                      required
                    >
                      {employeeService.getAvailableProfiles().map(prof => (
                        <option key={prof.value} value={prof.value}>
                          {prof.label}
                        </option>
                      ))}
                    </select>
                    {errors.profile && <div className="invalid-feedback">{errors.profile}</div>}
                    <div className="form-text">
                      Novos colaboradores são criados como Funcionário por padrão
                    </div>
                  </div>
                </div>
                
                <div className="modal-footer">
                  <button type="button" className="btn btn-secondary" onClick={handleCloseModal} disabled={saving}>
                    Cancelar
                  </button>
                  <button type="submit" className="btn btn-primary" disabled={saving}>
                    {saving ? (
                      <>
                        <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                        Criando...
                      </>
                    ) : (
                      <>
                        <i className="bi bi-check-circle me-2"></i>
                        Criar Colaborador
                      </>
                    )}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default Employees;
