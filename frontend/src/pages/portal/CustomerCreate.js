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
      
      // Transform the form data to match API structure (CustomerCreateDto)
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
        observations: null,
        relationships: []
      };
      
      await customerService.createCustomer(apiData);
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
