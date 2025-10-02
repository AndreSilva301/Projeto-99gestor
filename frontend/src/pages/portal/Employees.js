import React from 'react';
import ComingSoonPage from '../../components/portal/ComingSoonPage';

const Employees = () => {
  return (
    <ComingSoonPage
      title="Employee Management"
      description="Manage your team members and their access levels. Only administrators can add new employees according to MVP requirements."
      features={[
        'Add new employees (Admin only)',
        'Manage employee profiles',
        'Set access permissions',
        'Employee activity tracking',
        'Role-based access control',
        'Employee performance metrics'
      ]}
    />
  );
};

export default Employees;
