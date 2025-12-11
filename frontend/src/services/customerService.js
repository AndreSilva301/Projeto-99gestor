import { httpClient } from './httpClient';

/**
 * Customer Service
 * Handles all customer-related API calls
 * Requires authentication
 */
class CustomerService {
  constructor(client) {
    this.client = client;
  }

  /**
   * Search customers with pagination and filtering
   * @param {number} page - Page number
   * @param {number} pageSize - Items per page
   * @param {string} searchTerm - Search term
   * @param {string} status - Status filter (not used in API, kept for compatibility)
   * @returns {Promise<Object>} Paginated customer list
   */
  async getCustomers(page = 1, pageSize = 10, searchTerm = '', status = 'all') {
    try {
      const params = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString(),
        orderby: 'Name',
        direction: 'asc'
      });

      if (searchTerm && searchTerm.trim()) {
        params.append('term', searchTerm.trim());
      }

      const response = await this.client.get(`/customer/search?${params.toString()}`);
      
      if (response.success && response.data) {
        const mappedCustomers = response.data.items.map(customer => this.mapCustomerFromApi(customer));
        
        return {
          customers: mappedCustomers,
          pagination: {
            currentPage: response.data.page,
            totalPages: Math.ceil(response.data.totalItems / response.data.pageSize),
            totalItems: response.data.totalItems,
            itemsPerPage: response.data.pageSize,
            hasNext: response.data.page < Math.ceil(response.data.totalItems / response.data.pageSize),
            hasPrev: response.data.page > 1
          }
        };
      }

