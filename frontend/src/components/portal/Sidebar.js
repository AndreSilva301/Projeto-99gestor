import React from 'react';
import { NavLink } from 'react-router-dom';
import { mockApiService } from '../../services/mockApi';
import { Icon } from '../common';

const Sidebar = ({ isOpen, onClose, userRole = 'Administrator' }) => {
  const user = mockApiService.getCurrentUser();
  const company = mockApiService.getCompany();
  const counts = mockApiService.getNavigationCounts();

  // Get actual user data from localStorage to check permissions
  const getUserData = () => {
    const userData = localStorage.getItem('auth_user');
    return userData ? JSON.parse(userData) : null;
  };

  const isAdminOrSystemAdmin = () => {
    const userData = getUserData();
    if (!userData) return false;
    // Profile values: Admin = 1, SystemAdmin = 3
    return userData.profile === 1 || userData.profile === 3;
  };

  // Navigation items based on MVP requirements
  const navigationItems = [
    {
      path: '/portal',
      label: 'Indicadores',
      icon: 'speedometer2',
      end: true
    },
    {
      path: '/portal/customers',
      label: 'Clientes',
      icon: 'people',
      badge: counts.customers
    },
    {
      path: '/portal/quotes',
      label: 'Orçamentos',
      icon: 'file-text',
      badge: counts.quotes
    },
    // Only show Employees for Administrators and SystemAdmin (MVP requirement)
    ...(isAdminOrSystemAdmin() ? [{
      path: '/portal/employees',
      label: 'Colaboradores',
      icon: 'person-badge'
    }] : []),
    // Only show Company for Administrators and SystemAdmin (MVP requirement)
    ...(isAdminOrSystemAdmin() ? [{
      path: '/portal/company',
      label: 'Empresa',
      icon: 'building'
    }] : []),
    {
      path: '/portal/settings',
      label: 'Configurações',
      icon: 'gear'
    }
  ];

  const handleNavLinkClick = () => {
    // Close sidebar on mobile when navigation item is clicked
    if (window.innerWidth <= 768) {
      onClose?.();
    }
  };

  return (
    <>
      {/* Mobile overlay */}
      {isOpen && (
        <div 
          className="portal-sidebar-overlay"
          onClick={onClose}
          style={{
            position: 'fixed',
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: 'rgba(0, 0, 0, 0.5)',
            zIndex: 999,
            display: window.innerWidth <= 768 ? 'block' : 'none'
          }}
        />
      )}
      
      <aside className={`portal-sidebar ${isOpen ? 'show' : ''}`}>
        {/* Logo/Brand */}
        <div className="portal-sidebar-header">
          <div className="portal-sidebar-logo">
            <Icon name="clipboard-check" size={24} />
            <span style={{ marginLeft: '8px' }}>{company.name}</span>
          </div>
          <div className="portal-sidebar-subtitle">
            CRM para prestadores de serviço
          </div>
        </div>

        {/* User Info */}
        <div className="portal-sidebar-user">
          <div className="portal-sidebar-user-avatar">
            <Icon name="person-circle" size={24} />
          </div>
          <div className="portal-sidebar-user-name">{user.name}</div>
          <div className="portal-sidebar-user-role">{user.role}</div>
        </div>

        {/* Navigation */}
        <nav className="portal-sidebar-nav">
          {navigationItems.map((item) => (
            <div key={item.path} className="portal-nav-item text-start">
              <NavLink
                to={item.path}
                end={item.end}
                className={({ isActive }) => 
                  `portal-nav-link ${isActive ? 'active' : ''}`
                }
                onClick={handleNavLinkClick}
              >
                <div className="portal-nav-link-icon">
                  <Icon name={item.icon} />
                </div>
                <span className="portal-nav-link-text">{item.label}</span>
                {item.badge && (
                  <span className="portal-nav-link-badge">{item.badge}</span>
                )}
              </NavLink>
            </div>
          ))}
          
          {/* Logout */}
          <div className="portal-nav-item" style={{ marginTop: 'auto', paddingTop: '20px' }}>
            <button
              className="portal-nav-link text-start"
              onClick={() => {
                // TODO: Implement logout logic
                console.log('Logout clicked');
              }}
              style={{ 
                width: '100%', 
                background: 'none', 
                border: 'none',
                cursor: 'pointer'
              }}
            >
              <div className="portal-nav-link-icon">
                <Icon name="box-arrow-right" />
              </div>
              <span className="portal-nav-link-text">Sair</span>
            </button>
          </div>
        </nav>
      </aside>
    </>
  );
};

export default Sidebar;
