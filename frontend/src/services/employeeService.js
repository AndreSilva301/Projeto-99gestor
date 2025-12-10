import { httpClient } from './httpClient';

/**
 * Employee Service
 * Handles all employee/coworker-related API calls
 * Requires Admin or SystemAdmin authentication
 */
class EmployeeService {
  constructor(client) {
    this.client = client;
  }

  /**
   * Get all employees for the current company
   * @param {boolean} includeInactive - Include inactive employees
   * @returns {Promise<Object>} List of employees
   */
  async getAllEmployees(includeInactive = false) {
    try {
      const response = await this.client.get(`/coworkers?includeInactive=${includeInactive}`);
      
      return {
        success: true,
        data: response.map(emp => this.mapEmployeeFromApi(emp))
      };
    } catch (error) {
      console.error('Error fetching employees:', error);
      
      if (error.status === 403) {
        return {
          success: false,
          message: 'Acesso negado. Apenas administradores podem visualizar colaboradores.'
        };
      }

      return {
        success: false,
        message: 'Erro ao carregar colaboradores',
        error: error.message
      };
    }
  }

  /**
   * Create a new employee
   * @param {Object} employeeData - Employee data
   * @returns {Promise<Object>} Created employee
   */
  async createEmployee(employeeData) {
    try {
      const createDto = {
        name: employeeData.name,
        email: employeeData.email,
        profileType: this.mapProfileToNumber(employeeData.profile || 'Employee')
      };

      const response = await this.client.post('/coworkers', createDto);

      return {
        success: true,
        data: this.mapEmployeeFromApi(response),
        message: 'Colaborador criado com sucesso'
      };
    } catch (error) {
      console.error('Error creating employee:', error);
      
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
          message: 'Acesso negado. Apenas administradores podem criar colaboradores.'
        };
      }

