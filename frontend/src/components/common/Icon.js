import React from 'react';

/**
 * Centralized Icon component for consistent icon rendering across the app
 * Uses CSS classes for Font Awesome icons to avoid React compatibility issues
 */
const Icon = ({ name, size = 'md', className = '', style = {}, ...props }) => {
  // Convert size to appropriate values
  const getSizeValue = (size) => {
    if (typeof size === 'number') return `${size}px`;
    
    const sizeMap = {
      'sm': '14px',
      'md': '16px', 
      'lg': '20px',
      '3x': '48px'
    };
    
    return sizeMap[size] || size;
  };
  // Map our custom icon names to Font Awesome CSS classes
  const iconMap = {
    // Dashboard and navigation icons
    'speedometer2': 'fas fa-tachometer-alt',
    'people': 'fas fa-users',
    'file-text': 'fas fa-file-alt',
    'person-badge': 'fas fa-id-badge',
    'building': 'fas fa-building',
    'gear': 'fas fa-cog',
    'cog': 'fas fa-cog',
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
    'arrow-right': 'fas fa-arrow-right',
    'clock': 'fas fa-clock',
    'check-circle': 'fas fa-check-circle',
    'lightning-charge-fill': 'fas fa-bolt',
    'person-plus-fill': 'fas fa-user-plus',
    'file-plus-fill': 'fas fa-plus', // Using plus as file-plus doesn't exist
    'calendar-event': 'fas fa-calendar-alt',
    'gear-fill': 'fas fa-cog',
    
    // Customer and User Management Icons
    'users': 'fas fa-users',
    'user': 'fas fa-user',
    'user-plus': 'fas fa-user-plus',
    'user-check': 'fas fa-user-check',
    'user-xmark': 'fas fa-user-times',
    'user-edit': 'fas fa-user-edit',
    'plus': 'fas fa-plus',
    'eye': 'fas fa-eye',
    'trash': 'fas fa-trash',
    'times': 'fas fa-times',
    'x': 'fas fa-times',
    'save': 'fas fa-save',
    'check': 'fas fa-check',
    'envelope': 'fas fa-envelope',
    'location-dot': 'fas fa-map-marker-alt',
    'map-pin': 'fas fa-map-marker-alt',
    'triangle-exclamation': 'fas fa-exclamation-triangle',
    'alert-circle': 'fas fa-exclamation-circle',
    'chevron-left': 'fas fa-chevron-left',
    'chevron-right': 'fas fa-chevron-right',
    'inbox': 'fas fa-inbox',
    'comments': 'fas fa-comments',
    'arrow-left': 'fas fa-arrow-left',
    
    // Security and other icons
    'shield-check': 'fas fa-shield-alt',
    'link': 'fas fa-link',
    'card-text': 'fas fa-id-card',
    'refresh-cw': 'fas fa-sync-alt',
    
    // Additional common icons
    'home': 'fas fa-home',
    'search': 'fas fa-search',
    'edit': 'fas fa-edit',
    'delete': 'fas fa-trash',
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
          fontSize: getSizeValue(size),
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
        fontSize: getSizeValue(size),
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
