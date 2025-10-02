// Mock API service for portal data
export const mockApiService = {
  // User data
  getCurrentUser: () => ({
    id: 1,
    name: 'Welber Reis',
    email: 'welber@example.com',
    role: 'Administrator',
    companyId: 1,
    avatar: null
  }),

  // Company data
  getCompany: () => ({
    id: 1,
    name: 'ManiaDeLimpeza',
    cnpj: '12.345.678/0001-90',
    createdDate: '2024-01-15T10:00:00Z'
  }),

  // Dashboard statistics
  getDashboardStats: () => ({
    totalCustomers: 47,
    customersGrowth: { value: 5, period: 'this month', trend: 'positive' },
    
    pendingQuotes: 12,
    quotesStatus: { text: 'Awaiting approval', trend: 'neutral' },
    
    monthlyRevenue: 8450.00,
    revenueGrowth: { value: 12, period: 'vs last month', trend: 'positive' },
    
    activeEmployees: 3,
    employeesStatus: { text: 'All online', trend: 'positive' }
  }),

  // Recent customers
  getRecentCustomers: () => [
    {
      id: 1,
      name: 'Maria Silva',
      email: 'maria@email.com',
      phone: '(11) 99999-1234',
      registrationDate: '2024-10-01T14:30:00Z',
      status: 'active'
    },
    {
      id: 2,
      name: 'João Santos',
      email: 'joao@email.com',
      phone: '(11) 99999-5678',
      registrationDate: '2024-09-30T16:45:00Z',
      status: 'active'
    },
    {
      id: 3,
      name: 'Ana Costa',
      email: 'ana@email.com',
      phone: '(11) 99999-9012',
      registrationDate: '2024-09-29T09:20:00Z',
      status: 'active'
    }
  ],

  // Recent quotes
  getRecentQuotes: () => [
    {
      id: 1,
      customerId: 1,
      customerName: 'Maria Silva',
      totalValue: 450.00,
      status: 'pending',
      createdDate: '2024-10-02T10:15:00Z',
      items: 3
    },
    {
      id: 2,
      customerId: 2,
      customerName: 'João Santos',
      totalValue: 320.00,
      status: 'approved',
      createdDate: '2024-10-01T15:30:00Z',
      items: 2
    },
    {
      id: 3,
      customerId: 3,
      customerName: 'Ana Costa',
      totalValue: 680.00,
      status: 'pending',
      createdDate: '2024-10-01T11:45:00Z',
      items: 4
    }
  ],

  // Recent activities
  getRecentActivities: () => [
    {
      id: 1,
      type: 'quote_created',
      description: 'New quote created for Maria Silva',
      user: 'Welber Reis',
      timestamp: '2024-10-02T10:15:00Z',
      icon: 'file-text'
    },
    {
      id: 2,
      type: 'customer_added',
      description: 'New customer João Santos added',
      user: 'Welber Reis',
      timestamp: '2024-10-01T16:45:00Z',
      icon: 'user-plus'
    },
    {
      id: 3,
      type: 'quote_approved',
      description: 'Quote #001 approved by Ana Costa',
      user: 'System',
      timestamp: '2024-10-01T14:20:00Z',
      icon: 'check-circle'
    },
    {
      id: 4,
      type: 'employee_added',
      description: 'New employee Carlos Silva added',
      user: 'Welber Reis',
      timestamp: '2024-09-30T09:30:00Z',
      icon: 'user-plus'
    }
  ],

  // Navigation counts
  getNavigationCounts: () => ({
    customers: 47,
    quotes: 12,
    employees: 3,
    pendingApprovals: 5
  })
};

// Format currency helper
export const formatCurrency = (value) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value);
};

// Format date helper
export const formatDate = (dateString) => {
  return new Intl.DateTimeFormat('pt-BR', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(dateString));
};

// Format relative time helper
export const formatRelativeTime = (dateString) => {
  const date = new Date(dateString);
  const now = new Date();
  const diffInHours = Math.floor((now - date) / (1000 * 60 * 60));
  
  if (diffInHours < 1) return 'Just now';
  if (diffInHours < 24) return `${diffInHours}h ago`;
  
  const diffInDays = Math.floor(diffInHours / 24);
  if (diffInDays < 7) return `${diffInDays}d ago`;
  
  return formatDate(dateString);
};
