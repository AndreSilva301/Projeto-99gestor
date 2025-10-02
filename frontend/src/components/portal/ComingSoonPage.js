import React from 'react';

const ComingSoonPage = ({ title, description, features = [] }) => (
  <div style={{ 
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    minHeight: '400px',
    textAlign: 'center',
    padding: '40px 20px'
  }}>
    <div style={{ fontSize: '4rem', marginBottom: '20px' }}>🚧</div>
    <h1 style={{ 
      fontSize: '2rem', 
      margin: '0 0 16px 0',
      color: 'var(--portal-dark)'
    }}>
      {title}
    </h1>
    <p style={{ 
      fontSize: '1.125rem',
      color: 'var(--portal-secondary)',
      margin: '0 0 32px 0',
      maxWidth: '600px',
      lineHeight: '1.6'
    }}>
      {description}
    </p>
    
    {features.length > 0 && (
      <div style={{
        background: 'white',
        padding: '24px',
        borderRadius: 'var(--portal-border-radius)',
        boxShadow: 'var(--portal-shadow)',
        maxWidth: '500px',
        width: '100%'
      }}>
        <h3 style={{ 
          margin: '0 0 16px 0',
          color: 'var(--portal-primary)'
        }}>
          Planned Features:
        </h3>
        <ul style={{ 
          listStyle: 'none',
          padding: 0,
          margin: 0,
          textAlign: 'left'
        }}>
          {features.map((feature, index) => (
            <li key={index} style={{ 
              padding: '8px 0',
              borderBottom: index < features.length - 1 ? '1px solid #e9ecef' : 'none'
            }}>
              ✅ {feature}
            </li>
          ))}
        </ul>
      </div>
    )}
  </div>
);

export default ComingSoonPage;
