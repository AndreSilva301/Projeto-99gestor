import React from 'react';
import ComingSoonPage from '../../components/portal/ComingSoonPage';

const Settings = () => {
  return (
    <ComingSoonPage
      title="System Settings"
      description="Configure your CRM preferences, notifications, and user account settings."
      features={[
        'User profile settings',
        'Notification preferences',
        'System configuration',
        'Data export/import',
        'Security settings',
        'Integration settings'
      ]}
    />
  );
};

export default Settings;
