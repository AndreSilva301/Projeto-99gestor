import { httpClient } from './httpClient';

/**
 * Quote Service
 * Handles all quote-related API calls
 * Requires authentication
 */
class QuoteService {
  constructor(client) {
    this.client = client;
  }

  /**
   * Search quotes with pagination and filtering
   * @param {number} page - Page number
   * @param {number} pageSize - Items per page
   * @param {string} searchTerm - Search term
   * @param {string} sortBy - Sort field
   * @param {boolean} sortDescending - Sort direction
   * @returns {Promise<Object>} Paginated quote list
   */
  async getQuotes(page = 1, pageSize = 10, searchTerm = '', sortBy = 'CreatedAt', sortDescending = true) {
    try {
      const payload = {
        searchTerm: searchTerm && searchTerm.trim() ? searchTerm.trim() : null,
        page: page,
        pageSize: pageSize,
        sortBy: sortBy,
        sortDescending: sortDescending
      };

      const response = await this.client.post('/quote/search', payload);
      
      if (response.success && response.data) {
        const mappedQuotes = response.data.items.map(quote => this.mapQuoteFromApi(quote));
        
        return {
          quotes: mappedQuotes,
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
        quotes: [],
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
      console.error('Error fetching quotes:', error);
      throw new Error('Erro ao carregar orçamentos');
    }
  }

  /**
   * Get quote by ID
   * @param {number} id - Quote ID
   * @returns {Promise<Object>} Quote data
   */
  async getQuoteById(id) {
    try {
      const response = await this.client.get(`/quote/${id}`);
      
      if (response.success && response.data) {
        return this.mapQuoteFromApi(response.data);
      }

      throw new Error('Orçamento não encontrado');
    } catch (error) {
      console.error('Error fetching quote:', error);
      
      if (error.status === 404) {
        throw new Error('Orçamento não encontrado');
      }
      
      if (error.status === 403) {
        throw new Error('Acesso negado');
      }

      throw new Error('Erro ao carregar orçamento');
    }
  }

  /**
   * Create new quote
   * @param {Object} quoteData - Quote data
   * @returns {Promise<Object>} Created quote
   */
  async createQuote(quoteData) {
    try {
      const response = await this.client.post('/quote', quoteData);

      if (response.success && response.data) {
        return this.mapQuoteFromApi(response.data);
      }

      throw new Error('Erro ao criar orçamento');
    } catch (error) {
      console.error('Error creating quote:', error);
      
      if (error.status === 400 && error.data) {
        const errorMessage = error.data.message || 'Dados inválidos';
        const errors = Array.isArray(error.data.errors) ? error.data.errors : [errorMessage];
        throw new Error(errors.join(', '));
      }

      throw new Error('Erro ao criar orçamento');
    }
  }

  /**
   * Update quote
   * @param {number} id - Quote ID
   * @param {Object} quoteData - Updated quote data
   * @returns {Promise<Object>} Updated quote
   */
  async updateQuote(id, quoteData) {
    try {
      const response = await this.client.put(`/quote/${id}`, quoteData);

      if (response.success && response.data) {
        return this.mapQuoteFromApi(response.data);
      }

      throw new Error('Erro ao atualizar orçamento');
    } catch (error) {
      console.error('Error updating quote:', error);
      
      if (error.status === 400 && error.data) {
        const errorMessage = error.data.message || 'Dados inválidos';
        const errors = Array.isArray(error.data.errors) ? error.data.errors : [errorMessage];
        throw new Error(errors.join(', '));
      }
      
      if (error.status === 404) {
        throw new Error('Orçamento não encontrado');
      }
      
      if (error.status === 403) {
        throw new Error('Acesso negado');
      }

      throw new Error('Erro ao atualizar orçamento');
    }
  }

  /**
   * Delete quote
   * @param {number} id - Quote ID
   * @returns {Promise<Object>} Success response
   */
  async deleteQuote(id) {
    try {
      const response = await this.client.delete(`/quote/${id}`);

      if (response.success || response.status === 204) {
        return { success: true };
      }

      throw new Error('Erro ao excluir orçamento');
    } catch (error) {
      console.error('Error deleting quote:', error);
      
      if (error.status === 404) {
        throw new Error('Orçamento não encontrado');
      }
      
      if (error.status === 403) {
        throw new Error('Acesso negado');
      }

      throw new Error('Erro ao excluir orçamento');
    }
  }

  /**
   * Get quote statistics
   * @returns {Promise<Object>} Statistics
   */
  async getQuoteStats() {
    try {
      const response = await this.client.get('/quote/stats');
      
      if (response.success && response.data) {
        return response.data;
      }

      // Return mock data if endpoint fails
      return {
        total: 0,
        pending: 0,
        approved: 0,
        totalValue: 0
      };
    } catch (error) {
      console.error('Error fetching quote stats:', error);
      
      // Return mock data on error
      return {
        total: 0,
        pending: 0,
        approved: 0,
        totalValue: 0
      };
    }
  }

  /**
   * Map quote data from API response to frontend format
   * @param {Object} apiData - API response data
   * @returns {Object} Mapped quote data
   */
  mapQuoteFromApi(apiData) {
    return {
      id: apiData.id,
      customerId: apiData.customerId,
      customerName: apiData.customerName || '',
      userId: apiData.userId,
      userName: apiData.userName || '',
      totalPrice: apiData.totalPrice || 0,
      paymentMethod: apiData.paymentMethod,
      paymentConditions: apiData.paymentConditions || '',
      cashDiscount: apiData.cashDiscount || 0,
      createdAt: apiData.createdAt,
      updatedAt: apiData.updatedAt,
      customFields: apiData.customFields || {},
      quoteItems: apiData.quoteItems ? apiData.quoteItems.map(item => this.mapQuoteItemFromApi(item)) : []
    };
  }

  /**
   * Map quote item data from API response to frontend format
   * @param {Object} apiData - API response data
   * @returns {Object} Mapped quote item data
   */
  mapQuoteItemFromApi(apiData) {
    return {
      id: apiData.id,
      description: apiData.description || '',
      unitPrice: apiData.unitPrice || 0,
      quantity: apiData.quantity || 0,
      totalPrice: apiData.totalPrice || 0,
      customFields: apiData.customFields || {}
    };
  }
}

// Create and export singleton instance
const quoteServiceInstance = new QuoteService(httpClient);

export const quoteService = quoteServiceInstance;

// Utility functions
export const formatCurrency = (value) => {
  if (!value && value !== 0) return 'R$ 0,00';
  
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value);
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

export const formatPaymentMethod = (method) => {
  const methods = {
    0: 'Dinheiro',
    1: 'Cartão de Crédito',
    2: 'Cartão de Débito',
    3: 'PIX',
    4: 'Transferência',
    5: 'Boleto'
  };
  return methods[method] || 'Não especificado';
};
