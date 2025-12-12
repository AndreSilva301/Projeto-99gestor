import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import Icon from '../../components/common/Icon';
import DataTable from '../../components/common/DataTable';
import { quoteService, formatCurrency, formatDate, formatPaymentMethod } from '../../services/quoteService';
import { useHeader } from '../../contexts/HeaderContext';

const Quotes = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { updateHeader } = useHeader();
  const [quotes, setQuotes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [stats, setStats] = useState({
    total: 0,
    pending: 0,
    approved: 0,
    totalValue: 0
  });
  
  // Filters and pagination
  const [currentPage, setCurrentPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState('');
  const [pagination, setPagination] = useState(null);
  
  // Success/Error messages
  const [message, setMessage] = useState(null);

  useEffect(() => {    
    updateHeader('Orçamentos', 'Gerencie seus orçamentos e acompanhe aprovações');
  }, []);

  useEffect(() => {
    loadQuotes();
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
  }, [currentPage, searchTerm, location.state, navigate, location.pathname]);

  const loadQuotes = async () => {
    try {
      setLoading(true);
      const response = await quoteService.getQuotes(currentPage, 10, searchTerm);
      setQuotes(response.quotes);
      setPagination(response.pagination);
    } catch (error) {
      console.error('Erro ao carregar orçamentos:', error);
      setMessage({
        text: error.message || 'Erro ao carregar orçamentos',
        type: 'error'
      });
    } finally {
      setLoading(false);
    }
  };

  const loadStats = async () => {
    try {
      const statsData = await quoteService.getQuoteStats();
      setStats(statsData);
    } catch (error) {
      console.error('Erro ao carregar estatísticas:', error);
    }
  };

  const handleSearch = (value) => {
    setSearchTerm(value);
    setCurrentPage(1); // Reset to first page when searching
  };

  const handlePageChange = (page) => {
    setCurrentPage(page);
  };

  const handleViewQuote = (quote) => {
    navigate(`/portal/quotes/${quote.id}/view`);
  };

  const handleEditQuote = (quote) => {
    navigate(`/portal/quotes/${quote.id}/edit`);
  };

  const handleDeleteQuote = async (quote) => {
    try {
      await quoteService.deleteQuote(quote.id);
      loadQuotes(); // Reload quotes after deletion
      loadStats(); // Update stats
      setMessage({
        text: `Orçamento #${quote.id} excluído com sucesso!`,
        type: 'success'
      });
      setTimeout(() => setMessage(null), 5000);
    } catch (error) {
      console.error('Erro ao excluir orçamento:', error);
      setMessage({
        text: error.message || 'Erro ao excluir orçamento. Tente novamente.',
        type: 'error'
      });
      setTimeout(() => setMessage(null), 5000);
    }
  };

  const handleCreateQuote = () => {
    navigate('/portal/quotes/new');
  };

  // Table columns configuration
  const columns = [
    {
      accessor: 'id',
      header: 'ID',
      width: '8%',
      render: (quote) => (
        <div>
          <span className="badge bg-secondary">#{quote.id}</span>
        </div>
      )
    },
    {
      accessor: 'customerName',
      header: 'Cliente',
      width: '25%',
      render: (quote) => (
        <div>
          <div className="fw-semibold">{quote.customerName}</div>
          <small className="text-muted">{quote.userName}</small>
        </div>
      )
    },
    {
      accessor: 'totalPrice',
      header: 'Valor Total',
      width: '15%',
      render: (quote) => (
        <div className="fw-bold text-success">
          {formatCurrency(quote.totalPrice)}
        </div>
      )
    },
    {
      accessor: 'paymentMethod',
      header: 'Forma de Pagamento',
      width: '18%',
      render: (quote) => (
        <div>
          <div>{formatPaymentMethod(quote.paymentMethod)}</div>
          {quote.cashDiscount > 0 && (
            <small className="text-muted">
              Desconto: {quote.cashDiscount}%
            </small>
          )}
        </div>
      )
    },
    {
      accessor: 'quoteItems',
      header: 'Itens',
      width: '10%',
      render: (quote) => (
        <span className="badge bg-info">
          {quote.quoteItems.length} {quote.quoteItems.length === 1 ? 'item' : 'itens'}
        </span>
      )
    },
    {
      accessor: 'createdAt',
      header: 'Criado em',
      width: '18%',
      render: (quote) => (
        <small className="text-muted">
          {formatDate(quote.createdAt)}
        </small>
      )
    }
  ];

  return (
    <div className="quotes-page">  
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
                  <p className="card-text text-muted mb-0">Total de Orçamentos</p>
                </div>
                <div className="text-primary">
                  <Icon name="document" size="lg" />
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <div className="col-md-3 mb-3">
          <div className="card border-warning">
            <div className="card-body">
              <div className="d-flex justify-content-between align-items-center">
                <div>
                  <h5 className="card-title text-warning mb-1">{stats.pending}</h5>
                  <p className="card-text text-muted mb-0">Pendentes</p>
                </div>
                <div className="text-warning">
                  <Icon name="clock" size="lg" />
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
                  <h5 className="card-title text-success mb-1">{stats.approved}</h5>
                  <p className="card-text text-muted mb-0">Aprovados</p>
                </div>
                <div className="text-success">
                  <Icon name="check-circle" size="lg" />
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
                  <h5 className="card-title text-info mb-1">{formatCurrency(stats.totalValue)}</h5>
                  <p className="card-text text-muted mb-0">Valor Total</p>
                </div>
                <div className="text-info">
                  <Icon name="currency-dollar" size="lg" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="d-flex justify-content-end align-items-center mb-4">
        <button
          className="btn btn-primary"
          onClick={handleCreateQuote}
        >
          <Icon name="plus" size="sm" className="me-1" />
          Novo Orçamento
        </button>
      </div>

      {/* Data Table */}
      <div className="card">
        <div className="card-body">
          <DataTable
            data={quotes}
            columns={columns}
            loading={loading}
            onView={handleViewQuote}
            onEdit={handleEditQuote}
            onDelete={handleDeleteQuote}
            pagination={pagination}
            onPageChange={handlePageChange}
            searchValue={searchTerm}
            onSearchChange={handleSearch}
            searchPlaceholder="Pesquisar por cliente ou usuário..."
            emptyMessage="Nenhum orçamento encontrado"
          />
        </div>
      </div>
    </div>
  );
};

export default Quotes;
