import { httpClient } from './httpClient';

/**
 * Dashboard Service
 * Handles all dashboard-related API operations
 */
class DashboardService {
  constructor(client) {
    this.client = client;
  }

  /**
   * Get dashboard statistics
   * @returns {Promise<Object>} Dashboard stats including customers, quotes, revenue, etc.
   */
  async getStats() {
    try {
      const response = await this.client.get('/dashboard/stats');
      
      if (response.success && response.data) {
        return this.mapStatsFromApi(response.data);
      }

      throw new Error('Erro ao carregar estatísticas do dashboard');
    } catch (error) {
      console.error('Error fetching dashboard stats:', error);
      
      if (error.status === 403) {
        throw new Error('Acesso negado');
      }
      
      if (error.status === 401) {
        throw new Error('Sessão expirada. Faça login novamente.');
      }
      
      throw new Error(error.message || 'Erro ao carregar estatísticas do dashboard');
    }
  }

  /**
   * Map dashboard stats from API response to frontend format
   * @param {Object} apiData - API response data
   * @returns {Object} Mapped dashboard stats
   */
  mapStatsFromApi(apiData) {
    return {
      // Customer stats
      totalCustomers: apiData.totalCustomers || 0,
      customersThisMonth: apiData.customersThisMonth || 0,
      customersLastMonth: apiData.customersLastMonth || 0,
      
      // Quote stats
      totalQuotes: apiData.totalQuotes || 0,
      pendingQuotes: apiData.totalQuotes || 0, // Using totalQuotes as pendingQuotes for now
      quotesThisMonth: apiData.quotesThisMonth || 0,
      quotesLastMonth: apiData.quotesLastMonth || 0,
      
      // Revenue stats
      monthlyRevenue: apiData.revenueThisMonth || 0,
      revenueThisMonth: apiData.revenueThisMonth || 0,
      revenueLastMonth: apiData.revenueLastMonth || 0,
      
      // Employee stats
      activeEmployees: apiData.totalEmployees || 0,
      totalEmployees: apiData.totalEmployees || 0
    };
  }
}

// Create and export singleton instance
const dashboardServiceInstance = new DashboardService(httpClient);

export { dashboardServiceInstance as dashboardService };
