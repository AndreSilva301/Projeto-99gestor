import React from 'react';

/**
 * Centralized Icon component for consistent icon rendering across the app
 * Uses CSS classes for Font Awesome icons to avoid React compatibility issues
 */
const Icon = ({ name, size = 20, className = '', style = {}, ...props }) => {
  // Map our custom icon names to Font Awesome CSS classes
  const iconMap = {
    // Dashboard and navigation icons
    'speedometer2': 'fas fa-tachometer-alt',
    'people': 'fas fa-users',
    'file-text': 'fas fa-file-alt',
    'person-badge': 'fas fa-id-badge',
    'building': 'fas fa-building',
    'gear': 'fas fa-cog',
    'box-arrow-right': 'fas fa-sign-out-alt',
    'clipboard-check': 'fas fa-clipboard-check',
    'person-circle': 'fas fa-user-circle',
    
    // Header and UI icons
    'list': 'fas fa-bars',
    'bell': 'fas fa-bell',
    
    // Dashboard stats icons (using same icons as non-fill versions)
    'people-fill': 'fas fa-users',
    'file-text-fill': 'fas fa-file-alt',
    'currency-dollar': 'fas fa-dollar-sign',
    'person-badge-fill': 'fas fa-id-badge',
    
    // Action and status icons
    'arrow-up': 'fas fa-arrow-up',
    'arrow-down': 'fas fa-arrow-down',
    'clock': 'fas fa-clock',
    'check-circle': 'fas fa-check-circle',
    'lightning-charge-fill': 'fas fa-bolt',
    'person-plus-fill': 'fas fa-user-plus',
    'file-plus-fill': 'fas fa-plus', // Using plus as file-plus doesn't exist
    'calendar-event': 'fas fa-calendar-alt',
    'gear-fill': 'fas fa-cog',
    
    // Additional common icons
    'home': 'fas fa-home',
    'search': 'fas fa-search',
    'edit': 'fas fa-edit',
    'delete': 'fas fa-trash',
    'save': 'fas fa-save',
    'cancel': 'fas fa-times',
    'info': 'fas fa-info-circle',
    'warning': 'fas fa-exclamation-triangle',
    'error': 'fas fa-times',
    'success': 'fas fa-check-circle',
    'phone': 'fas fa-phone',
    'email': 'fas fa-envelope',
    'location': 'fas fa-map-marker-alt',
    'calendar': 'fas fa-calendar',
    'document': 'fas fa-file',
    'folder': 'fas fa-folder',
    'image': 'fas fa-image',
    'video': 'fas fa-video',
    'music': 'fas fa-music',
    'attachment': 'fas fa-paperclip',
    'download': 'fas fa-download',
    'upload': 'fas fa-upload',
    'share': 'fas fa-share',
    'print': 'fas fa-print',
    'refresh': 'fas fa-sync',
    'settings': 'fas fa-cog',
    'help': 'fas fa-question-circle',
    'menu': 'fas fa-bars',
    'close': 'fas fa-times',
    'add': 'fas fa-plus',
    'remove': 'fas fa-minus',
    'filter': 'fas fa-filter',
    'sort': 'fas fa-sort',
    'star': 'fas fa-star',
    'heart': 'fas fa-heart',
    'thumbs-up': 'fas fa-thumbs-up',
    'thumbs-down': 'fas fa-thumbs-down'
  };
  
  const iconClass = iconMap[name];
  
  if (!iconClass) {
    console.warn(`Icon "${name}" not found`);
    return (
      <i 
        className={`fas fa-question-circle icon ${className}`}
        style={{ 
          fontSize: `${size}px`,
          display: 'inline-flex',
          alignItems: 'center',
          justifyContent: 'center',
          lineHeight: 1,
          ...style 
        }}
        title={`Unknown icon: ${name}`}
        {...props}
      />
    );
  }
  
  return (
    <i 
      className={`${iconClass} icon ${className}`}
      style={{ 
        fontSize: `${size}px`,
        display: 'inline-flex',
        alignItems: 'center',
        justifyContent: 'center',
        lineHeight: 1,
        ...style 
      }}
      title={name}
      {...props}
    />
  );
};

export default Icon;