      return {
        success: false,
        message: 'Erro ao criar colaborador',
        errors: [error.message]
      };
    }
  }

  /**
   * Update employee information
   * @param {number} employeeId - Employee ID
   * @param {Object} employeeData - Updated employee data
   * @returns {Promise<Object>} Updated employee
   */
  async updateEmployee(employeeId, employeeData) {
    try {
      const updateDto = {
        name: employeeData.name,
        email: employeeData.email || null,
        profile: employeeData.profile ? this.mapProfileToNumber(employeeData.profile) : null
      };

      const response = await this.client.put(`/coworkers/${employeeId}`, updateDto);

      return {
        success: true,
        data: this.mapEmployeeFromApi(response),
        message: 'Colaborador atualizado com sucesso'
      };
    } catch (error) {
      console.error('Error updating employee:', error);
      
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
          message: 'Acesso negado. Você não tem permissão para editar este colaborador.'
        };
      }
      
      if (error.status === 404) {
        return {
          success: false,
          message: 'Colaborador não encontrado'
        };
      }

      return {
        success: false,
        message: 'Erro ao atualizar colaborador',
        errors: [error.message]
      };
    }
  }

  /**
   * Deactivate employee (soft delete)
   * @param {number} employeeId - Employee ID
   * @returns {Promise<Object>} Deactivated employee
   */
  async deactivateEmployee(employeeId) {
    try {
      const response = await this.client.delete(`/coworkers/${employeeId}`);

      return {
        success: true,
        data: this.mapEmployeeFromApi(response),
        message: 'Colaborador desativado com sucesso'
      };
    } catch (error) {
      console.error('Error deactivating employee:', error);
      
      if (error.status === 403) {
        return {
          success: false,
          message: 'Acesso negado. Apenas administradores podem desativar colaboradores.'
        };
      }
      
      if (error.status === 404) {
        return {
          success: false,
          message: 'Colaborador não encontrado'
        };
      }

      return {
        success: false,
        message: 'Erro ao desativar colaborador',
        error: error.message
      };
    }
  }

  /**
   * Reactivate employee
   * @param {number} employeeId - Employee ID
   * @returns {Promise<Object>} Reactivated employee
   */
  async reactivateEmployee(employeeId) {
    try {
      const response = await this.client.post(`/coworkers/${employeeId}/reactivate`, {});

      return {
        success: true,
        data: this.mapEmployeeFromApi(response),
        message: 'Colaborador reativado com sucesso'
      };
    } catch (error) {
      console.error('Error reactivating employee:', error);
      
      if (error.status === 403) {
        return {
          success: false,
          message: 'Acesso negado. Apenas administradores podem reativar colaboradores.'
        };
      }
      
      if (error.status === 404) {
        return {
          success: false,
          message: 'Colaborador não encontrado'
        };
      }

      return {
        success: false,
        message: 'Erro ao reativar colaborador',
        error: error.message
      };
    }
  }

  /**
   * Map employee data from API response to frontend format
   * @param {Object} apiData - API response data
   * @returns {Object} Mapped employee data
   */
  mapEmployeeFromApi(apiData) {
    return {
      id: apiData.id,
      name: apiData.name || '',
      email: apiData.email || '',
      profile: this.mapProfileToString(apiData.profile),
      profileValue: apiData.profile,
      createdDate: apiData.createdDate,
      isActive: apiData.isActive !== undefined ? apiData.isActive : apiData.profile !== 4
    };
  }

  /**
   * Map profile number to string
   * @param {number} profileNumber - Profile enum value
   * @returns {string} Profile name
   */
  mapProfileToString(profileNumber) {
    const profiles = {
      1: 'Admin',
      2: 'Employee',
      3: 'SystemAdmin',
      4: 'Inactive'
    };
    return profiles[profileNumber] || 'Unknown';
  }

  /**
   * Map profile string to number
   * @param {string} profileString - Profile name
   * @returns {number} Profile enum value
   */
  mapProfileToNumber(profileString) {
    const profiles = {
      'Admin': 1,
      'Employee': 2,
      'SystemAdmin': 3,
      'Inactive': 4
    };
    return profiles[profileString] || 2; // Default to Employee
  }

  /**
   * Get profile display name in Portuguese
   * @param {string} profile - Profile name
   * @returns {string} Display name
   */
  getProfileDisplayName(profile) {
    const displayNames = {
      'Admin': 'Administrador',
      'Employee': 'Funcionário',
      'SystemAdmin': 'Administrador do Sistema',
      'Inactive': 'Inativo'
    };
    return displayNames[profile] || profile;
  }

  /**
   * Get available profiles for selection
   * @returns {Array} Array of profile options
   */
  getAvailableProfiles() {
    return [
      { value: 'Employee', label: 'Funcionário', number: 2 },
      { value: 'Admin', label: 'Administrador', number: 1 }
    ];
  }

  /**
   * Validate employee data
   * @param {Object} employeeData - Employee data to validate
   * @param {boolean} isUpdate - Is this an update operation
   * @returns {Object} Validation result
   */
  validateEmployeeData(employeeData, isUpdate = false) {
    const errors = {};

    // Validate name
    if (!employeeData.name || !employeeData.name.trim()) {
      errors.name = 'Nome é obrigatório';
    }

    // Validate email
    if (!isUpdate || employeeData.email) {
      if (!employeeData.email || !employeeData.email.trim()) {
        errors.email = 'E-mail é obrigatório';
      } else if (!this.isValidEmail(employeeData.email)) {
        errors.email = 'E-mail inválido';
      }
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

  /**
   * Format date for display
   * @param {string} dateString - Date string
   * @returns {string} Formatted date
   */
  formatDate(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
  }

  /**
   * Check if current user can manage employees
   * @returns {boolean} Can manage
   */
  canManageEmployees() {
    const userData = this.getUserData();
    if (!userData) return false;
    
    // Only Admin and SystemAdmin can manage employees
    // Profile values: Admin = 1, SystemAdmin = 3
    return userData.profile === 1 || userData.profile === 3;
  }

  /**
   * Get user data from storage
   * @returns {Object|null} User data
   */
  getUserData() {
    const userData = localStorage.getItem('auth_user');
    return userData ? JSON.parse(userData) : null;
  }
}

// Create and export singleton instance
const employeeService = new EmployeeService(httpClient);

export { employeeService };
export default employeeService;
