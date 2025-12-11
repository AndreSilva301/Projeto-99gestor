import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import CustomerForm from '../../components/portal/CustomerForm';
import { customerService } from '../../services/customerService';

const CustomerEdit = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [customer, setCustomer] = useState(null);
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);

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
      setInitialLoading(false);
    }
  };

  const handleSubmit = async (customerData) => {
    try {
      setLoading(true);
      
      // Transform the form data to match API structure (CustomerUpdateDto)
      const apiData = {
        name: customerData.name,
        phone: {
          mobile: customerData.phone.mobile || null,
          landline: customerData.phone.landline || null
        },
        address: {
          street: customerData.address.street,
          number: customerData.address.number,
          complement: customerData.address.complement || null,
          neighborhood: customerData.address.neighborhood,
          city: customerData.address.city,
          state: customerData.address.state,
          zipCode: customerData.address.zipCode
        },
        email: customerData.email || '',
        observations: customerData.observations || null
      };
      
      await customerService.updateCustomer(id, apiData);
      navigate('/portal/customers', { 
        state: { 
          message: 'Cliente atualizado com sucesso!',
          type: 'success'
        }
      });
    } catch (error) {
      console.error('Erro ao atualizar cliente:', error);
      alert('Erro ao atualizar cliente. Tente novamente.');
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

  const handleCancel = () => {
    navigate('/portal/customers');
  };

  if (initialLoading) {
    return (
      <div className="customer-edit-page">
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
    <div className="customer-edit-page">
      <div className="card">
        <div className="card-body">
          <CustomerForm
            customer={customer}
            mode="edit"
            onSubmit={handleSubmit}
            onCancel={handleCancel}
            onAddRelationship={handleAddRelationship}
            onDeleteRelationship={handleDeleteRelationship}
            loading={loading}
          />
        </div>
      </div>
    </div>
  );
};

export default CustomerEdit;
