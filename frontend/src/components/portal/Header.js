import React from 'react';
import { Icon } from '../common';
import { useHeader } from '../../contexts/HeaderContext';

const Header = ({ onToggleSidebar }) => {
  const { headerData } = useHeader();
  const getCurrentTime = () => {
    return new Intl.DateTimeFormat('en-US', {
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(new Date());
  };

  return (
    <header className="portal-header">
      {/* Mobile menu toggle */}
      <button
        className="portal-header-mobile-toggle"
        onClick={onToggleSidebar}
        aria-label="Toggle sidebar"
      >
        <Icon name="list" size={20} />
      </button>

      {/* Title section */}
      <div className="portal-header-title">
        <h1>{headerData.title}</h1>
        <p>{headerData.subtitle}</p>
      </div>

      {/* Actions section */}
      <div className="portal-header-actions">
        {/* Notifications - Future feature */}
        <button
          className="portal-header-action-btn"
          style={{
            background: 'none',
            border: 'none',
            padding: '8px',
            borderRadius: '8px',
            cursor: 'pointer',
            color: '#6c757d',
            transition: 'all 0.3s ease'
          }}
          onMouseEnter={(e) => e.target.style.backgroundColor = '#f8f9fa'}
          onMouseLeave={(e) => e.target.style.backgroundColor = 'transparent'}
          title="Notifications"
        >
          <Icon name="bell" />
        </button>

        {/* User menu - Future feature */}
        <button
          className="portal-header-action-btn"
          style={{
            background: 'none',
            border: 'none',
            padding: '8px',
            borderRadius: '8px',
            cursor: 'pointer',
            color: '#6c757d',
            transition: 'all 0.3s ease'
          }}
          onMouseEnter={(e) => e.target.style.backgroundColor = '#f8f9fa'}
          onMouseLeave={(e) => e.target.style.backgroundColor = 'transparent'}
          title="User menu"
        >
          <Icon name="person-circle" />
        </button>
      </div>
    </header>
  );
};

export default Header;
