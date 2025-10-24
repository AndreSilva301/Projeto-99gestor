import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import CompanyForm from '../../../components/portal/CompanyForm';
import Icon from '../../../components/common/Icon';
import companyService from '../../../services/companyService';

const CompanySettings = () => {
  const navigate = useNavigate();
  const [company, setCompany] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [mode, setMode] = useState('view'); // 'view' or 'edit'
  const [error, setError] = useState(null);
  const [successMessage, setSuccessMessage] = useState('');

  // Mock user data - in real app, this would come from auth context
  const currentUser = {
    id: 1,
    profile: 'Admin', // This should come from auth context
    companyId: 1
  };

  useEffect(() => {
    loadCompanyData();
  }, []);

  const loadCompanyData = async () => {
    try {
      setLoading(true);
      setError(null);
      
      const result = await companyService.getCurrentCompany();
      
      if (result.success) {
        setCompany(result.data);
      } else {
        setError(result.message || 'Erro ao carregar dados da empresa');
      }
    } catch (err) {
      console.error('Error loading company data:', err);
      setError('Erro inesperado ao carregar dados da empresa');
    } finally {
      setLoading(false);
    }
  };

  const handleSave = async (formData) => {
    try {
      setSaving(true);
      setError(null);
      setSuccessMessage('');
      
      const result = await companyService.updateCompany(company.id, formData);
      
      if (result.success) {
        setCompany(result.data);
        setMode('view');
        setSuccessMessage('Dados da empresa atualizados com sucesso!');
        
        // Clear success message after 5 seconds
        setTimeout(() => {
          setSuccessMessage('');
        }, 5000);
      } else {
        setError(result.message || 'Erro ao salvar dados da empresa');
      }
    } catch (err) {
      console.error('Error saving company data:', err);
      setError('Erro inesperado ao salvar dados da empresa');
    } finally {
      setSaving(false);
    }
  };

  const handleEdit = () => {
    setMode('edit');
    setError(null);
    setSuccessMessage('');
  };

  const handleCancel = () => {
    if (mode === 'edit') {
      setMode('view');
      setError(null);
      setSuccessMessage('');
    } else {
      navigate('/portal/settings');
    }
  };

  const canEditCompany = () => {
    return currentUser.profile === 'Admin' || currentUser.profile === 'SystemAdmin';
  };

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center" style={{ minHeight: '400px' }}>
        <div className="text-center">
          <div className="spinner-border text-primary mb-3" role="status">
            <span className="visually-hidden">Carregando...</span>
          </div>
          <p className="text-muted">Carregando dados da empresa...</p>
        </div>
      </div>
    );
  }

  if (error && !company) {
    return (
      <div className="container-fluid">
        <div className="d-flex justify-content-between align-items-center mb-4">
          <div>
            <h2 className="mb-1">
              <Icon name="building" className="me-2" />
              Configurações da Empresa
            </h2>
            <p className="text-muted mb-0">
              Gerencie as informações da sua empresa, endereço e detalhes de contato
            </p>
          </div>
          <button
            type="button"
            className="btn btn-outline-primary"
            onClick={() => navigate('/portal/settings')}
          >
            <Icon name="arrow-left" size="sm" className="me-1" />
            Back to Settings
          </button>
        </div>

        <div className="alert alert-danger d-flex align-items-center" role="alert">
          <Icon name="alert-circle" className="me-2" />
          <div>
            <strong>Erro ao carregar dados:</strong> {error}
          </div>
        </div>

        <div className="text-center mt-4">
          <button
            type="button"
            className="btn btn-primary"
            onClick={loadCompanyData}
          >
            <Icon name="refresh-cw" size="sm" className="me-1" />
            Tentar Novamente
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="container-fluid">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="mb-1">
            <Icon name="building" className="me-2" />
            Company Settings
          </h2>
          <p className="text-muted mb-0">
            Manage your company information and details
          </p>
        </div>
      </div>

      {/* Alert Messages */}
      {error && (
        <div className="alert alert-danger d-flex align-items-center mb-4" role="alert">
          <Icon name="alert-circle" className="me-2" />
          <div>
            <strong>Erro:</strong> {error}
          </div>
          <button
            type="button"
            className="btn-close ms-auto"
            onClick={() => setError(null)}
            aria-label="Close"
          ></button>
        </div>
      )}

      {successMessage && (
        <div className="alert alert-success d-flex align-items-center mb-4" role="alert">
          <Icon name="check-circle" className="me-2" />
          <div>
            {successMessage}
          </div>
          <button
            type="button"
            className="btn-close ms-auto"
            onClick={() => setSuccessMessage('')}
            aria-label="Close"
          ></button>
        </div>
      )}

      {/* Permissions Info */}
      {!canEditCompany() && (
        <div className="alert alert-info d-flex align-items-center mb-4" role="alert">
          <Icon name="info" className="me-2" />
          <div>
            <strong>Informação:</strong> Você não tem permissão para editar os dados da empresa. 
            Entre em contato com um administrador se precisar fazer alterações.
          </div>
        </div>
      )}

      {/* Company Form */}
      <div className="card">
        <div className="card-body">
          {company ? (
            <CompanyForm
              company={company}
              mode={mode}
              currentUserProfile={currentUser.profile}
              loading={saving}
              onSubmit={handleSave}
              onCancel={handleCancel}
              onEdit={handleEdit}
            />
          ) : (
            <div className="text-center py-5">
              <Icon name="building" size="lg" className="text-muted mb-3" />
              <h5 className="text-muted">Nenhuma empresa encontrada</h5>
              <p className="text-muted">
                Não foi possível carregar os dados da empresa.
              </p>
              <button
                type="button"
                className="btn btn-primary"
                onClick={loadCompanyData}
              >
                <Icon name="refresh-cw" size="sm" className="me-1" />
                Tentar Novamente
              </button>
            </div>
          )}
        </div>
      </div>

      {/* Additional Information */}
      <div className="row mt-4">
        <div className="col-md-6">
          <div className="card border-0 bg-light">
            <div className="card-body">
              <h6 className="card-title d-flex align-items-center">
                <Icon name="info" className="me-2 text-primary" />
                Informações Importantes
              </h6>
              <ul className="mb-0 small text-muted">
                <li>Apenas administradores podem editar os dados da empresa</li>
                <li>Alterações nos dados da empresa afetam todos os usuários</li>
                <li>Mantenha sempre os dados atualizados para melhor experiência</li>
              </ul>
            </div>
          </div>
        </div>
        
        <div className="col-md-6">
          <div className="card border-0 bg-light">
            <div className="card-body">
              <h6 className="card-title d-flex align-items-center">
                <Icon name="shield-check" className="me-2 text-success" />
                Segurança dos Dados
              </h6>
              <ul className="mb-0 small text-muted">
                <li>Seus dados são protegidos por criptografia</li>
                <li>Acesso restrito conforme perfil do usuário</li>
                <li>Todas as alterações são registradas para auditoria</li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CompanySettings;
