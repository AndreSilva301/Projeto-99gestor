import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import CustomerForm from '../../components/portal/CustomerForm';
import { customerService } from '../../services/customerService';

const CustomerView = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [customer, setCustomer] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadCustomer();
  }, [id]);

  const loadCustomer = async () => {
    try {
      const customerData = await customerService.getCustomerById(id);
      
      // Data should already be in the correct format from mapCustomerFromApi
      // No transformation needed
      setCustomer(customerData);
    } catch (error) {
      console.error('Erro ao carregar cliente:', error);
      alert('Cliente não encontrado.');
      navigate('/portal/customers');
    } finally {
      setLoading(false);
    }
  };

  const handleAddRelationship = async (relationshipText) => {
    try {
      const newRelationship = {
        id: 0, // 0 indicates a new relationship
        description: relationshipText,
        dateTime: new Date().toISOString()
      };
      await customerService.addOrUpdateCustomerRelationships(id, [newRelationship]);
      loadCustomer(); // Reload to get updated relationships
    } catch (error) {
      console.error('Erro ao adicionar relacionamento:', error);
      alert('Erro ao adicionar relacionamento. Tente novamente.');
    }
  };

  const handleDeleteRelationship = async (relationshipId) => {
    try {
      await customerService.deleteCustomerRelationships(id, [relationshipId]);
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
      <div className="card">
        <div className="card-body">
          <CustomerForm
            customer={customer}
            mode="view"
            onCancel={handleBack}
            onEdit={handleEdit}
            onAddRelationship={handleAddRelationship}
            onDeleteRelationship={handleDeleteRelationship}
            loading={loading}
          />
        </div>
      </div>
    </div>
  );
};

export default CustomerView;
