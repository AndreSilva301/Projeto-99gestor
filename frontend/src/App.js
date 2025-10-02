import { Routes, Route } from 'react-router-dom';
import Layout from './components/layout/Layout';
import Home from './pages/Home';
import Services from './pages/Services';
import Contact from './pages/Contact';
import Login from './pages/Auth/Login';
import Register from './pages/Auth/Register';
import ForgotPassword from './pages/Auth/ForgotPassword';
import ResetPassword from './pages/Auth/ResetPassword';
import PortalRouter from './routes/PortalRouter';
import './styles/globals.css';
import './styles/portal.css';
import './App.css';

function App() {
  return (
    <div className="App">
      <Routes>
        {/* Public routes with main layout */}
        <Route path="/" element={<Layout />}>
          <Route index element={<Home />} />
          <Route path="services" element={<Services />} />
          <Route path="contact" element={<Contact />} />
          <Route path="login" element={<Login />} />
          <Route path="register" element={<Register />} />
          <Route path="forgot-password" element={<ForgotPassword />} />
          <Route path="reset-password" element={<ResetPassword />} />
        </Route>
        
        {/* Portal routes with portal layout */}
        <Route path="/portal/*" element={<PortalRouter />} />
      </Routes>
    </div>
  );
}

export default App;
