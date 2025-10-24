import React from 'react';
import { useNavigate } from 'react-router-dom';
import Icon from '../../../components/common/Icon';

const Settings = () => {
  const navigate = useNavigate();

  const settingsOptions = [
    {
      id: 'company',
      title: 'Configurações da Empresa',
      description: 'Gerencie as informações da sua empresa, endereço e detalhes de contato',
      icon: 'building',
      path: '/portal/settings/company',
      requiredRole: ['Admin', 'SystemAdmin']
    },
    {
      id: 'profile',
      title: 'User Profile',
      description: 'Update your personal information and preferences',
      icon: 'user',
      path: '/portal/settings/profile',
      requiredRole: ['Admin', 'Employee', 'SystemAdmin'],
      comingSoon: true
    },
    {
      id: 'notifications',
      title: 'Notification Preferences',
      description: 'Configure how and when you receive notifications',
      icon: 'bell',
      path: '/portal/settings/notifications',
      requiredRole: ['Admin', 'Employee', 'SystemAdmin'],
      comingSoon: true
    },
    {
      id: 'security',
      title: 'Security Settings',
      description: 'Manage passwords, two-factor authentication, and security preferences',
      icon: 'shield-check',
      path: '/portal/settings/security',
      requiredRole: ['Admin', 'Employee', 'SystemAdmin'],
      comingSoon: true
    },
    {
      id: 'integrations',
      title: 'Integration Settings',
      description: 'Configure third-party integrations and API settings',
      icon: 'link',
      path: '/portal/settings/integrations',
      requiredRole: ['Admin', 'SystemAdmin'],
      comingSoon: true
    },
    {
      id: 'system',
      title: 'System Configuration',
      description: 'Advanced system settings and configurations',
      icon: 'cog',
      path: '/portal/settings/system',
      requiredRole: ['SystemAdmin'],
      comingSoon: true
    }
  ];

  // Mock user data - in real app, this would come from auth context
  const currentUser = {
    profile: 'Admin' // This should come from auth context
  };

  const handleOptionClick = (option) => {
    if (option.comingSoon) {
      return;
    }
    navigate(option.path);
  };

  const canAccessOption = (option) => {
    return option.requiredRole.includes(currentUser.profile);
  };

  return (
    <div className="settings-page">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="mb-1">
            <Icon name="cog" className="me-2" />
            Settings
          </h2>
          <p className="text-muted mb-0">
            Manage your account settings and preferences
          </p>
        </div>
      </div>

      <div className="row g-4">
        {settingsOptions.map((option) => {
          const canAccess = canAccessOption(option);
          
          return (
            <div key={option.id} className="col-md-6 col-lg-4">
              <div 
                className={`card h-100 settings-card ${
                  canAccess && !option.comingSoon ? 'clickable' : 'disabled'
                }`}
                onClick={() => canAccess && handleOptionClick(option)}
                style={{
                  cursor: canAccess && !option.comingSoon ? 'pointer' : 'not-allowed',
                  opacity: canAccess ? 1 : 0.6
                }}
              >
                <div className="card-body d-flex flex-column">
                  <div className="d-flex align-items-center mb-3">
                    <div className={`icon-wrapper me-3 ${canAccess ? 'text-primary' : 'text-muted'}`}>
                      <Icon name={option.icon} size="lg" />
                    </div>
                    <div className="flex-grow-1">
                      <h5 className="card-title mb-0 d-flex align-items-center">
                        {option.title}
                        {option.comingSoon && (
                          <span className="badge bg-warning text-dark ms-2">
                            Coming Soon
                          </span>
                        )}
                        {!canAccess && (
                          <span className="badge bg-secondary ms-2">
                            Restricted
                          </span>
                        )}
                      </h5>
                    </div>
                  </div>
                  
                  <p className="card-text text-muted flex-grow-1">
                    {option.description}
                  </p>
                  
                  {canAccess && !option.comingSoon && (
                    <div className="mt-auto">
                      <Icon name="arrow-right" className="text-primary" />
                    </div>
                  )}
                </div>
              </div>
            </div>
          );
        })}
      </div>

      <style>{`
        .settings-card {
          transition: all 0.2s ease-in-out;
          border: 1px solid #dee2e6;
        }
        
        .settings-card.clickable:hover {
          transform: translateY(-2px);
          box-shadow: 0 4px 8px rgba(0,0,0,0.1);
          border-color: #007bff;
        }
        
        .settings-card.disabled {
          background-color: #f8f9fa;
        }
        
        .icon-wrapper {
          display: flex;
          align-items: center;
          justify-content: center;
          width: 48px;
          height: 48px;
          border-radius: 8px;
          background-color: rgba(13, 110, 253, 0.1);
        }
        
        .settings-card.disabled .icon-wrapper {
          background-color: rgba(108, 117, 125, 0.1);
        }
      `}</style>
    </div>
  );
};

export default Settings;
