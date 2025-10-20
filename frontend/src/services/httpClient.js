// HTTP Client Service following SOLID principles
class HttpClient {
  constructor(baseURL = '') {
    this.baseURL = baseURL;
    this.defaultHeaders = {
      'Content-Type': 'application/json',
    };
  }

  /**
   * Set authorization token for subsequent requests
   * @param {string} token - Bearer token
   */
  setAuthToken(token) {
    if (token) {
      this.defaultHeaders['Authorization'] = `Bearer ${token}`;
    } else {
      delete this.defaultHeaders['Authorization'];
    }
  }

  /**
   * Build full URL from endpoint
   * @param {string} endpoint - API endpoint
   * @returns {string} Full URL
   */
  buildUrl(endpoint) {
    return `${this.baseURL}${endpoint}`;
  }

  /**
   * Merge headers with defaults
   * @param {Object} headers - Additional headers
   * @returns {Object} Merged headers
   */
  buildHeaders(headers = {}) {
    return { ...this.defaultHeaders, ...headers };
  }

  /**
   * Handle HTTP response
   * @param {Response} response - Fetch response
   * @returns {Promise<Object>} Parsed response
   */
  async handleResponse(response) {
    const contentType = response.headers.get('content-type');
    const isJson = contentType && contentType.includes('application/json');
    
    let data;
    if (isJson) {
      data = await response.json();
    } else {
      data = await response.text();
    }

    if (!response.ok) {
      const error = new Error(data.message || `HTTP Error: ${response.status}`);
      error.status = response.status;
      error.data = data;
      throw error;
    }

    return data;
  }

  /**
   * Make GET request
   * @param {string} endpoint - API endpoint
   * @param {Object} headers - Additional headers
   * @returns {Promise<Object>} Response data
   */
  async get(endpoint, headers = {}) {
    const response = await fetch(this.buildUrl(endpoint), {
      method: 'GET',
      headers: this.buildHeaders(headers),
    });

    return this.handleResponse(response);
  }

  /**
   * Make POST request
   * @param {string} endpoint - API endpoint
   * @param {Object} data - Request body data
   * @param {Object} headers - Additional headers
   * @returns {Promise<Object>} Response data
   */
  async post(endpoint, data = null, headers = {}) {
    const response = await fetch(this.buildUrl(endpoint), {
      method: 'POST',
      headers: this.buildHeaders(headers),
      body: data ? JSON.stringify(data) : null,
    });

    return this.handleResponse(response);
  }

  /**
   * Make PUT request
   * @param {string} endpoint - API endpoint
   * @param {Object} data - Request body data
   * @param {Object} headers - Additional headers
   * @returns {Promise<Object>} Response data
   */
  async put(endpoint, data = null, headers = {}) {
    const response = await fetch(this.buildUrl(endpoint), {
      method: 'PUT',
      headers: this.buildHeaders(headers),
      body: data ? JSON.stringify(data) : null,
    });

    return this.handleResponse(response);
  }

  /**
   * Make DELETE request
   * @param {string} endpoint - API endpoint
   * @param {Object} headers - Additional headers
   * @returns {Promise<Object>} Response data
   */
  async delete(endpoint, headers = {}) {
    const response = await fetch(this.buildUrl(endpoint), {
      method: 'DELETE',
      headers: this.buildHeaders(headers),
    });

    return this.handleResponse(response);
  }
}

// Create and export a configured HTTP client instance
const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'https://localhost:7033/api';
const httpClient = new HttpClient(API_BASE_URL);

export { HttpClient, httpClient };
