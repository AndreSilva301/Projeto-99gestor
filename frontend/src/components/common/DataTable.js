import React, { useState } from 'react';
import Icon from '../common/Icon';

const DataTable = ({ 
  data = [], 
  columns = [], 
  loading = false, 
  onEdit = null, 
  onView = null, 
  onDelete = null,
  pagination = null,
  onPageChange = null,
  searchValue = '',
  onSearchChange = null,
  searchPlaceholder = 'Pesquisar...',
  emptyMessage = 'Nenhum item encontrado',
  className = ''
}) => {
  const [itemToDelete, setItemToDelete] = useState(null);

  const handleDeleteClick = (item) => {
    setItemToDelete(item);
  };

  const confirmDelete = () => {
    if (onDelete && itemToDelete) {
      onDelete(itemToDelete);
    }
    setItemToDelete(null);
  };

  const cancelDelete = () => {
    setItemToDelete(null);
  };

  const renderPagination = () => {
    if (!pagination || pagination.totalPages <= 1) return null;

    const pages = [];
    const currentPage = pagination.currentPage;
    const totalPages = pagination.totalPages;

    // First page
    if (currentPage > 2) {
      pages.push(
        <button
          key={1}
          className="btn btn-outline-primary btn-sm"
          onClick={() => onPageChange(1)}
        >
          1
        </button>
      );
      if (currentPage > 3) {
        pages.push(<span key="dots1" className="px-2">...</span>);
      }
    }

    // Pages around current
    for (let i = Math.max(1, currentPage - 1); i <= Math.min(totalPages, currentPage + 1); i++) {
      pages.push(
        <button
          key={i}
          className={`btn btn-sm ${i === currentPage ? 'btn-primary' : 'btn-outline-primary'}`}
          onClick={() => onPageChange(i)}
        >
          {i}
        </button>
      );
    }

    // Last page
    if (currentPage < totalPages - 1) {
      if (currentPage < totalPages - 2) {
        pages.push(<span key="dots2" className="px-2">...</span>);
      }
      pages.push(
        <button
          key={totalPages}
          className="btn btn-outline-primary btn-sm"
          onClick={() => onPageChange(totalPages)}
        >
          {totalPages}
        </button>
      );
    }

    return (
      <div className="d-flex justify-content-between align-items-center mt-3">
        <div className="text-muted">
          Mostrando {((currentPage - 1) * pagination.itemsPerPage) + 1} a{' '}
          {Math.min(currentPage * pagination.itemsPerPage, pagination.totalItems)} de{' '}
          {pagination.totalItems} itens
        </div>
        <div className="d-flex gap-2 align-items-center">
          <button
            className="btn btn-outline-primary btn-sm"
            disabled={!pagination.hasPrev}
            onClick={() => onPageChange(currentPage - 1)}
          >
            <Icon name="chevron-left" size="sm" />
          </button>
          {pages}
          <button
            className="btn btn-outline-primary btn-sm"
            disabled={!pagination.hasNext}
            onClick={() => onPageChange(currentPage + 1)}
          >
            <Icon name="chevron-right" size="sm" />
          </button>
        </div>
      </div>
    );
  };

  return (
    <div className={`data-table ${className}`}>
      {/* Search Bar */}
      {onSearchChange && (
        <div className="row mb-3">
          <div className="col-md-6">
            <div className="input-group">
              <span className="input-group-text">
                <Icon name="search" size="sm" />
              </span>
              <input
                type="text"
                className="form-control"
                placeholder={searchPlaceholder}
                value={searchValue}
                onChange={(e) => onSearchChange(e.target.value)}
              />
            </div>
          </div>
        </div>
      )}

      {/* Table */}
      <div className="table-responsive">
        <table className="table table-hover">
          <thead className="table-light">
            <tr>
              {columns.map((column, index) => (
                <th key={index} style={{ width: column.width }}>
                  {column.header}
                </th>
              ))}
              {(onView || onEdit || onDelete) && (
                <th width="120">
                  Ações
                </th>
              )}
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan={columns.length + (onView || onEdit || onDelete ? 1 : 0)} className="text-center py-4">
                  <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Carregando...</span>
                  </div>
                </td>
              </tr>
            ) : data.length === 0 ? (
              <tr>
                <td colSpan={columns.length + (onView || onEdit || onDelete ? 1 : 0)} className="text-center py-4 text-muted">
                  <Icon name="inbox" size="lg" className="mb-2 d-block mx-auto" />
                  {emptyMessage}
                </td>
              </tr>
            ) : (
              data.map((item, rowIndex) => (
                <tr key={rowIndex}>
                  {columns.map((column, colIndex) => (
                    <td key={colIndex}>
                      {column.render ? column.render(item, rowIndex) : item[column.accessor]}
                    </td>
                  ))}
                  {(onView || onEdit || onDelete) && (
                    <td>
                      <div className="btn-group btn-group-sm">
                        {onView && (
                          <button
                            type="button"
                            className="btn btn-outline-info btn-sm"
                            onClick={() => onView(item)}
                            title="Visualizar"
                          >
                            <Icon name="eye" size="sm" />
                          </button>
                        )}
                        {onEdit && (
                          <button
                            type="button"
                            className="btn btn-outline-primary btn-sm"
                            onClick={() => onEdit(item)}
                            title="Editar"
                          >
                            <Icon name="edit" size="sm" />
                          </button>
                        )}
                        {onDelete && (
                          <button
                            type="button"
                            className="btn btn-outline-danger btn-sm"
                            onClick={() => handleDeleteClick(item)}
                            title="Excluir"
                          >
                            <Icon name="trash" size="sm" />
                          </button>
                        )}
                      </div>
                    </td>
                  )}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* Pagination */}
      {renderPagination()}

      {/* Delete Confirmation Modal */}
      {itemToDelete && (
        <div className="modal d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">
                  <Icon name="triangle-exclamation" className="text-warning me-2" />
                  Confirmar Exclusão
                </h5>
              </div>
              <div className="modal-body">
                <p>Tem certeza que deseja excluir este item?</p>
                <p className="text-muted small">Esta ação não pode ser desfeita.</p>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn-secondary" onClick={cancelDelete}>
                  Cancelar
                </button>
                <button type="button" className="btn btn-danger" onClick={confirmDelete}>
                  <Icon name="trash" size="sm" className="me-1" />
                  Excluir
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default DataTable;
