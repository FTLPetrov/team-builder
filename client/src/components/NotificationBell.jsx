import { useState, useEffect } from 'react';
import { announcementService } from '../services/api/announcementService';
import { warningService } from '../services/api/warningService';
import { useAuth } from '../contexts/AuthContext';
import { safeText } from '../utils/escapeHtml';

const NotificationBell = () => {
  const { isAuthenticated } = useAuth();
  const [announcements, setAnnouncements] = useState([]);
  const [warnings, setWarnings] = useState([]);
  const [showDropdown, setShowDropdown] = useState(false);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (isAuthenticated) {
      fetchNotifications();
    }
  }, [isAuthenticated]);

  const fetchNotifications = async () => {
    setLoading(true);
    try {
      const [announcementsData, warningsData] = await Promise.all([
        announcementService.getActiveAnnouncements(),
        warningService.getUserWarnings()
      ]);
      setAnnouncements(announcementsData);
      setWarnings(warningsData);
    } catch (error) {
      console.error('Error fetching notifications:', error);
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  if (!isAuthenticated) {
    return null;
  }

  return (
    <div className="relative">
             <button
         onClick={() => setShowDropdown(!showDropdown)}
         className="relative p-2 text-yellow-600 hover:text-yellow-700 focus:outline-none focus:ring-2 focus:ring-yellow-500 focus:ring-offset-2 rounded-full transition-colors duration-200"
       >
         <svg className="w-7 h-7" fill="currentColor" viewBox="0 0 24 24">
           <path d="M12 22c1.1 0 2-.9 2-2h-4c0 1.1.89 2 2 2zm6-6v-5c0-3.07-1.64-5.64-4.5-6.32V4c0-.83-.67-1.5-1.5-1.5s-1.5.67-1.5 1.5v.68C7.63 5.36 6 7.92 6 11v5l-2 2v1h16v-1l-2-2z"/>
         </svg>
        
                 {(announcements.length > 0 || warnings.length > 0) && (
           <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full h-6 w-6 flex items-center justify-center font-bold shadow-lg">
             {announcements.length + warnings.length}
           </span>
         )}
      </button>

      {showDropdown && (
        <div className="absolute right-0 mt-2 w-80 bg-white rounded-md shadow-lg ring-1 ring-black ring-opacity-5 z-50">
          <div className="py-1">
            <div className="px-4 py-2 border-b border-blue-200 bg-blue-50">
              <h3 className="text-sm font-medium text-blue-900">Notifications</h3>
            </div>
            
            {loading ? (
              <div className="px-4 py-3 text-sm text-gray-500">
                Loading notifications...
              </div>
            ) : announcements.length === 0 && warnings.length === 0 ? (
              <div className="px-4 py-3 text-sm text-gray-500">
                No notifications
              </div>
            ) : (
              <div className="max-h-64 overflow-y-auto">
                {/* Warnings Section */}
                {warnings.length > 0 && (
                  <>
                    <div className="px-4 py-2 bg-red-50 border-b border-red-200">
                      <h4 className="text-sm font-medium text-red-800">⚠️ Warnings</h4>
                    </div>
                    {warnings.map((warning) => (
                      <div key={warning.id} className="px-4 py-3 border-b border-gray-100 last:border-b-0 bg-red-50">
                        <div className="flex items-start justify-between">
                          <div className="flex-1">
                            <h4 className="text-sm font-medium text-red-900 mb-1">
                              Warning from Admin
                            </h4>
                            <p className="text-sm text-red-700 mb-2 line-clamp-3">
                              {safeText(warning.message)}
                            </p>
                            <div className="flex items-center justify-between text-xs text-red-600">
                              <span>By {warning.createdByUserName}</span>
                              <span>{formatDate(warning.createdAt)}</span>
                            </div>
                          </div>
                        </div>
                      </div>
                    ))}
                  </>
                )}

                {/* Announcements Section */}
                {announcements.length > 0 && (
                  <>
                    <div className="px-4 py-2 bg-blue-50 border-b border-blue-200">
                      <h4 className="text-sm font-medium text-blue-800">📢 Announcements</h4>
                    </div>
                    {announcements.map((announcement) => (
                      <div key={announcement.id} className="px-4 py-3 border-b border-gray-100 last:border-b-0 bg-blue-50">
                        <div className="flex items-start justify-between">
                          <div className="flex-1">
                            <h4 className="text-sm font-medium text-blue-900 mb-1">
                              {safeText(announcement.title)}
                            </h4>
                            <p className="text-sm text-blue-700 mb-2 line-clamp-3">
                              {safeText(announcement.message)}
                            </p>
                            <div className="flex items-center justify-between text-xs text-blue-600">
                              <span>By {announcement.createdByUserName}</span>
                              <span>{formatDate(announcement.createdAt)}</span>
                            </div>
                          </div>
                        </div>
                      </div>
                    ))}
                  </>
                )}
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default NotificationBell; 