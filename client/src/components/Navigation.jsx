import { useAuth } from '../contexts/AuthContext';
import { Link, useLocation } from 'react-router-dom';
import { useState, useEffect } from 'react';
import ProfileDropdown from './ProfileDropdown';
import NotificationBell from './NotificationBell';
import { invitationService } from '../services/api/invitationService';

const Navigation = () => {
  const { isAuthenticated, isAdmin } = useAuth();
  const location = useLocation();
  const [invitationCount, setInvitationCount] = useState(0);

  useEffect(() => {
    if (isAuthenticated) {
      fetchInvitationCount();
    }
  }, [isAuthenticated]);

  useEffect(() => {
    const handleInvitationCountChanged = () => {
      fetchInvitationCount();
    };

    window.addEventListener('invitationCountChanged', handleInvitationCountChanged);
    return () => {
      window.removeEventListener('invitationCountChanged', handleInvitationCountChanged);
    };
  }, []);

  const fetchInvitationCount = async () => {
    try {
      const invitations = await invitationService.getUserInvitations();
      setInvitationCount(invitations.length);
    } catch (error) {
      console.error('Error fetching invitation count:', error);
    }
  };

  const isActive = (path) => {
    return location.pathname === path;
  };


  if (isAdmin && location.pathname.startsWith('/admin')) {
    return null; // AdminNavigation will be rendered by the layout
  }

  return (
    <nav className="bg-white shadow-lg border-b border-gray-200">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16">
          <div className="flex items-center">
            <Link to="/" className="flex-shrink-0 flex items-center">
              <span className="text-2xl font-bold text-blue-600">TeamBuilder</span>
            </Link>
            
            {isAuthenticated && (
              <div className="ml-10 flex items-baseline space-x-4">
                <Link
                  to="/dashboard"
                  className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                    isActive('/dashboard')
                      ? 'bg-blue-100 text-blue-700'
                      : 'text-gray-700 hover:text-blue-600 hover:bg-gray-50'
                  }`}
                >
                  Dashboard
                </Link>
                <Link
                  to="/teams"
                  className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                    isActive('/teams')
                      ? 'bg-blue-100 text-blue-700'
                      : 'text-gray-700 hover:text-blue-600 hover:bg-gray-50'
                  }`}
                >
                  Teams
                </Link>
                <Link
                  to="/events"
                  className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                    isActive('/events')
                      ? 'bg-blue-100 text-blue-700'
                      : 'text-gray-700 hover:text-blue-600 hover:bg-gray-50'
                  }`}
                >
                  Events
                </Link>
                {!isAdmin && (
                  <Link
                    to="/about"
                    className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                      isActive('/about')
                        ? 'bg-blue-100 text-blue-700'
                        : 'text-gray-700 hover:text-blue-600 hover:bg-gray-50'
                    }`}
                  >
                    About Us
                  </Link>
                )}
                {!isAdmin && (
                  <Link
                    to="/contact"
                    className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                      isActive('/contact')
                        ? 'bg-blue-100 text-blue-700'
                        : 'text-gray-700 hover:text-blue-600 hover:bg-gray-50'
                    }`}
                  >
                    Contact Support
                  </Link>
                )}
                {isAdmin && (
                  <Link
                    to="/admin"
                    className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                      isActive('/admin')
                        ? 'bg-red-100 text-red-700'
                        : 'text-red-600 hover:text-red-700 hover:bg-red-50'
                    }`}
                  >
                    Admin Panel
                  </Link>
                )}
              </div>
            )}
          </div>

          <div className="flex items-center space-x-4">
            {isAuthenticated && <NotificationBell />}
            {isAuthenticated ? (
              <ProfileDropdown />
            ) : (
              <div className="flex items-center space-x-4">
                <Link
                  to="/login"
                  className="px-4 py-2 text-sm font-medium text-gray-700 hover:text-blue-600 transition-colors"
                >
                  Login
                </Link>
                <Link
                  to="/register"
                  className="px-4 py-2 text-sm font-medium bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors"
                >
                  Register
                </Link>
              </div>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Navigation; 