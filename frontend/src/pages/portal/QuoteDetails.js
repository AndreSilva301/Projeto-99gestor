import React, { useState, useEffect } from 'react';
import { useNavigate, useParams, useLocation } from 'react-router-dom';
import { useHeader } from '../../contexts/HeaderContext';
import { quoteService, formatCurrency } from '../../services/quoteService';
import { customerService } from '../../services/customerService';
import Icon from '../../components/common/Icon';
import './QuoteDetails.css';

const QuoteDetails = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const location = useLocation();
  const { updateHeader } = useHeader();
  
  // Determine mode from route
  const isViewMode = location.pathname.includes('/view');
  const isEditMode = location.pathname.includes('/edit');
  const isCreateMode = !id || location.pathname.includes('/new');
  
  const [loading, setLoading] = useState(false);
  const [loadingCustomers, setLoadingCustomers] = useState(false);
  const [customers, setCustomers] = useState([]);
  const [customerSearch, setCustomerSearch] = useState('');
  const [showCustomerDropdown, setShowCustomerDropdown] = useState(false);
  const [filteredCustomers, setFilteredCustomers] = useState([]);
  const [errors, setErrors] = useState({});
  const [message, setMessage] = useState(null);
  
  const [formData, setFormData] = useState({
    customerId: '',
    paymentMethod: 0, // Default to Dinheiro
    paymentConditions: '',
    cashDiscount: 0,
    items: [{
      description: '',
      quantity: 1,
      unitPrice: 0,
      totalPrice: 0,
      customFields: {}
    }],
    customFields: {}
  });
  
  // Custom fields state - HIDDEN FOR NOW
  // const [customFieldKey, setCustomFieldKey] = useState('');
  // const [customFieldValue, setCustomFieldValue] = useState('');
  // const [itemCustomFields, setItemCustomFields] = useState({}); // Track custom fields per item

  // Payment method options
  const paymentMethods = [
    { value: 0, label: 'Dinheiro' },
    { value: 1, label: 'Cartão de Crédito' },
    { value: 2, label: 'Cartão de Débito' },
    { value: 3, label: 'PIX' },
    { value: 4, label: 'Transferência Bancária' },
    { value: 5, label: 'Boleto' }
  ];

  // Set page title based on mode
  useEffect(() => {
    if (isViewMode) {
      updateHeader('Visualizar Orçamento', 'Detalhes do orçamento');
    } else if (isEditMode) {
      updateHeader('Editar Orçamento', 'Modifique as informações do orçamento');
    } else {
      updateHeader('Novo Orçamento', 'Crie um novo orçamento para seu cliente');
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isViewMode, isEditMode]);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (showCustomerDropdown && !event.target.closest('.autocomplete-wrapper')) {
        setShowCustomerDropdown(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [showCustomerDropdown]);

  // Load customers for dropdown
  useEffect(() => {
    const loadCustomers = async () => {
      try {
        setLoadingCustomers(true);
        const result = await customerService.getCustomers(1, 1000, '', 'all'); // Load all customers
        setCustomers(result.customers || []);
      } catch (error) {
        console.error('Error loading customers:', error);
        setMessage({ type: 'error', text: 'Erro ao carregar lista de clientes' });
      } finally {
        setLoadingCustomers(false);
      }
    };
    
    // Load customers for both create and edit modes (not in view mode)
    if (!isViewMode) {
      loadCustomers();
    }
  }, [isViewMode]);

  // Load quote data for edit/view mode
  useEffect(() => {    
    loadQuote();
  }, [id, isCreateMode]);

  
  const loadQuote = async () => {
    if (!id || isCreateMode) return;
    
    try {
      setLoading(true);
      const quote = await quoteService.getQuoteById(id);
      
      setFormData({
        customerId: quote.customerId || '',
        paymentMethod: quote.paymentMethod || 0,
        paymentConditions: quote.paymentConditions || '',
        cashDiscount: quote.cashDiscount || 0,
        items: quote.quoteItems && quote.quoteItems.length > 0 ? quote.quoteItems.map(item => ({
          id: item.id,
          description: item.description || '',
          quantity: item.quantity || 1,
          unitPrice: item.unitPrice || 0,
          totalPrice: item.totalPrice || 0,
          customFields: item.customFields || {}
        })) : [{
          description: '',
          quantity: 1,
          unitPrice: 0,
          totalPrice: 0,
          customFields: {}
        }],
        customFields: quote.customFields || {}
      });
      
      // Set customer search with customer name
      if (quote.customerName) {
        setCustomerSearch(quote.customerName);
      }
    } catch (error) {
      console.error('Error loading quote:', error);
      setMessage({ type: 'error', text: 'Erro ao carregar orçamento' });
    } finally {
      setLoading(false);
    }
  };

  // Calculate item total price
  const calculateItemTotal = (quantity, unitPrice) => {
    const qty = parseFloat(quantity) || 0;
    const price = parseFloat(unitPrice) || 0;
    return qty * price;
  };

  // Calculate quote total
  const calculateQuoteTotal = () => {
    const subtotal = formData.items.reduce((sum, item) => {
      return sum + (parseFloat(item.totalPrice) || 0);
    }, 0);
    
    const discount = parseFloat(formData.cashDiscount) || 0;
    return Math.max(0, subtotal - discount);
  };

  // Handle customer search
  const handleCustomerSearch = (value) => {
    setCustomerSearch(value);
    
    if (value.trim().length > 0) {
      const filtered = customers.filter(customer =>
        customer.name.toLowerCase().includes(value.toLowerCase())
      );
      setFilteredCustomers(filtered);
      setShowCustomerDropdown(true);
    } else {
      setFilteredCustomers([]);
      setShowCustomerDropdown(false);
    }
  };

  // Handle customer selection from dropdown
  const handleCustomerSelect = (customer) => {
    setFormData(prev => ({ ...prev, customerId: customer.id }));
    setCustomerSearch(customer.name);
    setShowCustomerDropdown(false);
    
    // Clear error
    if (errors.customerId) {
      setErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors.customerId;
        return newErrors;
      });
    }
  };

  // Handle input change
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'paymentMethod' ? parseInt(value) : value
    }));
    
    // Clear error for this field
    if (errors[name]) {
      setErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[name];
        return newErrors;
      });
    }
  };

  // Handle item change
  const handleItemChange = (index, field, value) => {
    const newItems = [...formData.items];
    newItems[index] = {
      ...newItems[index],
      [field]: value
    };
    
    // Recalculate item total if quantity or unit price changed
    if (field === 'quantity' || field === 'unitPrice') {
      newItems[index].totalPrice = calculateItemTotal(
        newItems[index].quantity,
        newItems[index].unitPrice
      );
    }
    
    setFormData(prev => ({ ...prev, items: newItems }));
    
    // Clear error for this item
    if (errors[`item${index}`]) {
      setErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[`item${index}`];
        return newErrors;
      });
    }
  };

  // Add new item
  const handleAddItem = () => {
    setFormData(prev => ({
      ...prev,
      items: [...prev.items, {
        description: '',
        quantity: 1,
        unitPrice: 0,
        totalPrice: 0,
        customFields: {}
      }]
    }));
  };

  // Remove item
  const handleRemoveItem = (index) => {
    if (formData.items.length === 1) {
      setMessage({ type: 'error', text: 'Deve haver pelo menos um item no orçamento' });
      return;
    }
    
    setFormData(prev => ({
      ...prev,
      items: prev.items.filter((_, i) => i !== index)
    }));
    
    // Clear error for this item
    setErrors(prev => {
      const newErrors = { ...prev };
      delete newErrors[`item${index}`];
      return newErrors;
    });
  };

  // Add custom field to quote - HIDDEN FOR NOW
  /* const handleAddCustomField = () => {
    if (!customFieldKey.trim()) {
      setMessage({ type: 'error', text: 'Nome do campo customizado não pode estar vazio' });
      return;
    }
    
    if (customFieldKey.length > 50) {
      setMessage({ type: 'error', text: 'Nome do campo customizado não pode ter mais de 50 caracteres' });
      return;
    }
    
    if (customFieldValue && customFieldValue.length > 200) {
      setMessage({ type: 'error', text: 'Valor do campo customizado não pode ter mais de 200 caracteres' });
      return;
    }
    
    setFormData(prev => ({
      ...prev,
      customFields: {
        ...prev.customFields,
        [customFieldKey]: customFieldValue
      }
    }));
    
    setCustomFieldKey('');
    setCustomFieldValue('');
  }; */

  // Remove custom field from quote - HIDDEN FOR NOW
  /* const handleRemoveCustomField = (key) => {
    setFormData(prev => {
      const newCustomFields = { ...prev.customFields };
      delete newCustomFields[key];
      return {
        ...prev,
        customFields: newCustomFields
      };
    });
  }; */

  // Add custom field to item - HIDDEN FOR NOW
  /* const handleAddItemCustomField = (itemIndex, key, value) => {
    if (!key.trim()) {
      setMessage({ type: 'error', text: 'Nome do campo customizado não pode estar vazio' });
      return;
    }
    
    if (key.length > 50) {
      setMessage({ type: 'error', text: 'Nome do campo customizado não pode ter mais de 50 caracteres' });
      return;
    }
    
    if (value && value.length > 200) {
      setMessage({ type: 'error', text: 'Valor do campo customizado não pode ter mais de 200 caracteres' });
      return;
    }
    
    const newItems = [...formData.items];
    newItems[itemIndex] = {
      ...newItems[itemIndex],
      customFields: {
        ...newItems[itemIndex].customFields,
        [key]: value
      }
    };
    
    setFormData(prev => ({ ...prev, items: newItems }));
    
    // Clear temporary state for this item
    setItemCustomFields(prev => {
      const newState = { ...prev };
      delete newState[itemIndex];
      return newState;
    });
  }; */

  // Remove custom field from item - HIDDEN FOR NOW
  /* const handleRemoveItemCustomField = (itemIndex, key) => {
    const newItems = [...formData.items];
    const newCustomFields = { ...newItems[itemIndex].customFields };
    delete newCustomFields[key];
    
    newItems[itemIndex] = {
      ...newItems[itemIndex],
      customFields: newCustomFields
    };
    
    setFormData(prev => ({ ...prev, items: newItems }));
  }; */

  // Validate form
  const validateForm = () => {
    const newErrors = {};
    
    // Validate customer
    if (!formData.customerId) {
      newErrors.customerId = 'Cliente é obrigatório';
    }
    
    // Validate payment conditions length
    if (formData.paymentConditions && formData.paymentConditions.length > 500) {
      newErrors.paymentConditions = 'Condições de pagamento não podem ter mais de 500 caracteres';
    }
    
    // Validate cash discount
    if (formData.cashDiscount < 0) {
      newErrors.cashDiscount = 'Desconto não pode ser negativo';
    }
    
    // Validate items
    if (formData.items.length === 0) {
      newErrors.items = 'Deve haver pelo menos um item';
    } else {
      formData.items.forEach((item, index) => {
        if (!item.description || !item.description.trim()) {
          newErrors[`item${index}`] = 'Descrição do item é obrigatória';
        } else if (item.description.length > 200) {
          newErrors[`item${index}`] = 'Descrição do item não pode ter mais de 200 caracteres';
        }
        
        if (item.quantity !== undefined && item.quantity < 0) {
          newErrors[`item${index}`] = 'Quantidade não pode ser negativa';
        }
        
        if (item.unitPrice !== undefined && item.unitPrice < 0) {
          newErrors[`item${index}`] = 'Preço unitário não pode ser negativo';
        }
      });
    }
    
    // Validate custom fields
    Object.entries(formData.customFields).forEach(([key, value]) => {
      if (!key.trim()) {
        newErrors.customFields = 'Chave do campo customizado não pode estar vazia';
      } else if (key.length > 50) {
        newErrors.customFields = 'Chave do campo customizado não pode ter mais de 50 caracteres';
      }
      
      if (value && value.length > 200) {
        newErrors.customFields = 'Valor do campo customizado não pode ter mais de 200 caracteres';
      }
    });
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Handle submit
  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!validateForm()) {
      setMessage({ type: 'error', text: 'Por favor, corrija os erros no formulário' });
      return;
    }
    
    try {
      setLoading(true);
      
      if (isCreateMode) {
        // Create new quote
        const createData = {
          customerId: parseInt(formData.customerId),
          paymentMethod: formData.paymentMethod,
          paymentConditions: formData.paymentConditions || null,
          cashDiscount: parseFloat(formData.cashDiscount) || 0,
          items: formData.items.map(item => ({
            description: item.description,
            quantity: parseFloat(item.quantity) || null,
            unitPrice: parseFloat(item.unitPrice) || null,
            totalPrice: parseFloat(item.totalPrice) || null,
            customFields: item.customFields
          })),
          customFields: formData.customFields
        };
        
        await quoteService.createQuote(createData);
        navigate('/portal/quotes', {
          state: {
            message: 'Orçamento criado com sucesso!',
            type: 'success'
          }
        });
      } else {
        // Update existing quote
        const updateData = {
          id: parseInt(id),
          totalPrice: calculateQuoteTotal(),
          paymentMethod: formData.paymentMethod,
          paymentConditions: formData.paymentConditions || null,
          cashDiscount: parseFloat(formData.cashDiscount) || 0,
          items: formData.items.map((item, index) => ({
            id: item.id || 0,
            quoteId: parseInt(id),
            order: index + 1,
            description: item.description,
            quantity: parseFloat(item.quantity) || null,
            unitPrice: parseFloat(item.unitPrice) || null,
            totalPrice: parseFloat(item.totalPrice) || null,
            customFields: item.customFields
          })),
          customFields: formData.customFields
        };
        
        await quoteService.updateQuote(id, updateData);
        navigate('/portal/quotes', {
          state: {
            message: 'Orçamento atualizado com sucesso!',
            type: 'success'
          }
        });
      }
    } catch (error) {
      console.error('Error saving quote:', error);
      setMessage({
        type: 'error',
        text: error.message || 'Erro ao salvar orçamento. Tente novamente.'
      });
    } finally {
      setLoading(false);
    }
  };

  // Handle cancel
  const handleCancel = () => {
    navigate('/portal/quotes');
  };

  // Handle edit button (in view mode)
  const handleEdit = () => {
    navigate(`/portal/quotes/${id}/edit`);
  };

  if (loading && !isCreateMode) {
    return (
      <div className="quote-details-loading">
        <div className="spinner"></div>
        <p>Carregando orçamento...</p>
      </div>
    );
  }

  return (
    <div className="quote-details-page">
      {message && (
        <div className={`message message-${message.type}`}>
          <Icon name={message.type === 'success' ? 'check-circle' : 'x-circle'} />
          <span>{message.text}</span>
          <button onClick={() => setMessage(null)} className="message-close">
            <Icon name="x" />
          </button>
        </div>
      )}

      <div className="card">
        <div className="card-body">
          <form onSubmit={handleSubmit}>
            {/* Customer Selection with Autocomplete */}
            <div className="form-section">
              <h3>Cliente</h3>
              <div className="form-group">
                <label htmlFor="customerSearch">
                  Cliente <span className="required">*</span>
                </label>
                {isViewMode ? (
                  <input
                    type="text"
                    value={customerSearch || 'Carregando...'}
                    disabled
                    className="form-control"
                  />
                ) : (
                  <div className="autocomplete-wrapper">
                    <input
                      type="text"
                      id="customerSearch"
                      value={customerSearch}
                      onChange={(e) => handleCustomerSearch(e.target.value)}
                      onFocus={() => {
                        if (customerSearch.trim().length > 0) {
                          setShowCustomerDropdown(true);
                        }
                      }}
                      placeholder="Digite o nome do cliente..."
                      disabled={loadingCustomers}
                      className={`form-control ${errors.customerId ? 'is-invalid' : ''}`}
                      autoComplete="off"
                    />
                    {showCustomerDropdown && filteredCustomers.length > 0 && (
                      <div className="autocomplete-dropdown">
                        {filteredCustomers.map(customer => (
                          <div
                            key={customer.id}
                            className="autocomplete-item"
                            onClick={() => handleCustomerSelect(customer)}
                          >
                            <div className="customer-name">{customer.name}</div>
                            {customer.address && customer.address.street && (
                              <div className="customer-info">
                                {customer.address.street}
                                {customer.address.number && `, ${customer.address.number}`}
                              </div>
                            )}
                            {customer.phone && customer.phone.mobile && (
                              <div className="customer-info">{customer.phone.mobile}</div>
                            )}
                          </div>
                        ))}
                      </div>
                    )}
                    {showCustomerDropdown && customerSearch.trim().length > 0 && filteredCustomers.length === 0 && (
                      <div className="autocomplete-dropdown">
                        <div className="autocomplete-item no-results">
                          Nenhum cliente encontrado
                        </div>
                      </div>
                    )}
                  </div>
                )}
                {errors.customerId && (
                  <div className="invalid-feedback">{errors.customerId}</div>
                )}
              </div>
            </div>

            {/* Items */}
            <div className="form-section">
              <div className="section-header">
                <h3>Itens do Orçamento <span className="required">*</span></h3>
                {!isViewMode && (
                  <button
                    type="button"
                    onClick={handleAddItem}
                    className="btn btn-secondary btn-sm"
                  >
                    <Icon name="plus" /> Adicionar Item
                  </button>
                )}
              </div>

              {errors.items && (
                <div className="alert alert-danger">{errors.items}</div>
              )}

              <div className="items-list">
                {formData.items.map((item, index) => (
                  <div key={index} className="item-card-compact">
                    <div className="item-header">
                      <h4>Item {index + 1}</h4>
                      {!isViewMode && formData.items.length > 1 && (
                        <button
                          type="button"
                          onClick={() => handleRemoveItem(index)}
                          className="btn btn-danger btn-sm"
                        >
                          <Icon name="trash" /> Remover
                        </button>
                      )}
                    </div>

                    {errors[`item${index}`] && (
                      <div className="alert alert-danger">{errors[`item${index}`]}</div>
                    )}

                    <div className="item-fields-compact">
                      <div className="form-group item-description">
                        <label htmlFor={`item-description-${index}`}>
                          Descrição <span className="required">*</span>
                        </label>
                        <input
                          type="text"
                          id={`item-description-${index}`}
                          value={item.description}
                          onChange={(e) => handleItemChange(index, 'description', e.target.value)}
                          disabled={isViewMode}
                          maxLength="200"
                          placeholder="Descrição do serviço ou produto"
                          className={`form-control ${errors[`item${index}`] ? 'is-invalid' : ''}`}
                        />
                      </div>

                      <div className="form-group item-quantity">
                        <label htmlFor={`item-quantity-${index}`}>Qtd</label>
                        <input
                          type="number"
                          id={`item-quantity-${index}`}
                          value={item.quantity}
                          onChange={(e) => handleItemChange(index, 'quantity', e.target.value)}
                          disabled={isViewMode}
                          min="0"
                          step="0.01"
                          className="form-control"
                        />
                      </div>

                      <div className="form-group item-price">
                        <label htmlFor={`item-unitPrice-${index}`}>Preço Unit.</label>
                        <input
                          type="number"
                          id={`item-unitPrice-${index}`}
                          value={item.unitPrice}
                          onChange={(e) => handleItemChange(index, 'unitPrice', e.target.value)}
                          disabled={isViewMode}
                          min="0"
                          step="0.01"
                          placeholder="0.00"
                          className="form-control"
                        />
                      </div>

                      <div className="form-group item-total">
                        <label>Total</label>
                        <input
                          type="text"
                          value={formatCurrency(item.totalPrice)}
                          disabled
                          className="form-control"
                        />
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>

            {/* Payment Information - MOVED AFTER ITEMS */}
            <div className="form-section">
              <h3>Pagamento</h3>
              <div className="form-row">
                <div className="form-group">
                  <label htmlFor="paymentMethod">Forma de Pagamento</label>
                  {isViewMode ? (
                    <input
                      type="text"
                      value={paymentMethods.find(p => p.value === formData.paymentMethod)?.label || ''}
                      disabled
                      className="form-control"
                    />
                  ) : (
                    <select
                      id="paymentMethod"
                      name="paymentMethod"
                      value={formData.paymentMethod}
                      onChange={handleChange}
                      className="form-control"
                    >
                      {paymentMethods.map(method => (
                        <option key={method.value} value={method.value}>
                          {method.label}
                        </option>
                      ))}
                    </select>
                  )}
                </div>

                <div className="form-group">
                  <label htmlFor="cashDiscount">Desconto à Vista (R$)</label>
                  <input
                    type="number"
                    id="cashDiscount"
                    name="cashDiscount"
                    value={formData.cashDiscount}
                    onChange={handleChange}
                    disabled={isViewMode}
                    min="0"
                    step="0.01"
                    className={`form-control ${errors.cashDiscount ? 'is-invalid' : ''}`}
                  />
                  {errors.cashDiscount && (
                    <div className="invalid-feedback">{errors.cashDiscount}</div>
                  )}
                </div>
              </div>

              <div className="form-group">
                <label htmlFor="paymentConditions">Condições de Pagamento</label>
                <textarea
                  id="paymentConditions"
                  name="paymentConditions"
                  value={formData.paymentConditions}
                  onChange={handleChange}
                  disabled={isViewMode}
                  rows="3"
                  maxLength="500"
                  className={`form-control ${errors.paymentConditions ? 'is-invalid' : ''}`}
                  placeholder="Ex: Entrada de 30% e restante em 3 parcelas"
                />
                {errors.paymentConditions && (
                  <div className="invalid-feedback">{errors.paymentConditions}</div>
                )}
              </div>
            </div>

            {/* Quote Custom Fields - HIDDEN FOR NOW */}
            {/* <div className="form-section">
              <h3>Campos Customizados do Orçamento</h3>
              
              {!isViewMode && (
                <div className="form-row">
                  <div className="form-group">
                    <input
                      type="text"
                      placeholder="Nome do campo (máx 50 caracteres)"
                      value={customFieldKey}
                      onChange={(e) => setCustomFieldKey(e.target.value)}
                      maxLength="50"
                      className="form-control"
                    />
                  </div>
                  <div className="form-group">
                    <input
                      type="text"
                      placeholder="Valor (máx 200 caracteres)"
                      value={customFieldValue}
                      onChange={(e) => setCustomFieldValue(e.target.value)}
                      maxLength="200"
                      className="form-control"
                    />
                  </div>
                  <button
                    type="button"
                    onClick={handleAddCustomField}
                    className="btn btn-secondary btn-sm"
                  >
                    <Icon name="plus" /> Adicionar
                  </button>
                </div>
              )}

              {Object.keys(formData.customFields).length > 0 && (
                <div className="custom-fields-display">
                  <div className="custom-fields-list">
                    {Object.entries(formData.customFields).map(([key, value]) => (
                      <div key={key} className="custom-field-item">
                        <span className="custom-field-key">{key}:</span>
                        <span className="custom-field-value">{value}</span>
                        {!isViewMode && (
                          <button
                            type="button"
                            onClick={() => handleRemoveCustomField(key)}
                            className="btn btn-danger btn-xs"
                          >
                            <Icon name="x" />
                          </button>
                        )}
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div> */}

            {/* Total */}
            <div className="form-section total-section">
              <div className="total-row">
                <span className="total-label">Subtotal:</span>
                <span className="total-value">
                  {formatCurrency(
                    formData.items.reduce((sum, item) => sum + (parseFloat(item.totalPrice) || 0), 0)
                  )}
                </span>
              </div>
              {formData.cashDiscount > 0 && (
                <div className="total-row">
                  <span className="total-label">Desconto:</span>
                  <span className="total-value discount">
                    - {formatCurrency(formData.cashDiscount)}
                  </span>
                </div>
              )}
              <div className="total-row grand-total">
                <span className="total-label">Total:</span>
                <span className="total-value">
                  {formatCurrency(calculateQuoteTotal())}
                </span>
              </div>
            </div>

            {/* Actions */}
            <div className="form-actions">
              {isViewMode ? (
                <>
                  <button
                    type="button"
                    onClick={handleEdit}
                    className="btn btn-primary"
                  >
                    <Icon name="edit" /> Editar
                  </button>
                  <button
                    type="button"
                    onClick={handleCancel}
                    className="btn btn-secondary"
                  >
                    Voltar
                  </button>
                </>
              ) : (
                <>
                  <button
                    type="submit"
                    disabled={loading}
                    className="btn btn-primary"
                  >
                    {loading ? (
                      <>
                        <span className="spinner-small"></span>
                        {isCreateMode ? 'Criando...' : 'Salvando...'}
                      </>
                    ) : (
                      <>
                        <Icon name="save" />
                        {isCreateMode ? 'Criar Orçamento' : 'Salvar Alterações'}
                      </>
                    )}
                  </button>
                  <button
                    type="button"
                    onClick={handleCancel}
                    disabled={loading}
                    className="btn btn-secondary"
                  >
                    Cancelar
                  </button>
                </>
              )}
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default QuoteDetails;
