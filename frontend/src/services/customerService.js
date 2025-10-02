// Customer Service with Mock Data
const mockCustomers = [
  {
    id: 1,
    companyId: 1,
    name: 'Maria Silva Santos',
    phone: '(11) 99999-1234',
    email: 'maria.silva@email.com',
    address: 'Rua das Flores, 123 - Vila Madalena, São Paulo - SP, 05435-000',
    registrationDate: '2024-01-15T10:30:00Z',
    status: 'active',
    relationships: [
      {
        id: 1,
        customerId: 1,
        description: 'Tem 2 filhos pequenos, prefere serviços no período da manhã',
        registrationDate: '2024-01-15T10:35:00Z'
      },
      {
        id: 2,
        customerId: 1,
        description: 'Apartamento de 80m², 3 quartos',
        registrationDate: '2024-01-16T14:20:00Z'
      }
    ]
  },
  {
    id: 2,
    companyId: 1,
    name: 'João Carlos Oliveira',
    phone: '(11) 98888-5678',
    email: 'joao.carlos@email.com',
    address: 'Av. Paulista, 1000 - Bela Vista, São Paulo - SP, 01310-100',
    registrationDate: '2024-01-20T14:15:00Z',
    status: 'active',
    relationships: [
      {
        id: 3,
        customerId: 2,
        description: 'Escritório comercial, limpeza semanal',
        registrationDate: '2024-01-20T14:20:00Z'
      }
    ]
  },
  {
    id: 3,
    companyId: 1,
    name: 'Ana Paula Costa',
    phone: '(11) 97777-9012',
    email: 'ana.costa@email.com',
    address: 'Rua Augusta, 500 - Consolação, São Paulo - SP, 01305-000',
    registrationDate: '2024-02-01T09:45:00Z',
    status: 'inactive',
    relationships: [
      {
        id: 4,
        customerId: 3,
        description: 'Casa grande, tem 3 pets (2 gatos, 1 cachorro)',
        registrationDate: '2024-02-01T09:50:00Z'
      }
    ]
  },
  {
    id: 4,
    companyId: 1,
    name: 'Carlos Eduardo Lima',
    phone: '(11) 96666-3456',
    email: 'carlos.lima@email.com',
    address: 'Rua Oscar Freire, 800 - Jardins, São Paulo - SP, 01426-000',
    registrationDate: '2024-02-10T16:30:00Z',
    status: 'active',
    relationships: []
  },
  {
    id: 5,
    companyId: 1,
    name: 'Fernanda Rodrigues',
    phone: '(11) 95555-7890',
    email: 'fernanda.rodrigues@email.com',
    address: 'Rua Consolação, 1200 - Higienópolis, São Paulo - SP, 01302-001',
    registrationDate: '2024-02-15T11:20:00Z',
    status: 'active',
    relationships: [
      {
        id: 5,
        customerId: 5,
        description: 'Apartamento novo, primeira limpeza detalhada necessária',
        registrationDate: '2024-02-15T11:25:00Z'
      }
    ]
  },
  {
    id: 6,
    companyId: 1,
    name: 'Roberto Santos',
    phone: '(11) 94444-2468',
    email: 'roberto.santos@email.com',
    address: 'Av. Faria Lima, 2000 - Itaim Bibi, São Paulo - SP, 04538-132',
    registrationDate: '2024-03-01T13:10:00Z',
    status: 'active',
    relationships: []
  }
];

let customerIdCounter = 7;

