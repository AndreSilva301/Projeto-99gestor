import React from 'react';
import ComingSoonPage from '../../components/portal/ComingSoonPage';

const Company = () => {
  return (
    <ComingSoonPage
      title="Company Settings"
      description="Manage your company information, branding, and general configuration settings."
      features={[
        'Update company information',
        'Manage company branding',
        'Configure quote templates',
        'Set payment conditions',
        'Manage additional quote fields',
        'Company profile settings'
      ]}
    />
  );
};

export default Company;