      return {
        customers: [],
        pagination: {
          currentPage: 1,
          totalPages: 0,
          totalItems: 0,
          itemsPerPage: pageSize,
          hasNext: false,
          hasPrev: false
        }
      };
    } catch (error) {
      console.error('Error fetching customers:', error);
      throw new Error('Erro ao carregar clientes');
    }
  }

  /**
   * Get customer by ID
   * @param {number} id - Customer ID
   * @returns {Promise<Object>} Customer data
   */
  async getCustomerById(id) {
    try {
      const response = await this.client.get(`/customer/${id}`);
      
      if (response.success && response.data) {
        return this.mapCustomerFromApi(response.data);
      }

      throw new Error('Cliente não encontrado');
    } catch (error) {
      console.error('Error fetching customer:', error);
      
      if (error.status === 404) {
        throw new Error('Cliente não encontrado');
      }
      
      if (error.status === 403) {
        throw new Error('Acesso negado');
      }

      throw new Error('Erro ao carregar cliente');
    }
  }

  /**
   * Create new customer
   * @param {Object} customerData - Customer data (must match CustomerCreateDto structure)
   * @returns {Promise<Object>} Created customer
   */
  async createCustomer(customerData) {
    try {
      // customerData should already be formatted as CustomerCreateDto
      // Expected structure:
      // {
      //   name: string,
      //   phone: { mobile: string, landline: string },
      //   address: { street, number, complement, neighborhood, city, state, zipCode },
      //   email: string,
      //   observations: string,
      //   relationships: []
      // }
      const response = await this.client.post('/customer', customerData);

      if (response.success && response.data) {
        return this.mapCustomerFromApi(response.data);
      }

      throw new Error('Erro ao criar cliente');
    } catch (error) {
      console.error('Error creating customer:', error);
      
      if (error.status === 400 && error.data) {
        const errorMessage = error.data.message || 'Dados inválidos';
        const errors = Array.isArray(error.data.errors) ? error.data.errors : [errorMessage];
        throw new Error(errors.join(', '));
      }

      throw new Error('Erro ao criar cliente');
    }
  }

  /**
   * Update customer
   * @param {number} id - Customer ID
   * @param {Object} customerData - Updated customer data (must match CustomerUpdateDto structure)
   * @returns {Promise<Object>} Updated customer
   */
  async updateCustomer(id, customerData) {
    try {
      // customerData should already be formatted as CustomerUpdateDto
      // Expected structure:
      // {
      //   name: string,
      //   phone: { mobile: string, landline: string },
      //   address: { street, number, complement, neighborhood, city, state, zipCode },
      //   email: string,
      //   observations: string
      // }
      const response = await this.client.put(`/customer/${id}`, customerData);

      if (response.success && response.data) {
        return this.mapCustomerFromApi(response.data);
      }

      throw new Error('Erro ao atualizar cliente');
    } catch (error) {
      console.error('Error updating customer:', error);
      
      if (error.status === 400 && error.data) {
        const errorMessage = error.data.message || 'Dados inválidos';
        const errors = Array.isArray(error.data.errors) ? error.data.errors : [errorMessage];
        throw new Error(errors.join(', '));
      }
      
      if (error.status === 404) {
        throw new Error('Cliente não encontrado');
      }
      
      if (error.status === 403) {
        throw new Error('Acesso negado');
      }

      throw new Error('Erro ao atualizar cliente');
    }
  }

  /**
   * Delete customer (soft delete)
   * @param {number} id - Customer ID
   * @returns {Promise<Object>} Success response
   */
  async deleteCustomer(id) {
    try {
      const response = await this.client.delete(`/customer/${id}`);

      if (response.success) {
        return { success: true };
      }

      throw new Error('Erro ao excluir cliente');
    } catch (error) {
      console.error('Error deleting customer:', error);
      
      if (error.status === 404) {
        throw new Error('Cliente não encontrado');
      }
      
      if (error.status === 403) {
        throw new Error('Acesso negado');
      }

      throw new Error('Erro ao excluir cliente');
    }
  }

  /**
   * Get customer relationships
   * @param {number} customerId - Customer ID
   * @returns {Promise<Array>} List of relationships
   */
  async getCustomerRelationships(customerId) {
    try {
      const response = await this.client.get(`/customer/${customerId}/relationships`);
      
      if (response.success && response.data) {
        return response.data.map(rel => this.mapRelationshipFromApi(rel));
      }

      return [];
    } catch (error) {
      console.error('Error fetching relationships:', error);
      
      if (error.status === 404) {
        throw new Error('Cliente não encontrado');
      }

      throw new Error('Erro ao carregar relacionamentos');
    }
  }

  /**
   * Add or update customer relationships
   * @param {number} customerId - Customer ID
   * @param {Array} relationships - Array of relationship objects
   * @returns {Promise<Array>} Created/updated relationships
   */
  async addOrUpdateCustomerRelationships(customerId, relationships) {
    try {
      const relationshipDtos = relationships.map(rel => ({
        id: rel.id || 0,
        description: rel.description,
        dateTime: rel.dateTime || new Date().toISOString()
      }));

      const response = await this.client.post(`/customer/${customerId}/relationships`, relationshipDtos);

      if (response.success && response.data) {
        return response.data.map(rel => this.mapRelationshipFromApi(rel));
      }

      throw new Error('Erro ao salvar relacionamentos');
    } catch (error) {
      console.error('Error saving relationships:', error);
      
      if (error.status === 400 && error.data) {
        const errorMessage = error.data.message || 'Dados inválidos';
        const errors = Array.isArray(error.data.errors) ? error.data.errors : [errorMessage];
        throw new Error(errors.join(', '));
      }
      
      if (error.status === 404) {
        throw new Error('Cliente não encontrado');
      }

      throw new Error('Erro ao salvar relacionamentos');
    }
  }

  /**
   * Delete customer relationships
   * @param {number} customerId - Customer ID
   * @param {Array} relationshipIds - Array of relationship IDs to delete
   * @returns {Promise<Object>} Success response
   */
  async deleteCustomerRelationships(customerId, relationshipIds) {
    try {
      const response = await this.client.delete(`/customer/${customerId}/relationships`, {
        data: relationshipIds
      });

      if (response.success) {
        return { success: true };
      }

      throw new Error('Erro ao excluir relacionamentos');
    } catch (error) {
      console.error('Error deleting relationships:', error);
      
      if (error.status === 400) {
        throw new Error('IDs de relacionamento inválidos');
      }
      
      if (error.status === 404) {
        throw new Error('Cliente ou relacionamento não encontrado');
      }

      throw new Error('Erro ao excluir relacionamentos');
    }
  }

  /**
   * Get customer statistics (mock implementation - endpoint doesn't exist)
   * @returns {Promise<Object>} Statistics
   */
  async getCustomerStats() {
    try {
      const response = await this.client.get('/customer/stats');
      
      if (response.success && response.data) {
        return response.data;
      }

      // Return mock data if endpoint fails
      return {
        total: 0,
        active: 0,
        inactive: 0,
        newThisMonth: 0
      };
    } catch (error) {
      console.error('Error fetching customer stats:', error);
      
      // Return mock data on error
      return {
        total: 0,
        active: 0,
        inactive: 0,
        newThisMonth: 0
      };
    }
  }

  /**
   * Map customer data from API response to frontend format
   * @param {Object} apiData - API response data
   * @returns {Object} Mapped customer data
   */
  mapCustomerFromApi(apiData) {
    return {
      id: apiData.id,
      companyId: apiData.companyId,
      name: apiData.name || '',
      phone: apiData.phone ? {
        mobile: apiData.phone.mobile || '',
        landline: apiData.phone.landline || ''
      } : { mobile: '', landline: '' },
      email: apiData.email || '',
      // Address can be either AddressDto object (from CustomerDto) or shortAddress string (from CustomerListItemDto)
      address: apiData.address ? {
        street: apiData.address.street || '',
        number: apiData.address.number || '',
        complement: apiData.address.complement || '',
        neighborhood: apiData.address.neighborhood || '',
        city: apiData.address.city || '',
        state: apiData.address.state || '',
        zipCode: apiData.address.zipCode || ''
      } : (apiData.shortAddress || ''), // Use shortAddress string if address object doesn't exist
      observations: apiData.observations || '',
      registrationDate: apiData.createdDate || apiData.registrationDate,
      status: apiData.isDeleted ? 'inactive' : 'active',
      relationships: apiData.relationships ? apiData.relationships.map(rel => this.mapRelationshipFromApi(rel)) : []
    };
  }

  /**
   * Map relationship data from API response to frontend format
   * @param {Object} apiData - API response data
   * @returns {Object} Mapped relationship data
   */
  mapRelationshipFromApi(apiData) {
    return {
      id: apiData.id,
      customerId: apiData.customerId,
      description: apiData.description || '',
      registrationDate: apiData.dateTime || apiData.registrationDate
    };
  }

  /**
   * Validate customer data
   * @param {Object} customerData - Customer data to validate
   * @returns {Object} Validation result
   */
  validateCustomerData(customerData) {
    const errors = {};

    if (!customerData.name || !customerData.name.trim()) {
      errors.name = 'Nome é obrigatório';
    }

    // Phone validation - at least one is required
    const hasMobile = customerData.phone?.mobile && customerData.phone.mobile.trim();
    const hasLandline = customerData.phone?.landline && customerData.phone.landline.trim();
    
    if (!hasMobile && !hasLandline) {
      errors.phone = 'Pelo menos um telefone é obrigatório';
    }

    if (customerData.email && !this.isValidEmail(customerData.email)) {
      errors.email = 'E-mail inválido';
    }

    // Address validation
    if (!customerData.address?.street || !customerData.address.street.trim()) {
      errors.address = 'Logradouro é obrigatório';
    }
    
    if (!customerData.address?.city || !customerData.address.city.trim()) {
      errors.addressCity = 'Cidade é obrigatória';
    }

    return {
      isValid: Object.keys(errors).length === 0,
      errors
    };
  }

  /**
   * Validate email format
   * @param {string} email - Email to validate
   * @returns {boolean} Is valid
   */
  isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }
}

