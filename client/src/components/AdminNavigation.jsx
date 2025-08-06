import { Link, useLocation } from 'react-router-dom';
import ProfileDropdown from './ProfileDropdown';

const AdminNavigation = () => {
  const location = useLocation();

  const isActive = (path) => {
    return location.pathname === path;
  };

  return (
    <nav className="bg-red-600 shadow-lg border-b border-red-700">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16">
          <div className="flex items-center">
            <Link to="/admin" className="flex-shrink-0 flex items-center">
              <span className="text-2xl font-bold text-white">Admin Panel</span>
            </Link>
            
            <div className="ml-10 flex items-baseline space-x-4">
              <Link
                to="/admin/users"
                className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                  isActive('/admin/users')
                    ? 'bg-red-700 text-white'
                    : 'text-red-100 hover:text-white hover:bg-red-700'
                }`}
              >
                All Users
              </Link>
              <Link
                to="/admin/teams"
                className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                  isActive('/admin/teams')
                    ? 'bg-red-700 text-white'
                    : 'text-red-100 hover:text-white hover:bg-red-700'
                }`}
              >
                All Teams
              </Link>
              <Link
                to="/admin/events"
                className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                  isActive('/admin/events')
                    ? 'bg-red-700 text-white'
                    : 'text-red-100 hover:text-white hover:bg-red-700'
                }`}
              >
                All Events
              </Link>
              <Link
                to="/admin/support"
                className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                  isActive('/admin/support')
                    ? 'bg-red-700 text-white'
                    : 'text-red-100 hover:text-white hover:bg-red-700'
                }`}
              >
                Support Messages
              </Link>
              <Link
                to="/admin/notices"
                className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                  isActive('/admin/notices')
                    ? 'bg-red-700 text-white'
                    : 'text-red-100 hover:text-white hover:bg-red-700'
                }`}
              >
                Admin Notices
              </Link>
            </div>
          </div>

          <div className="flex items-center">
            <ProfileDropdown />
          </div>
        </div>
      </div>
    </nav>
  );
};

export default AdminNavigation; 