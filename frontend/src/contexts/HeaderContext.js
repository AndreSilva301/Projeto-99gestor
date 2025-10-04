import React, { createContext, useContext, useState } from 'react';

const HeaderContext = createContext();

export const useHeader = () => {
  const context = useContext(HeaderContext);
  if (!context) {
    throw new Error('useHeader must be used within HeaderProvider');
  }
  return context;
};

export const HeaderProvider = ({ children }) => {
  const [headerData, setHeaderData] = useState({
    title: 'Dashboard',
    subtitle: 'Bem-vindo ao seu painel de controle CRM'
  });

  const updateHeader = (title, subtitle) => {
    setHeaderData({ title, subtitle });
  };

  return (
    <HeaderContext.Provider value={{ headerData, updateHeader }}>
      {children}
    </HeaderContext.Provider>
  );
};
