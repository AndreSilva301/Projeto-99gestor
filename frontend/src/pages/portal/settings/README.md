# Company Settings Implementation

This document describes the implementation of the Company Settings feature, following the tasks outlined in `endpoints-company.md`.

## 📁 New File Structure

```
frontend/src/
├── pages/portal/settings/
│   ├── index.js                  # Main Settings page with navigation
│   └── CompanySettings.js        # Company details view/edit page
├── components/portal/
│   └── CompanyForm.js           # Reusable company form component
└── services/
    └── companyService.js        # API service for company operations
```

## 🔧 Features Implemented

### Settings Navigation Page
- **Location**: `pages/portal/settings/index.js`
- **Features**:
  - Card-based navigation to different settings sections
  - Role-based access control (displays which options user can access)
  - "Coming Soon" badges for future features
  - Responsive grid layout

### Company Settings Page
- **Location**: `pages/portal/settings/CompanySettings.js`
- **Features**:
  - View/Edit company information
  - Role-based permissions (Admin/SystemAdmin can edit)
  - Error handling and success messages
  - Loading states
  - Data validation

### Company Form Component
- **Location**: `components/portal/CompanyForm.js`
- **Features**:
  - Reusable form component following project patterns
  - Same Address and Phone structure as Customer entity
  - Field validation and formatting
  - Support for view/edit modes
  - Proper error display

### Company Service
- **Location**: `services/companyService.js`
- **Features**:
  - API methods for company operations
  - Mock data for development
  - Error handling
  - Data validation utilities
  - CNPJ formatting

## 🔐 Access Control

| User Profile | Permissions |
|-------------|-------------|
| **SystemAdmin** | View all companies, Edit any company |
| **Admin** | View own company, Edit own company |
| **Employee** | View own company only |

## 🚀 Navigation Flow

1. **Settings Page** (`/portal/settings`)
   - Displays available settings options
   - Shows user permissions for each option
   
2. **Company Settings** (`/portal/settings/company`)
   - Accessible from Settings page
   - View mode by default
   - Edit button for authorized users

## 📋 API Endpoints Expected

Based on the tasks document, the following endpoints should be implemented:

```
GET /api/company          # List all companies (SystemAdmin only)
GET /api/company/{id}     # Get company by ID
PUT /api/company/{id}     # Update company
```

## 🎨 UI/UX Features

- **Consistent Design**: Follows existing project patterns
- **Responsive Layout**: Works on desktop and mobile
- **Loading States**: Clear feedback during operations
- **Error Handling**: User-friendly error messages
- **Success Feedback**: Confirmation of successful operations
- **Permission Indicators**: Clear indication of user capabilities

## 📝 Data Structure

The company data follows the same pattern as Customer entity:

```javascript
{
  id: number,
  name: string,
  cnpj: string,
  address: {
    street: string,
    number: string,
    complement: string,
    neighborhood: string,
    city: string,
    state: string,
    zipCode: string
  },
  phone: {
    mobile: string,
    landline: string
  },
  dateTime: string
}
```

## 🔄 Integration Points

1. **Authentication Context**: Currently uses mock data, should integrate with real auth
2. **API Service**: Ready for backend integration
3. **Routing**: Integrated with existing portal router
4. **Components**: Reuses existing Icon and form patterns

## 🛠️ Next Steps

1. **Backend Integration**: Connect to real API endpoints
2. **Authentication**: Integrate with real user context
3. **Testing**: Add unit and integration tests
4. **Additional Settings**: Implement other settings pages (Profile, Security, etc.)

## 📖 Usage Examples

### Basic Usage
```javascript
// View company settings
navigate('/portal/settings/company');

// Edit company data
const updatedData = await companyService.updateCompany(companyId, formData);
```

### Component Usage
```javascript
<CompanyForm
  company={companyData}
  mode="edit"
  currentUserProfile="Admin"
  onSubmit={handleSave}
  onCancel={handleCancel}
/>
```

This implementation provides a solid foundation for company management while maintaining consistency with the existing codebase patterns and following the specifications from the tasks document.
