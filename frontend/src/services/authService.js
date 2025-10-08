import { httpClient } from './httpClient';

/**
 * Authentication Service
 * Handles all authentication-related API calls following SOLID principles
 */
class AuthService {
  constructor(client) {
    this.client = client;
    this.tokenKey = 'auth_token';
    this.userKey = 'auth_user';
  }

  /**
   * Register a new user
   * @param {Object} registrationData - User registration data
   * @param {string} registrationData.name - User name
   * @param {string} registrationData.phone - User phone
   * @param {string} registrationData.companyName - Company name
   * @param {string} registrationData.email - User email
   * @param {string} registrationData.password - User password
   * @param {string} registrationData.confirmPassword - Password confirmation
   * @param {boolean} registrationData.acceptTerms - Terms acceptance
   * @returns {Promise<Object>} Registration response
   */
  async register(registrationData) {
    try {
      // Map frontend form data to backend DTO structure
      const registerDto = {
        name: registrationData.name,
        phone: registrationData.phone,
        companyName: registrationData.companyName,
        email: registrationData.email,
        password: registrationData.password,
        confirmPassword: registrationData.confirmPassword,
        acceptTerms: registrationData.acceptTerms
      };

      const response = await this.client.post('/auth/register', registerDto);

      // Handle successful registration
      if (response.success && response.data) {
        const { bearerToken, ...userData } = response.data;
        
        // Store authentication data
        this.setAuthToken(bearerToken);
        this.setUserData(userData);

        return {
          success: true,
          user: userData,
          token: bearerToken,
          message: response.message || 'Registration successful'
        };
      }

      return {
        success: false,
        message: response.message || 'Registration failed',
        errors: response.errors || []
      };

    } catch (error) {
      console.error('Registration error:', error);
      
      // Handle specific HTTP errors
      if (error.status === 400 && error.data) {
        return {
          success: false,
          message: error.data.message || 'Registration failed',
          errors: error.data.errors || [error.message]
        };
      }

      return {
        success: false,
        message: 'Network error. Please check your connection and try again.',
        errors: [error.message]
      };
    }
  }

  /**
   * Login user
   * @param {Object} credentials - Login credentials
   * @param {string} credentials.email - User email
   * @param {string} credentials.password - User password
   * @returns {Promise<Object>} Login response
   */
  async login(credentials) {
    try {
      const response = await this.client.post('/auth/login', credentials);

      if (response.success && response.data) {
        const { bearerToken, ...userData } = response.data;
        
        // Store authentication data
        this.setAuthToken(bearerToken);
        this.setUserData(userData);

        return {
          success: true,
          user: userData,
          token: bearerToken,
          message: response.message || 'Login successful'
        };
      }

      return {
        success: false,
        message: response.message || 'Login failed',
        errors: response.errors || []
      };

    } catch (error) {
      console.error('Login error:', error);
      
      if (error.status === 401 && error.data) {
        return {
          success: false,
          message: error.data.message || 'Invalid credentials',
          errors: error.data.errors || ['Invalid email or password']
        };
      }

      return {
        success: false,
        message: 'Network error. Please check your connection and try again.',
        errors: [error.message]
      };
    }
  }

  /**
   * Logout user
   */
  logout() {
    this.clearAuthToken();
    this.clearUserData();
  }

  /**
   * Check if user is authenticated
   * @returns {boolean} Authentication status
   */
  isAuthenticated() {
    const token = this.getAuthToken();
    const user = this.getUserData();
    return !!(token && user);
  }

  /**
   * Get current user data
   * @returns {Object|null} User data
   */
  getCurrentUser() {
    return this.getUserData();
  }

  /**
   * Get authentication token
   * @returns {string|null} Authentication token
   */
  getAuthToken() {
    return localStorage.getItem(this.tokenKey);
  }

  /**
   * Set authentication token
   * @param {string} token - Authentication token
   */
  setAuthToken(token) {
    if (token) {
      localStorage.setItem(this.tokenKey, token);
      this.client.setAuthToken(token);
    }
  }

  /**
   * Clear authentication token
   */
  clearAuthToken() {
    localStorage.removeItem(this.tokenKey);
    this.client.setAuthToken(null);
  }

  /**
   * Get user data from storage
   * @returns {Object|null} User data
   */
  getUserData() {
    const userData = localStorage.getItem(this.userKey);
    return userData ? JSON.parse(userData) : null;
  }

  /**
   * Set user data in storage
   * @param {Object} userData - User data
   */
  setUserData(userData) {
    localStorage.setItem(this.userKey, JSON.stringify(userData));
  }

  /**
   * Clear user data from storage
   */
  clearUserData() {
    localStorage.removeItem(this.userKey);
  }

  /**
   * Initialize authentication state
   * Restores authentication state from storage
   */
  initialize() {
    const token = this.getAuthToken();
    if (token) {
      this.client.setAuthToken(token);
    }
  }
}

// Create and export a configured AuthService instance
const authService = new AuthService(httpClient);

// Initialize the service
authService.initialize();

export { AuthService, authService };
