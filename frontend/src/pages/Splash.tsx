import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './Splash.css';

export function Splash() {
  const navigate = useNavigate();

  useEffect(() => {
    const timer = setTimeout(() => navigate('/boas-vindas'), 1400);
    return () => clearTimeout(timer);
  }, [navigate]);

  return (
    <div className="splash">
      <div className="splash__logo">
        <svg width="72" height="72" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <rect x="3" y="4" width="18" height="12" rx="4" fill="#1e4d94" />
          <circle cx="7.5" cy="18.5" r="1.6" fill="#1e4d94" />
          <circle cx="16.5" cy="18.5" r="1.6" fill="#1e4d94" />
          <path d="M6 8h12M6 11.5h8" stroke="#fff" strokeWidth="1.4" strokeLinecap="round" />
          <path d="M17 3.5c1.4.4 2.4 1.5 2.7 3" stroke="#2fa360" strokeWidth="1.4" strokeLinecap="round" />
        </svg>
      </div>
      <h1>SmartBus</h1>
      <p>Santo André</p>
    </div>
  );
}
