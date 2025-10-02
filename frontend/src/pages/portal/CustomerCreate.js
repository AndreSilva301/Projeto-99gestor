import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import CustomerForm from '../../components/portal/CustomerForm';
import { customerService } from '../../services/customerService';

const CustomerCreate = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (customerData) => {
    try {
      setLoading(true);
      await customerService.createCustomer(customerData);
      navigate('/portal/customers', { 
        state: { 
          message: 'Cliente criado com sucesso!',
          type: 'success'
        }
      });
    } catch (error) {
      console.error('Erro ao criar cliente:', error);
      alert('Erro ao criar cliente. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = () => {
    navigate('/portal/customers');
  };

  return (
    <div className="customer-create-page">
      <div className="card">
        <div className="card-body">
          <CustomerForm
            mode="create"
            onSubmit={handleSubmit}
            onCancel={handleCancel}
            loading={loading}
          />
        </div>
      </div>
    </div>
  );
};

export default CustomerCreate;
