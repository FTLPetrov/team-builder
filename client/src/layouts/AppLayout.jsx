import Navigation from '../components/Navigation';
import AdminNavigation from '../components/AdminNavigation';
import Footer from '../components/Footer';
import { useAuth } from '../contexts/AuthContext';
import { useLocation } from 'react-router-dom';

const AppLayout = ({ children }) => {
  const { isAdmin } = useAuth();
  const location = useLocation();

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col">
      {isAdmin ? <AdminNavigation /> : <Navigation />}
      <main className="flex-1">
        {children}
      </main>
      <Footer />
    </div>
  );
};

export default AppLayout; 