// Create and export singleton instance
const customerServiceInstance = new CustomerService(httpClient);

export const customerService = customerServiceInstance;

// Utility functions
export const formatPhone = (phone) => {
  if (!phone) return '';
  
  // Handle phone as object (PhoneDto structure)
  if (typeof phone === 'object') {
    const mobile = phone.mobile?.trim();
    const landline = phone.landline?.trim();
    
    // Prefer mobile, fallback to landline
    const phoneNumber = mobile || landline;
    if (!phoneNumber) return '';
    
    // Remove all non-numeric characters
    const numbers = phoneNumber.replace(/\D/g, '');
    
    // Format as (XX) XXXXX-XXXX or (XX) XXXX-XXXX
    if (numbers.length === 11) {
      return `(${numbers.slice(0, 2)}) ${numbers.slice(2, 7)}-${numbers.slice(7)}`;
    } else if (numbers.length === 10) {
      return `(${numbers.slice(0, 2)}) ${numbers.slice(2, 6)}-${numbers.slice(6)}`;
    } else if (numbers.length === 9) {
      // Mobile without area code: 99999-9999
      return `${numbers.slice(0, 5)}-${numbers.slice(5)}`;
    } else if (numbers.length === 8) {
      // Landline without area code: 9999-9999
      return `${numbers.slice(0, 4)}-${numbers.slice(4)}`;
    }
    
    return phoneNumber;
  }
  
  // Handle phone as string (legacy/backward compatibility)
  if (typeof phone === 'string') {
    const numbers = phone.replace(/\D/g, '');
    
    if (numbers.length === 11) {
      return `(${numbers.slice(0, 2)}) ${numbers.slice(2, 7)}-${numbers.slice(7)}`;
    } else if (numbers.length === 10) {
      return `(${numbers.slice(0, 2)}) ${numbers.slice(2, 6)}-${numbers.slice(6)}`;
    } else if (numbers.length === 9) {
      return `${numbers.slice(0, 5)}-${numbers.slice(5)}`;
    } else if (numbers.length === 8) {
      return `${numbers.slice(0, 4)}-${numbers.slice(4)}`;
    }
    
    return phone;
  }
  
  return '';
};

