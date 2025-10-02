import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import CustomerForm from '../../components/portal/CustomerForm';
import Icon from '../../components/common/Icon';
import { customerService, formatDate } from '../../services/customerService';

const CustomerView = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [customer, setCustomer] = useState(null);
  const [loading, setLoading] = useState(true);
  const [newRelationship, setNewRelationship] = useState('');
  const [addingRelationship, setAddingRelationship] = useState(false);

  useEffect(() => {
    loadCustomer();
  }, [id]);

  const loadCustomer = async () => {
    try {
      const customerData = await customerService.getCustomerById(id);
      setCustomer(customerData);
    } catch (error) {
      console.error('Erro ao carregar cliente:', error);
      alert('Cliente não encontrado.');
      navigate('/portal/customers');
    } finally {
      setLoading(false);
    }
  };

  const handleAddRelationship = async () => {
    if (!newRelationship.trim()) return;

    try {
      setAddingRelationship(true);
      await customerService.addCustomerRelationship(id, newRelationship.trim());
      setNewRelationship('');
      loadCustomer(); // Reload to get updated relationships
    } catch (error) {
      console.error('Erro ao adicionar relacionamento:', error);
      alert('Erro ao adicionar relacionamento. Tente novamente.');
    } finally {
      setAddingRelationship(false);
    }
  };

  const handleDeleteRelationship = async (relationshipId) => {
    if (!window.confirm('Tem certeza que deseja excluir este relacionamento?')) {
      return;
    }

    try {
      await customerService.deleteCustomerRelationship(id, relationshipId);
      loadCustomer(); // Reload to get updated relationships
    } catch (error) {
      console.error('Erro ao excluir relacionamento:', error);
      alert('Erro ao excluir relacionamento. Tente novamente.');
    }
  };

  const handleBack = () => {
    navigate('/portal/customers');
  };

  const handleEdit = () => {
    navigate(`/portal/customers/${id}/edit`);
  };

  if (loading) {
    return (
      <div className="customer-view-page">
        <div className="card">
          <div className="card-body text-center py-5">
            <div className="spinner-border text-primary" role="status">
              <span className="visually-hidden">Carregando...</span>
            </div>
            <p className="mt-3 text-muted">Carregando dados do cliente...</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="customer-view-page">
      {/* Customer Details Card */}
      <div className="card mb-4">
        <div className="card-body">
          <div className="row">
            <div className="col-md-8">
              <CustomerForm
                customer={customer}
                mode="view"
                onCancel={handleBack}
              />
            </div>
            <div className="col-md-4">
              <div className="d-flex flex-column gap-2">
                <button
                  className="btn btn-primary"
                  onClick={handleEdit}
                >
                  <Icon name="edit" size="sm" className="me-1" />
                  Editar Cliente
                </button>
                <button
                  className="btn btn-outline-secondary"
                  onClick={handleBack}
                >
                  <Icon name="arrow-left" size="sm" className="me-1" />
                  Voltar à Lista
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Customer Relationships Card */}
      <div className="card">
        <div className="card-header">
          <h5 className="mb-0">
            <Icon name="comments" className="me-2" />
            Relacionamentos e Observações
          </h5>
        </div>
        <div className="card-body">
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
                  />
                  <label htmlFor="newRelationship">Nova Observação</label>
                </div>
              </div>
              <div className="col-md-4 d-flex align-items-start">
                <button
                  className="btn btn-success w-100"
                  onClick={handleAddRelationship}
                  disabled={!newRelationship.trim() || addingRelationship}
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
                        className="btn btn-outline-danger btn-sm ms-2"
                        onClick={() => handleDeleteRelationship(relationship.id)}
                        title="Excluir observação"
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
    </div>
  );
};

export default CustomerView;
