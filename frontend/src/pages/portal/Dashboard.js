import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { mockApiService, formatCurrency } from '../../services/mockApi';
import { Icon } from '../../components/common';
import { useHeader } from '../../contexts/HeaderContext';

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
  const { updateHeader } = useHeader();
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    updateHeader('Painel de controle', 'Bem-vindo ao seu painel de controle CRM');
  }, []);

  useEffect(() => {
    const loadDashboardData = async () => {
      try {
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

  const quickActions = [
    {
      icon: 'person-plus-fill',
      label: 'Adicionar cliente',
      to: '/portal/customers/new'
    },
    {
      icon: 'file-plus-fill',
      label: 'Criar orçamento',
      to: '/portal/quotes/new'
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

      {/* Welcome Message — MOVIDO PARA O TOPO */}
      <div style={{ 
        background: 'white',
        borderRadius: 'var(--portal-border-radius)',
        padding: '24px',
        boxShadow: 'var(--portal-shadow)',
        marginBottom: '32px',
        textAlign: 'center'
      }}>
        <h3 style={{ margin: '0 0 16px 0', color: 'var(--portal-dark)' }}>
          Bem-vindo ao seu painel de controle do CRM! 🎉
        </h3>
        <p style={{ 
          margin: '0',
          color: 'var(--portal-secondary)',
          lineHeight: '1.6'
        }}>
          Este é o seu painel MVP. Use as ações rápidas acima para começar a gerenciar seus clientes e orçamentos. 
          Os recursos do sistema, como agendamentos, avaliações e CRM, estão disponíveis para ajudar você a se organizar e manter sua gestão em dia.
        </p>
        <div style={{ 
          marginTop: '20px',
          padding: '16px',
          background: '#f8f9fa',
          borderRadius: 'var(--portal-border-radius-sm)',
          borderLeft: '4px solid var(--portal-primary)'
        }}>
          <strong style={{ color: 'var(--portal-primary)' }}>Recursos MVP disponíveis:</strong>
          <br />
          ✅ Gestão de Clientes • ✅ Criação de Orçamentos • ✅ Gestão de Funcionários (Somente para Administrator) • ✅ Configuração da Empresa
        </div>
      </div>

      {/* Statistics Cards */}
      <div className="portal-stats-grid">
       <StatsCard
       icon="people-fill"
       label="Total de clientes"
       value={stats.totalCustomers}
       change={{
        text: `+${stats.customersThisMonth} novos clientes`,
        trend:
         stats.customersThisMonth > 0
          ? 'positive'
          : stats.customersThisMonth < 0
          ? 'negative'
          : 'neutral'
          }}
         gradient="portal-gradient-success"
       />
        
        <StatsCard
          icon="file-text-fill"
          label="Orçamentos pendentes"
          value={stats.pendingQuotes}
          change={{
            text: `+${stats.quotesThisMonth} novos orçamentos`,
            trend:
              stats.quotesThisMonth > 0
              ? 'positive'
              : stats.quotesThisMonth < 0
              ? 'negative'
              : 'neutral'
          }}
          gradient="portal-gradient-warning"
        />
        
        <StatsCard
          icon="currency-dollar"
          label="Receita mensal"
          value={formatCurrency(stats.monthlyRevenue)}
          change={{
            text: `${formatCurrency(stats.revenueLastMonth)} mês anterior`,
            trend: 
            stats.revenueThisMonth > stats.revenueLastMonth
              ? 'positive'
              : stats.revenueThisMonth < stats.revenueLastMonth
              ? 'negative'
              : 'neutral'
          }}
          gradient="portal-gradient-info"
        />
        
        <StatsCard
          icon="person-badge-fill"
          label="Funcionários ativos"
          value={stats.activeEmployees}
          change={null}
          gradient="portal-gradient-danger"
        />
      </div>

      {/* Quick Actions */}
      <div className="portal-quick-actions">
        <div className="portal-quick-actions-header">
          <Icon name="lightning-charge-fill" size={24} />
          <h2 className="portal-quick-actions-title">Ações Rápidas</h2>
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

    </div>
  );
};

export default Dashboard;