export const customerService = {
  // Get all customers with pagination and filtering
  getCustomers: async (page = 1, limit = 10, search = '', status = 'all') => {
    // Simulate API delay
    await new Promise(resolve => setTimeout(resolve, 300));
    
    let filteredCustomers = [...mockCustomers];
    
    // Apply search filter
    if (search) {
      const searchLower = search.toLowerCase();
      filteredCustomers = filteredCustomers.filter(customer =>
        customer.name.toLowerCase().includes(searchLower) ||
        customer.email.toLowerCase().includes(searchLower) ||
        customer.phone.includes(search) ||
        customer.address.toLowerCase().includes(searchLower)
      );
    }
    
    // Apply status filter
    if (status !== 'all') {
      filteredCustomers = filteredCustomers.filter(customer => customer.status === status);
    }
    
    // Calculate pagination
    const total = filteredCustomers.length;
    const totalPages = Math.ceil(total / limit);
    const start = (page - 1) * limit;
    const end = start + limit;
    const customers = filteredCustomers.slice(start, end);
    
    return {
      customers,
      pagination: {
        currentPage: page,
        totalPages,
        totalItems: total,
        itemsPerPage: limit,
        hasNext: page < totalPages,
        hasPrev: page > 1
      }
    };
  },

  // Get customer by ID
  getCustomerById: async (id) => {
    await new Promise(resolve => setTimeout(resolve, 200));
    
    const customer = mockCustomers.find(c => c.id === parseInt(id));
    if (!customer) {
      throw new Error('Cliente não encontrado');
    }
    
    return customer;
  },

  // Create new customer
  createCustomer: async (customerData) => {
    await new Promise(resolve => setTimeout(resolve, 400));
    
    const newCustomer = {
      id: customerIdCounter++,
      companyId: 1, // Current company
      ...customerData,
      registrationDate: new Date().toISOString(),
      status: 'active',
      relationships: []
    };
    
    mockCustomers.push(newCustomer);
    return newCustomer;
  },

  // Update customer
  updateCustomer: async (id, customerData) => {
    await new Promise(resolve => setTimeout(resolve, 400));
    
    const index = mockCustomers.findIndex(c => c.id === parseInt(id));
    if (index === -1) {
      throw new Error('Cliente não encontrado');
    }
    
    mockCustomers[index] = {
      ...mockCustomers[index],
      ...customerData,
      id: parseInt(id) // Ensure ID doesn't change
    };
    
    return mockCustomers[index];
  },

  // Delete customer
  deleteCustomer: async (id) => {
    await new Promise(resolve => setTimeout(resolve, 300));
    
    const index = mockCustomers.findIndex(c => c.id === parseInt(id));
    if (index === -1) {
      throw new Error('Cliente não encontrado');
    }
    
    mockCustomers.splice(index, 1);
    return { success: true };
  },

  // Add customer relationship
  addCustomerRelationship: async (customerId, description) => {
    await new Promise(resolve => setTimeout(resolve, 300));
    
    const customer = mockCustomers.find(c => c.id === parseInt(customerId));
    if (!customer) {
      throw new Error('Cliente não encontrado');
    }
    
    const newRelationship = {
      id: Math.max(...mockCustomers.flatMap(c => c.relationships.map(r => r.id)), 0) + 1,
      customerId: parseInt(customerId),
      description,
      registrationDate: new Date().toISOString()
    };
    
    customer.relationships.push(newRelationship);
    return newRelationship;
  },

  // Delete customer relationship
  deleteCustomerRelationship: async (customerId, relationshipId) => {
    await new Promise(resolve => setTimeout(resolve, 300));
    
    const customer = mockCustomers.find(c => c.id === parseInt(customerId));
    if (!customer) {
      throw new Error('Cliente não encontrado');
    }
    
    const index = customer.relationships.findIndex(r => r.id === parseInt(relationshipId));
    if (index === -1) {
      throw new Error('Relacionamento não encontrado');
    }
    
    customer.relationships.splice(index, 1);
    return { success: true };
  },

  // Get customer statistics
  getCustomerStats: async () => {
    await new Promise(resolve => setTimeout(resolve, 200));
    
    const total = mockCustomers.length;
    const active = mockCustomers.filter(c => c.status === 'active').length;
    const inactive = mockCustomers.filter(c => c.status === 'inactive').length;
    
    // Calculate new customers this month
    const thisMonth = new Date();
    const firstDayOfMonth = new Date(thisMonth.getFullYear(), thisMonth.getMonth(), 1);
    const newThisMonth = mockCustomers.filter(c => 
      new Date(c.registrationDate) >= firstDayOfMonth
    ).length;
    
    return {
      total,
      active,
      inactive,
      newThisMonth
    };
  }
};

// Utility functions
export const formatPhone = (phone) => {
  if (!phone) return '';
  
  // Remove all non-numeric characters
  const numbers = phone.replace(/\D/g, '');
  
  // Format as (XX) XXXXX-XXXX or (XX) XXXX-XXXX
  if (numbers.length === 11) {
    return `(${numbers.slice(0, 2)}) ${numbers.slice(2, 7)}-${numbers.slice(7)}`;
  } else if (numbers.length === 10) {
    return `(${numbers.slice(0, 2)}) ${numbers.slice(2, 6)}-${numbers.slice(6)}`;
  }
  
  return phone;
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