export const formatAddress = (address) => {
  if (!address) return '';
  
  // Handle address as object (AddressDto structure)
  if (typeof address === 'object') {
    const parts = [];
    
    // Street and number
    if (address.street) {
      let streetPart = address.street;
      if (address.number) {
        streetPart += `, ${address.number}`;
      }
      parts.push(streetPart);
    }
    
    // Complement (if exists)
    if (address.complement) {
      parts.push(address.complement);
    }
    
    // Neighborhood
    if (address.neighborhood) {
      parts.push(address.neighborhood);
    }
    
    // City and State
    if (address.city && address.state) {
      parts.push(`${address.city} - ${address.state}`);
    } else if (address.city) {
      parts.push(address.city);
    }
    
    // ZipCode
    if (address.zipCode) {
      const zipCode = address.zipCode.replace(/\D/g, '');
      if (zipCode.length === 8) {
        parts.push(`CEP: ${zipCode.slice(0, 5)}-${zipCode.slice(5)}`);
      } else {
        parts.push(`CEP: ${address.zipCode}`);
      }
    }
    
    return parts.length > 0 ? parts.join(', ') : '';
  }
  
  // Handle address as string (legacy/backward compatibility)
  if (typeof address === 'string') {
    return address;
  }
  
  return '';
};

export const formatDate = (dateString) => {
  if (!dateString) return '';
  
  return new Intl.DateTimeFormat('pt-BR', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(dateString));
};

export const getStatusLabel = (status) => {
  const labels = {
    active: 'Ativo',
    inactive: 'Inativo'
  };
  return labels[status] || status;
};

export const getStatusColor = (status) => {
  const colors = {
    active: 'success',
    inactive: 'secondary'
  };
  return colors[status] || 'secondary';
};
