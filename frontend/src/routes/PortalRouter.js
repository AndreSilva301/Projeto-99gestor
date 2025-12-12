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
import QuoteDetails from '../pages/portal/QuoteDetails';
import EmployeeDetail from '../pages/portal/EmployeeDetail';
import CompanySettings from '../pages/portal/settings/CompanySettings';

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
        <Route path="quotes/new" element={<QuoteDetails />} />
        <Route path="quotes/:id/edit" element={<QuoteDetails />} />
        <Route path="quotes/:id/view" element={<QuoteDetails />} />
        <Route path="employees" element={<Employees />} />
        <Route path="employees/:id" element={<EmployeeDetail />} />
        <Route path="company" element={<Company />} />
        <Route path="settings" element={<Settings />} />
        <Route path="settings/company" element={<CompanySettings />} />
      </Route>
    </Routes>
  );
};

export default PortalRouter;
