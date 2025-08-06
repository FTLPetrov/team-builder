import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { adminService } from '../services/api/adminService';
import Card, { CardHeader, CardTitle, CardContent } from '../components/Card';
import Button from '../components/Button';

const AdminNotices = () => {
  const { isAdmin } = useAuth();
  const [announcements, setAnnouncements] = useState([]);
  const [warnings, setWarnings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState('announcements');

  useEffect(() => {
    if (!isAdmin) return;
    
    const fetchNotices = async () => {
      try {
        const [announcementsData, warningsData] = await Promise.all([
          adminService.getAllAnnouncements(),
          adminService.getAllWarnings()
        ]);

        setAnnouncements(announcementsData);
        setWarnings(warningsData);
      } catch (error) {
        console.error('Error fetching notices:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchNotices();
  }, [isAdmin]);

  const handleToggleAnnouncementActive = async (announcementId) => {
    try {
      await adminService.toggleAnnouncementActive(announcementId);

      const updatedAnnouncements = await adminService.getAllAnnouncements();
      setAnnouncements(updatedAnnouncements);
    } catch (error) {
      console.error('Error toggling announcement:', error);
      alert('Failed to toggle announcement status.');
    }
  };

  const handleDeleteAnnouncement = async (announcementId) => {
    if (!window.confirm('Are you sure you want to delete this announcement?')) {
      return;
    }

    try {
      await adminService.deleteAnnouncement(announcementId);

      const updatedAnnouncements = await adminService.getAllAnnouncements();
      setAnnouncements(updatedAnnouncements);
      alert('Announcement deleted successfully!');
    } catch (error) {
      console.error('Error deleting announcement:', error);
      alert('Failed to delete announcement.');
    }
  };

  const handleDeleteWarning = async (warningId) => {
    if (!window.confirm('Are you sure you want to delete this warning?')) {
      return;
    }

    try {
      await adminService.deleteWarning(warningId);

      const updatedWarnings = await adminService.getAllWarnings();
      setWarnings(updatedWarnings);
      alert('Warning deleted successfully!');
    } catch (error) {
      console.error('Error deleting warning:', error);
      alert('Failed to delete warning.');
    }
  };

  if (!isAdmin) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-gray-900 mb-4">Access Denied</h1>
          <p className="text-gray-600">You don't have permission to access the admin panel.</p>
        </div>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-red-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading admin notices...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Admin Notices</h1>
          <p className="text-gray-600 mt-2">Manage announcements and user warnings</p>
        </div>

        {/* Tab Navigation */}
        <div className="mb-6">
          <div className="border-b border-gray-200">
            <nav className="-mb-px flex space-x-8">
              <button
                onClick={() => setActiveTab('announcements')}
                className={`py-2 px-1 border-b-2 font-medium text-sm ${
                  activeTab === 'announcements'
                    ? 'border-yellow-500 text-yellow-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }`}
              >
                Announcements ({announcements.length})
              </button>
              <button
                onClick={() => setActiveTab('warnings')}
                className={`py-2 px-1 border-b-2 font-medium text-sm ${
                  activeTab === 'warnings'
                    ? 'border-red-500 text-red-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }`}
              >
                Warnings ({warnings.length})
              </button>
            </nav>
          </div>
        </div>

        {/* Announcements Tab */}
        {activeTab === 'announcements' && (
          <div className="space-y-6">
            <div className="flex justify-between items-center">
              <h2 className="text-xl font-semibold text-gray-900">Global Announcements</h2>
            </div>
            
            {announcements.length === 0 ? (
              <Card>
                <CardContent className="p-6 text-center">
                  <p className="text-gray-500">No announcements found.</p>
                </CardContent>
              </Card>
            ) : (
              <div className="space-y-4">
                {announcements.map((announcement) => (
                  <Card key={announcement.id}>
                    <CardContent className="p-6">
                      <div className="flex justify-between items-start">
                        <div className="flex-1">
                          <div className="flex items-center space-x-2 mb-2">
                            <h3 className="text-lg font-medium text-gray-900">
                              {announcement.title}
                            </h3>
                            <span
                              className={`px-2 py-1 text-xs font-medium rounded-full ${
                                announcement.isActive
                                  ? 'bg-green-100 text-green-800'
                                  : 'bg-gray-100 text-gray-800'
                              }`}
                            >
                              {announcement.isActive ? 'Active' : 'Inactive'}
                            </span>
                          </div>
                          <p className="text-gray-600 mb-4">{announcement.message}</p>
                          <div className="text-sm text-gray-500">
                            <p>Created by: {announcement.createdByUserName}</p>
                            <p>Created at: {new Date(announcement.createdAt).toLocaleString()}</p>
                          </div>
                        </div>
                        <div className="flex space-x-2 ml-4">
                          <Button
                            onClick={() => handleToggleAnnouncementActive(announcement.id)}
                            className={`text-sm ${
                              announcement.isActive
                                ? 'bg-gray-500 hover:bg-gray-600 text-white'
                                : 'bg-green-500 hover:bg-green-600 text-white'
                            }`}
                          >
                            {announcement.isActive ? 'Deactivate' : 'Activate'}
                          </Button>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </div>
            )}
          </div>
        )}

        {/* Warnings Tab */}
        {activeTab === 'warnings' && (
          <div className="space-y-6">
            <div className="flex justify-between items-center">
              <h2 className="text-xl font-semibold text-gray-900">User Warnings</h2>
            </div>
            
            {warnings.length === 0 ? (
              <Card>
                <CardContent className="p-6 text-center">
                  <p className="text-gray-500">No warnings found.</p>
                </CardContent>
              </Card>
            ) : (
              <div className="space-y-4">
                {warnings.map((warning) => (
                  <Card key={warning.id}>
                    <CardContent className="p-6">
                      <div className="flex justify-between items-start">
                        <div className="flex-1">
                          <div className="flex items-center space-x-2 mb-2">
                            <h3 className="text-lg font-medium text-gray-900">
                              Warning to: {warning.userName}
                            </h3>
                            <span className="px-2 py-1 text-xs font-medium rounded-full bg-red-100 text-red-800">
                              Warning
                            </span>
                          </div>
                          <p className="text-gray-600 mb-4">{warning.message}</p>
                          <div className="text-sm text-gray-500">
                            <p>User: {warning.userEmail}</p>
                            <p>Sent at: {new Date(warning.createdAt).toLocaleString()}</p>
                          </div>
                        </div>
                        <div className="flex space-x-2 ml-4">
                          <Button
                            onClick={() => handleDeleteWarning(warning.id)}
                            className="bg-red-500 hover:bg-red-600 text-white text-sm"
                          >
                            Delete
                          </Button>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default AdminNotices; 