import React from 'react';
import { Routes, Route } from 'react-router-dom';
import { PortalLayout } from '../components/portal';
import { 
  Dashboard, 
  Customers, 
  Quotes, 
  Employees, 
  Company, 
  Settings 
} from '../pages/portal';
import CustomerCreate from '../pages/portal/CustomerCreate';
import CustomerEdit from '../pages/portal/CustomerEdit';
import CustomerView from '../pages/portal/CustomerView';

const PortalRouter = () => {
  return (
    <Routes>
      <Route path="/" element={<PortalLayout />}>
        <Route index element={<Dashboard />} />
        <Route path="customers" element={<Customers />} />
        <Route path="customers/new" element={<CustomerCreate />} />
        <Route path="customers/:id/edit" element={<CustomerEdit />} />
        <Route path="customers/:id/view" element={<CustomerView />} />
        <Route path="quotes" element={<Quotes />} />
        <Route path="employees" element={<Employees />} />
        <Route path="company" element={<Company />} />
        <Route path="settings" element={<Settings />} />
      </Route>
    </Routes>
  );
};

export default PortalRouter;
