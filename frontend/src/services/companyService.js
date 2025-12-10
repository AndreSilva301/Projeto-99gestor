import { httpClient } from './httpClient';

/**
 * Company Service
 * Handles all company-related API calls
 */
class CompanyService {
  constructor(client) {
    this.client = client;
  }

  /**
   * Get current user's company details
   * @returns {Promise<Object>} Company details
   */
  async getCurrentCompany() {
    try {
      // Get current user data to extract companyId
      const userData = this.getUserData();
      
      if (!userData || !userData.companyId) {
        return {
          success: false,
          message: 'Usuário não autenticado ou sem empresa associada'
        };
      }

      return await this.getCompanyById(userData.companyId);
    } catch (error) {
      console.error('Error fetching current company:', error);
      return {
        success: false,
        message: 'Erro ao carregar dados da empresa',
        error: error.message
      };
    }
  }

  /**
   * Get user data from storage
   * @returns {Object|null} User data
   */
  getUserData() {
    const userData = localStorage.getItem('auth_user');
    return userData ? JSON.parse(userData) : null;
  }

  /**
   * Get company by ID
   * @param {number} companyId - Company ID
   * @returns {Promise<Object>} Company details
   */
  async getCompanyById(companyId) {
    try {
      const response = await this.client.get(`/company/${companyId}`);
      
      // The API returns the company data directly, not wrapped in a response object
      return {
        success: true,
        data: this.mapCompanyFromApi(response)
      };
    } catch (error) {
      console.error('Error fetching company by ID:', error);
      
      if (error.status === 404) {
        return {
          success: false,
          message: 'Empresa não encontrada'
        };
      }
      
      if (error.status === 403) {
        return {
          success: false,
          message: 'Acesso negado. Você não tem permissão para visualizar esta empresa.'
        };
      }

      return {
        success: false,
        message: 'Erro ao carregar dados da empresa',
        error: error.message
      };
    }
  }

  /**
   * Map company data from API response to frontend format
   * @param {Object} apiData - API response data
   * @returns {Object} Mapped company data
   */
  mapCompanyFromApi(apiData) {
    return {
      id: apiData.id,
      name: apiData.name,
      cnpj: apiData.cnpj || '',
      address: {
        street: apiData.address?.street || '',
        number: apiData.address?.number || '',
        complement: apiData.address?.complement || '',
        neighborhood: apiData.address?.neighborhood || '',
        city: apiData.address?.city || '',
        state: apiData.address?.state || '',
        zipCode: apiData.address?.zipCode || ''
      },
      phone: {
        mobile: apiData.phone?.mobile || '',
        landline: apiData.phone?.landline || ''
      },
      dateTime: apiData.dateTime
    };
  }

  /**
   * Get all companies (SystemAdmin only)
   * @returns {Promise<Object>} List of companies
   */
  async getAllCompanies() {
    try {
      const response = await this.client.get('/company');
      
      if (response.success && response.data) {
        return {
          success: true,
          data: response.data
        };
      }

      return {
        success: false,
        message: response.message || 'Erro ao carregar empresas'
      };
    } catch (error) {
      console.error('Error fetching all companies:', error);
      
      if (error.status === 403) {
        return {
          success: false,
          message: 'Acesso negado. Apenas SystemAdmin pode listar todas as empresas.'
        };
      }

      return {
        success: false,
        message: 'Erro ao carregar empresas',
        error: error.message
      };
    }
  }

  /**
   * Update company details
   * @param {number} companyId - Company ID
   * @param {Object} companyData - Updated company data
   * @returns {Promise<Object>} Update result
   */
  async updateCompany(companyId, companyData) {
    try {
      // Map frontend data to backend DTO structure
      const updateDto = {
        name: companyData.name,
        cnpj: companyData.cnpj || null,
        address: {
          street: companyData.address.street,
          number: companyData.address.number,
          complement: companyData.address.complement || null,
          neighborhood: companyData.address.neighborhood,
          city: companyData.address.city,
          state: companyData.address.state,
          zipCode: companyData.address.zipCode
        },
        phone: {
          mobile: companyData.phone.mobile || null,
          landline: companyData.phone.landline || null
        }
      };

      const response = await this.client.put(`/company/${companyId}`, updateDto);

      // The API returns the company data directly, not wrapped in a response object
      return {
        success: true,
        data: this.mapCompanyFromApi(response),
        message: 'Empresa atualizada com sucesso'
      };
    } catch (error) {
      console.error('Error updating company:', error);
      
      if (error.status === 400 && error.data) {
        return {
          success: false,
          message: error.data.message || 'Dados inválidos',
          errors: Array.isArray(error.data.errors) ? error.data.errors : [error.data.message || 'Erro de validação']
        };
      }
      
      if (error.status === 403) {
        return {
          success: false,
          message: 'Acesso negado. Você não tem permissão para editar esta empresa.'
        };
      }
      
      if (error.status === 404) {
        return {
          success: false,
          message: 'Empresa não encontrada'
        };
      }

      return {
        success: false,
        message: 'Erro de rede. Verifique sua conexão e tente novamente.',
        errors: [error.message]
      };
    }
  }

  /**
   * Validate company data
   * @param {Object} companyData - Company data to validate
   * @returns {Object} Validation result
   */
  validateCompanyData(companyData) {
    const errors = {};

    // Validate required fields
    if (!companyData.name || !companyData.name.trim()) {
      errors.name = 'Nome da empresa é obrigatório';
    }

    // Validate CNPJ format (basic validation)
    if (companyData.cnpj) {
      const cnpjNumbers = companyData.cnpj.replace(/\D/g, '');
      if (cnpjNumbers.length !== 14) {
        errors.cnpj = 'CNPJ deve ter 14 dígitos';
      }
    }

    // Validate address if provided
    if (companyData.address) {
      if (!companyData.address.street || !companyData.address.street.trim()) {
        errors.addressStreet = 'Logradouro é obrigatório';
      }
      if (!companyData.address.number || !companyData.address.number.trim()) {
        errors.addressNumber = 'Número é obrigatório';
      }
      if (!companyData.address.neighborhood || !companyData.address.neighborhood.trim()) {
        errors.addressNeighborhood = 'Bairro é obrigatório';
      }
      if (!companyData.address.city || !companyData.address.city.trim()) {
        errors.addressCity = 'Cidade é obrigatória';
      }
      if (!companyData.address.state || !companyData.address.state.trim()) {
        errors.addressState = 'Estado é obrigatório';
      } else if (companyData.address.state.length !== 2) {
        errors.addressState = 'Estado deve ter 2 letras (ex: SP)';
      }
      if (!companyData.address.zipCode || !companyData.address.zipCode.trim()) {
        errors.addressZipCode = 'CEP é obrigatório';
      } else {
        const zipCodeNumbers = companyData.address.zipCode.replace(/\D/g, '');
        if (zipCodeNumbers.length !== 8) {
          errors.addressZipCode = 'CEP deve ter 8 dígitos';
        }
      }
    }

    return {
      isValid: Object.keys(errors).length === 0,
      errors
    };
  }

  /**
   * Format CNPJ for display
   * @param {string} cnpj - Raw CNPJ
   * @returns {string} Formatted CNPJ
   */
  formatCNPJ(cnpj) {
    if (!cnpj) return '';
    const numbers = cnpj.replace(/\D/g, '');
    return numbers.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5');
  }
}

// Create and export singleton instance
const companyService = new CompanyService(httpClient);

export { companyService };
export default companyService;
