import React from 'react';
import ComingSoonPage from '../../components/portal/ComingSoonPage';

const Quotes = () => {
  return (
    <ComingSoonPage
      title="Quote Management"
      description="Create detailed quotes for your customers with automatic calculations, export to PDF/Image, and track approval status."
      features={[
        'Create quotes with detailed items',
        'Automatic value calculations',
        'Manual total value entry option',
        'Payment conditions and cash discounts',
        'Export to PDF and Image formats',
        'Configurable additional fields',
        'Quote approval tracking'
      ]}
    />
  );
};

export default Quotes;
