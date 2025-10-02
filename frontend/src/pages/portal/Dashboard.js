import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { mockApiService, formatCurrency } from '../../services/mockApi';
import { Icon } from '../../components/common';

// Stats Card Component
const StatsCard = ({ icon, label, value, change, gradient }) => (
  <div className="portal-stats-card">
    <div className="portal-stats-card-content">
      <div className={`portal-stats-card-icon ${gradient}`}>
        <Icon name={icon} size={24} />
      </div>
      <div className="portal-stats-card-info">
        <div className="portal-stats-card-label">{label}</div>
        <div className="portal-stats-card-value">{value}</div>
        {change && (
          <div className={`portal-stats-card-change ${change.trend}`}>
            <Icon name={change.trend === 'positive' ? 'arrow-up' : change.trend === 'negative' ? 'arrow-down' : 'clock'} size={12} />
            <span>{change.text}</span>
          </div>
        )}
      </div>
    </div>
  </div>
);

// Quick Action Button Component
const QuickActionButton = ({ icon, label, onClick, to }) => {
  const Component = to ? Link : 'button';
  const props = to ? { to } : { onClick };
  
  return (
    <Component 
      className="portal-quick-action-btn"
      {...props}
    >
      <div className="portal-quick-action-icon">
        <Icon name={icon} size={32} />
      </div>
      <div className="portal-quick-action-text">{label}</div>
    </Component>
  );
};

const Dashboard = () => {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);

  // Load dashboard data
  useEffect(() => {
    const loadDashboardData = async () => {
      try {
        // Simulate API call delay
        await new Promise(resolve => setTimeout(resolve, 500));
        
        const dashboardStats = mockApiService.getDashboardStats();
        setStats(dashboardStats);
      } catch (error) {
        console.error('Error loading dashboard data:', error);
      } finally {
        setLoading(false);
      }
    };

    loadDashboardData();
  }, []);

  // Quick actions based on MVP requirements
  const quickActions = [
    {
      icon: 'person-plus-fill',
      label: 'Add Customer',
      to: '/portal/customers/new'
    },
    {
      icon: 'file-plus-fill',
      label: 'Create Quote',
      to: '/portal/quotes/new'
    },
    {
      icon: 'calendar-event',
      label: 'Schedule Service',
      onClick: () => alert('Coming soon in Phase 2!')
    },
    {
      icon: 'gear-fill',
      label: 'Settings',
      to: '/portal/settings'
    }
  ];

  if (loading) {
    return (
      <div style={{ 
        display: 'flex', 
        justifyContent: 'center', 
        alignItems: 'center', 
        height: '400px',
        color: '#6c757d'
      }}>
        Loading dashboard...
      </div>
    );
  }

  return (
    <div className="portal-dashboard">
      {/* Statistics Cards */}
      <div className="portal-stats-grid">
        <StatsCard
          icon="people-fill"
          label="Total Customers"
          value={stats.totalCustomers}
          change={{
            text: `+${stats.customersGrowth.value} ${stats.customersGrowth.period}`,
            trend: stats.customersGrowth.trend
          }}
          gradient="portal-gradient-success"
        />
        
        <StatsCard
          icon="file-text-fill"
          label="Pending Quotes"
          value={stats.pendingQuotes}
          change={{
            text: stats.quotesStatus.text,
            trend: stats.quotesStatus.trend
          }}
          gradient="portal-gradient-warning"
        />
        
        <StatsCard
          icon="currency-dollar"
          label="Monthly Revenue"
          value={formatCurrency(stats.monthlyRevenue)}
          change={{
            text: `+${stats.revenueGrowth.value}% ${stats.revenueGrowth.period}`,
            trend: stats.revenueGrowth.trend
          }}
          gradient="portal-gradient-info"
        />
        
        <StatsCard
          icon="person-badge-fill"
          label="Active Employees"
          value={stats.activeEmployees}
          change={{
            text: stats.employeesStatus.text,
            trend: stats.employeesStatus.trend
          }}
          gradient="portal-gradient-danger"
        />
      </div>

      {/* Quick Actions */}
      <div className="portal-quick-actions">
        <div className="portal-quick-actions-header">
          <Icon name="lightning-charge-fill" size={24} />
          <h2 className="portal-quick-actions-title">Quick Actions</h2>
        </div>
        <div className="portal-quick-actions-grid">
          {quickActions.map((action, index) => (
            <QuickActionButton
              key={index}
              icon={action.icon}
              label={action.label}
              onClick={action.onClick}
              to={action.to}
            />
          ))}
        </div>
      </div>

      {/* Welcome Message */}
      <div style={{ 
        background: 'white',
        borderRadius: 'var(--portal-border-radius)',
        padding: '24px',
        boxShadow: 'var(--portal-shadow)',
        marginTop: '32px',
        textAlign: 'center'
      }}>
        <h3 style={{ margin: '0 0 16px 0', color: 'var(--portal-dark)' }}>
          Welcome to your CRM Dashboard! 🎉
        </h3>
        <p style={{ 
          margin: '0',
          color: 'var(--portal-secondary)',
          lineHeight: '1.6'
        }}>
          This is your MVP dashboard. Use the quick actions above to start managing your customers and quotes. 
          More features like service scheduling, evaluations, and proactive CRM will be available in future phases.
        </p>
        <div style={{ 
          marginTop: '20px',
          padding: '16px',
          background: '#f8f9fa',
          borderRadius: 'var(--portal-border-radius-sm)',
          borderLeft: '4px solid var(--portal-primary)'
        }}>
          <strong style={{ color: 'var(--portal-primary)' }}>MVP Features Available:</strong>
          <br />
          ✅ Customer Management • ✅ Quote Creation • ✅ Employee Management (Admin only) • ✅ Company Settings
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
