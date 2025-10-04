import React, { useState, useEffect } from 'react';
import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';
import Header from './Header';
import { HeaderProvider } from '../../contexts/HeaderContext';
import '../../styles/portal.css';

const PortalLayout = ({ 
  userRole = 'Administrator' 
}) => {
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  const [isMobile, setIsMobile] = useState(false);

  // Handle responsive behavior
  useEffect(() => {
    const handleResize = () => {
      const mobile = window.innerWidth <= 768;
      setIsMobile(mobile);
      
      // Auto-close sidebar on mobile when resizing to desktop
      if (!mobile && isSidebarOpen) {
        setIsSidebarOpen(false);
      }
    };

    // Initial check
    handleResize();

    // Add event listener
    window.addEventListener('resize', handleResize);

    // Cleanup
    return () => window.removeEventListener('resize', handleResize);
  }, [isSidebarOpen]);

  // Handle sidebar toggle
  const handleToggleSidebar = () => {
    setIsSidebarOpen(!isSidebarOpen);
  };

  // Handle sidebar close
  const handleCloseSidebar = () => {
    setIsSidebarOpen(false);
  };

  // Handle escape key to close sidebar on mobile
  useEffect(() => {
    const handleEscape = (event) => {
      if (event.key === 'Escape' && isMobile && isSidebarOpen) {
        handleCloseSidebar();
      }
    };

    document.addEventListener('keydown', handleEscape);
    return () => document.removeEventListener('keydown', handleEscape);
  }, [isMobile, isSidebarOpen]);

  return (
    <HeaderProvider>
      <div className="portal-layout">
        {/* Sidebar */}
        <Sidebar 
          isOpen={isMobile ? isSidebarOpen : true}
          onClose={handleCloseSidebar}
          userRole={userRole}
        />

        {/* Main content area */}
        <div className="portal-content">
          {/* Header */}
          <Header 
            onToggleSidebar={handleToggleSidebar}
          />

          {/* Main content */}
          <main className="portal-main">
            <Outlet />
          </main>
        </div>
      </div>
    </HeaderProvider>
  );
};

export default PortalLayout;
