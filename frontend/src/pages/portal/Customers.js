import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import Icon from '../../components/common/Icon';
import DataTable from '../../components/common/DataTable';
import { customerService, formatPhone, formatAddress, formatDate, getStatusLabel, getStatusColor } from '../../services/customerService';
import { useHeader } from '../../contexts/HeaderContext';

const Customers = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { updateHeader } = useHeader();
  const [customers, setCustomers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [stats, setStats] = useState({
    total: 0,
    active: 0,
    inactive: 0,
    newThisMonth: 0
  });
  
  // Filters and pagination
  const [currentPage, setCurrentPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [pagination, setPagination] = useState(null);
  
  // Success/Error messages
  const [message, setMessage] = useState(null);

  useEffect(() => {    
    updateHeader('Clientes', 'Gerencie seus clientes e mantenha informações atualizadas');
  }, []);

  useEffect(() => {
    // Update header when component mounts    
    loadCustomers();
    loadStats();
    
    // Check for success message from navigation state
    if (location.state?.message) {
      setMessage({
        text: location.state.message,
        type: location.state.type || 'success'
      });
      
      // Clear the message after 5 seconds
      setTimeout(() => setMessage(null), 5000);
      
      // Clear the state to prevent message from showing again on refresh
      navigate(location.pathname, { replace: true });
    }
  }, [currentPage, searchTerm, statusFilter, location.state, navigate, location.pathname]);

  const loadCustomers = async () => {
    try {
      setLoading(true);
      const response = await customerService.getCustomers(currentPage, 10, searchTerm, statusFilter);
      setCustomers(response.customers);
      setPagination(response.pagination);
    } catch (error) {
      console.error('Erro ao carregar clientes:', error);
    } finally {
      setLoading(false);
    }
  };

  const loadStats = async () => {
    try {
      const statsData = await customerService.getCustomerStats();
      setStats(statsData);
    } catch (error) {
      console.error('Erro ao carregar estatísticas:', error);
    }
  };

  const handleSearch = (value) => {
    setSearchTerm(value);
    setCurrentPage(1); // Reset to first page when searching
  };

  const handleStatusFilter = (status) => {
    setStatusFilter(status);
    setCurrentPage(1); // Reset to first page when filtering
  };

  const handlePageChange = (page) => {
    setCurrentPage(page);
  };

  const handleViewCustomer = (customer) => {
    navigate(`/portal/customers/${customer.id}/view`);
  };

  const handleEditCustomer = (customer) => {
    navigate(`/portal/customers/${customer.id}/edit`);
  };

  const handleDeleteCustomer = async (customer) => {
    try {
      await customerService.deleteCustomer(customer.id);
      loadCustomers(); // Reload customers after deletion
      loadStats(); // Update stats
      setMessage({
        text: `Cliente "${customer.name}" excluído com sucesso!`,
        type: 'success'
      });
      setTimeout(() => setMessage(null), 5000);
    } catch (error) {
      console.error('Erro ao excluir cliente:', error);
      setMessage({
        text: 'Erro ao excluir cliente. Tente novamente.',
        type: 'error'
      });
      setTimeout(() => setMessage(null), 5000);
    }
  };

  const handleCreateCustomer = () => {
    navigate('/portal/customers/new');
  };

  // Table columns configuration
  const columns = [
    {
      accessor: 'name',
      header: 'Nome',
      width: '25%',
      render: (customer) => (
        <div>
          <div className="fw-semibold">{customer.name}</div>
          <small className="text-muted">{customer.email}</small>
        </div>
      )
    },
    {
      accessor: 'phone',
      header: 'Telefone',
      width: '15%',
      render: (customer) => formatPhone(customer.phone)
    },
    {
      accessor: 'address',
      header: 'Endereço',
      width: '30%',
      render: (customer) => {
        const formattedAddress = formatAddress(customer.address);
        return (
          <div className="text-truncate" style={{ maxWidth: '250px' }} title={formattedAddress}>
            {formattedAddress}
          </div>
        );
      }
    },
    {
      accessor: 'status',
      header: 'Status',
      width: '10%',
      render: (customer) => (
        <span className={`badge bg-${getStatusColor(customer.status)}`}>
          {getStatusLabel(customer.status)}
        </span>
      )
    },
    {
      accessor: 'registrationDate',
      header: 'Cadastro',
      width: '15%',
      render: (customer) => (
        <small className="text-muted">
          {formatDate(customer.registrationDate)}
        </small>
      )
    }
  ];

  return (
    <div className="customers-page">  
      {/* Success/Error Message */}
      {message && (
        <div className={`alert alert-${message.type === 'success' ? 'success' : 'danger'} alert-dismissible fade show`} role="alert">
          <Icon name={message.type === 'success' ? 'check-circle' : 'triangle-exclamation'} className="me-2" />
          {message.text}
          <button 
            type="button" 
            className="btn-close" 
            onClick={() => setMessage(null)}
            aria-label="Fechar"
          ></button>
        </div>
      )}

      {/* Stats Cards */}
      <div className="row mb-4">
        <div className="col-md-3 mb-3">
          <div className="card border-primary">
            <div className="card-body">
              <div className="d-flex justify-content-between align-items-center">
                <div>
                  <h5 className="card-title text-primary mb-1">{stats.total}</h5>
                  <p className="card-text text-muted mb-0">Total de Clientes</p>
                </div>
                <div className="text-primary">
                  <Icon name="users" size="lg" />
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <div className="col-md-3 mb-3">
          <div className="card border-success">
            <div className="card-body">
              <div className="d-flex justify-content-between align-items-center">
                <div>
                  <h5 className="card-title text-success mb-1">{stats.active}</h5>
                  <p className="card-text text-muted mb-0">Clientes Ativos</p>
                </div>
                <div className="text-success">
                  <Icon name="user-check" size="lg" />
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <div className="col-md-3 mb-3">
          <div className="card border-secondary">
            <div className="card-body">
              <div className="d-flex justify-content-between align-items-center">
                <div>
                  <h5 className="card-title text-secondary mb-1">{stats.inactive}</h5>
                  <p className="card-text text-muted mb-0">Clientes Inativos</p>
                </div>
                <div className="text-secondary">
                  <Icon name="user-xmark" size="lg" />
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <div className="col-md-3 mb-3">
          <div className="card border-info">
            <div className="card-body">
              <div className="d-flex justify-content-between align-items-center">
                <div>
                  <h5 className="card-title text-info mb-1">{stats.newThisMonth}</h5>
                  <p className="card-text text-muted mb-0">Novos este Mês</p>
                </div>
                <div className="text-info">
                  <Icon name="user-plus" size="lg" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="d-flex justify-content-end align-items-center mb-4">
        <button
          className="btn btn-primary"
          onClick={handleCreateCustomer}
        >
          <Icon name="plus" size="sm" className="me-1" />
          Novo Cliente
        </button>
      </div>

      {/* Data Table */}
      <div className="card">
        <div className="card-body">
          <DataTable
            data={customers}
            columns={columns}
            loading={loading}
            onView={handleViewCustomer}
            onEdit={handleEditCustomer}
            onDelete={handleDeleteCustomer}
            pagination={pagination}
            onPageChange={handlePageChange}
            searchValue={searchTerm}
            onSearchChange={handleSearch}
            searchPlaceholder="Pesquisar por nome, email, telefone ou endereço..."
            emptyMessage="Nenhum cliente encontrado"
          />
        </div>
      </div>
    </div>
  );
};

export default Customers;
