import React from 'react';
import ComingSoonPage from '../../components/portal/ComingSoonPage';

const Customers = () => {
  return (
    <ComingSoonPage
      title="Customer Management"
      description="This section will allow you to manage all your customers, add new ones from your phone contacts or forms, and store relationship information."
      features={[
        'Add customers from phone contacts',
        'Create customers via form',
        'Store personal and contact information',
        'Manage customer relationships and notes',
        'Search and filter customer list',
        'Customer history and quotes'
      ]}
    />
  );
};

export default Customers;
