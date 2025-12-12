# Dashboard Integration

## Overview
The Dashboard has been integrated with the backend API to display real-time statistics.

## Files Created/Modified

### New Files
- `frontend/src/services/dashboardService.js` - Service to handle dashboard API calls

### Modified Files
- `frontend/src/pages/portal/Dashboard.js` - Updated to use real API instead of mock data

## API Integration

### Endpoint
- **GET** `/api/dashboard/stats`
- **Controller**: `DashboardController.cs`
- **Service**: `DashboardService.cs`
- **DTO**: `DashboardStatsDto.cs`

### Response Structure
```json
{
  "totalCustomers": 150,
  "customersThisMonth": 12,
  "customersLastMonth": 8,
  "totalQuotes": 45,
  "quotesThisMonth": 7,
  "quotesLastMonth": 5,
  "revenueThisMonth": 25000.00,
  "revenueLastMonth": 18000.00,
  "totalEmployees": 5
}
```

## Features

### Statistics Cards
1. **Total de clientes**
   - Shows total customer count
   - Displays new customers this month
   - Positive/negative trend indicator

2. **Orçamentos pendentes**
   - Shows total quotes (displayed as pending)
   - Displays new quotes this month
   - Positive/negative trend indicator

3. **Receita mensal**
   - Shows revenue for current month
   - Displays last month revenue for comparison
   - Positive/negative trend indicator

4. **Funcionários ativos**
   - Shows total employees count
   - No trend indicator

### Quick Actions
- Add new customer (links to `/portal/customers/new`)
- Create new quote (links to `/portal/quotes/new`)

### Loading State
- Shows spinner with "Carregando dashboard..." message
- Centered layout

### Error State
- Shows error icon and message
- Provides "Tentar novamente" button to reload
- User-friendly error messages

## Error Handling
The service handles various error scenarios:
- 401 (Unauthorized): "Sessão expirada. Faça login novamente."
- 403 (Forbidden): "Acesso negado"
- Network errors: Generic error message
- Default: "Erro ao carregar estatísticas do dashboard"

## Dependencies
- `httpClient` - HTTP service for API calls
- `HeaderContext` - For page title/subtitle
- `formatCurrency` - From `quoteService` for formatting currency values
- `Icon` - Component for displaying icons

## Usage
The Dashboard component automatically loads statistics when mounted. No manual intervention needed.

```javascript
import Dashboard from './pages/portal/Dashboard';

// In your router
<Route path="/portal/dashboard" element={<Dashboard />} />
```

## Future Enhancements
- Real-time updates with WebSocket
- Date range selector
- Export statistics to PDF/Excel
- More detailed charts and graphs
- Filter by date range
- Drill-down capabilities for each stat
