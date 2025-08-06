import { useState, useEffect } from "react";
import { useAuth } from '../contexts/AuthContext';
import { adminService } from '../services/api/adminService';
import Card, { CardHeader, CardTitle, CardContent } from '../components/Card';
import Button from '../components/Button';
import Notification from '../components/Notification';

const AdminUsers = () => {
  const { user } = useAuth();
  const [users, setUsers] = useState([]);
  const [filteredUsers, setFilteredUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [notification, setNotification] = useState(null);
  const [selectedUser, setSelectedUser] = useState(null);
  const [showPasswordModal, setShowPasswordModal] = useState(false);
  const [showWarningModal, setShowWarningModal] = useState(false);
  const [showUserDetailsModal, setShowUserDetailsModal] = useState(false);
  const [newPassword, setNewPassword] = useState('');
  const [warningMessage, setWarningMessage] = useState('');
  

  const [searchTerm, setSearchTerm] = useState('');
  const [roleFilter, setRoleFilter] = useState('all');
  const [statusFilter, setStatusFilter] = useState('all');
  const [sortBy, setSortBy] = useState('name');

  useEffect(() => {
    fetchUsers();
  }, []);

  useEffect(() => {
    filterAndSortUsers();
  }, [users, searchTerm, roleFilter, statusFilter, sortBy]);

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const data = await adminService.getAllUsers();
      setUsers(data);
    } catch (error) {
      console.error('Error fetching users:', error);
      setNotification({
        message: 'Failed to fetch users',
        type: 'error'
      });
    } finally {
      setLoading(false);
    }
  };

  const filterAndSortUsers = () => {
    let filtered = users.filter(user => {
      const matchesSearch = 
        user.firstName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        user.lastName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        user.email?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        user.userName?.toLowerCase().includes(searchTerm.toLowerCase());
      
      const matchesRole = roleFilter === 'all' || 
        (roleFilter === 'admin' && user.isAdmin) ||
        (roleFilter === 'user' && !user.isAdmin);
      
      const matchesStatus = statusFilter === 'all' ||
        (statusFilter === 'active' && !user.isDeleted) ||
        (statusFilter === 'deleted' && user.isDeleted);

      return matchesSearch && matchesRole && matchesStatus;
    });


    filtered.sort((a, b) => {
      switch (sortBy) {
        case 'name':
          return `${a.firstName} ${a.lastName}`.localeCompare(`${b.firstName} ${b.lastName}`);
        case 'email':
          return (a.email || '').localeCompare(b.email || '');
        case 'role':
          return (a.isAdmin ? 1 : 0) - (b.isAdmin ? 1 : 0);
        case 'status':
          return (a.isDeleted ? 1 : 0) - (b.isDeleted ? 1 : 0);
        default:
          return 0;
      }
    });

    setFilteredUsers(filtered);
  };

  const handleDeleteUser = async (userId) => {
    if (!window.confirm('Are you sure you want to delete this user? This action cannot be undone.')) {
      return;
    }

    try {
      await adminService.deleteUser(userId);
      setUsers(users.filter(user => user.id !== userId));
      setNotification({
        message: 'User deleted successfully',
        type: 'success'
      });
    } catch (error) {
      console.error('Error deleting user:', error);
      setNotification({
        message: 'Failed to delete user',
        type: 'error'
      });
    }
  };

  const handleRecoverUser = async (userId) => {
    try {
      await adminService.recoverUser(userId);
      fetchUsers(); // Refresh the list
      setNotification({
        message: 'User recovered successfully',
        type: 'success'
      });
    } catch (error) {
      console.error('Error recovering user:', error);
      setNotification({
        message: 'Failed to recover user',
        type: 'error'
      });
    }
  };

  const handleUpdatePassword = async () => {
    if (!selectedUser || !newPassword.trim()) {
      setNotification({
        message: 'Please enter a new password',
        type: 'error'
      });
      return;
    }

    try {
      await adminService.updateUserPassword(selectedUser.id, newPassword);
      setShowPasswordModal(false);
      setNewPassword('');
      setSelectedUser(null);
      setNotification({
        message: 'Password updated successfully',
        type: 'success'
      });
    } catch (error) {
      console.error('Error updating password:', error);
      setNotification({
        message: 'Failed to update password',
        type: 'error'
      });
    }
  };

  const handleWarnUser = async () => {
    if (!selectedUser || !warningMessage.trim()) {
      setNotification({
        message: 'Please enter a warning message',
        type: 'error'
      });
      return;
    }

    try {
      await adminService.warnUser(selectedUser.id, warningMessage);
      setShowWarningModal(false);
      setWarningMessage('');
      setSelectedUser(null);
      setNotification({
        message: 'Warning sent successfully',
        type: 'success'
      });
    } catch (error) {
      console.error('Error sending warning:', error);
      setNotification({
        message: 'Failed to send warning',
        type: 'error'
      });
    }
  };

  const clearNotification = () => {
    setNotification(null);
  };

  const getUserDisplayName = (user) => {
    if (user.firstName && user.lastName) {
      return `${user.firstName} ${user.lastName}`;
    }
    return user.userName || user.email || 'Unknown User';
  };

  const getUserInitials = (user) => {
    if (user.firstName && user.lastName) {
      return `${user.firstName.charAt(0)}${user.lastName.charAt(0)}`.toUpperCase();
    }
    if (user.firstName) {
      return user.firstName.charAt(0).toUpperCase();
    }
    if (user.userName) {
      return user.userName.charAt(0).toUpperCase();
    }
    return user.email?.charAt(0).toUpperCase() || 'U';
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading users...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {notification && (
        <Notification
          message={notification.message}
          type={notification.type}
          onClose={clearNotification}
        />
      )}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">User Management</h1>
          <p className="text-gray-600 mt-2">Manage all users in the system</p>
        </div>

        {/* Search and Filter Controls */}
        <Card className="mb-6">
          <CardContent className="p-6">
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
              {/* Search */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Search Users
                </label>
                <input
                  type="text"
                  placeholder="Search by name, email, or username..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                />
              </div>

              {/* Role Filter */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Role
                </label>
                <select
                  value={roleFilter}
                  onChange={(e) => setRoleFilter(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                >
                  <option value="all">All Roles</option>
                  <option value="admin">Admin</option>
                  <option value="user">User</option>
                </select>
              </div>

              {/* Status Filter */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Status
                </label>
                <select
                  value={statusFilter}
                  onChange={(e) => setStatusFilter(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                >
                  <option value="all">All Status</option>
                  <option value="active">Active</option>
                  <option value="deleted">Deleted</option>
                </select>
              </div>

              {/* Sort By */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Sort By
                </label>
                <select
                  value={sortBy}
                  onChange={(e) => setSortBy(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                >
                  <option value="name">Name</option>
                  <option value="email">Email</option>
                  <option value="role">Role</option>
                  <option value="status">Status</option>
                </select>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Results Summary */}
        <div className="mb-4">
          <p className="text-sm text-gray-600">
            Showing {filteredUsers.length} of {users.length} users
          </p>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>All Users ({filteredUsers.length})</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      User
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Contact Info
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Role
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Status
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Actions
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {filteredUsers.map((user) => (
                    <tr key={user.id} className="hover:bg-gray-50">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center">
                          <div className="w-10 h-10 bg-blue-500 rounded-full flex items-center justify-center overflow-hidden">
                            {user.profilePictureUrl ? (
                              <img 
                                src={`${import.meta.env.VITE_BACKEND_URL || 'http://localhost:5000'}${user.profilePictureUrl}`}
                                alt="Profile" 
                                className="w-full h-full object-cover"
                              />
                            ) : (
                              <span className="text-white text-sm font-medium">
                                {getUserInitials(user)}
                              </span>
                            )}
                          </div>
                          <div className="ml-4">
                            <div className="text-sm font-medium text-gray-900">
                              {getUserDisplayName(user)}
                            </div>
                            <div className="text-sm text-gray-500">
                              @{user.userName}
                            </div>
                          </div>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm text-gray-900">{user.email}</div>
                        <div className="text-sm text-gray-500">
                          {user.emailConfirmed ? '✓ Verified' : '⚠ Not verified'}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                          user.isAdmin 
                            ? 'bg-red-100 text-red-800' 
                            : 'bg-green-100 text-green-800'
                        }`}>
                          {user.isAdmin ? 'Admin' : 'User'}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                          user.isDeleted 
                            ? 'bg-red-100 text-red-800' 
                            : 'bg-green-100 text-green-800'
                        }`}>
                          {user.isDeleted ? 'Deleted' : 'Active'}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                        <div className="flex space-x-2">
                          <Button
                            size="small"
                            variant="outline"
                            onClick={() => {
                              setSelectedUser(user);
                              setShowUserDetailsModal(true);
                            }}
                          >
                            Details
                          </Button>
                          <Button
                            size="small"
                            variant="outline"
                            onClick={() => {
                              setSelectedUser(user);
                              setShowPasswordModal(true);
                            }}
                          >
                            Password
                          </Button>
                          <Button
                            size="small"
                            variant="outline"
                            onClick={() => {
                              setSelectedUser(user);
                              setShowWarningModal(true);
                            }}
                          >
                            Warn
                          </Button>
                          {user.isDeleted ? (
                            <Button
                              size="small"
                              variant="success"
                              onClick={() => handleRecoverUser(user.id)}
                            >
                              Recover
                            </Button>
                          ) : (
                            <Button
                              size="small"
                              variant="danger"
                              onClick={() => handleDeleteUser(user.id)}
                            >
                              Delete
                            </Button>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
              
              {filteredUsers.length === 0 && (
                <div className="text-center py-8">
                  <p className="text-gray-500">No users found matching your criteria.</p>
                </div>
              )}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Password Change Modal */}
      {showPasswordModal && selectedUser && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
          <div className="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
            <div className="mt-3">
              <h3 className="text-lg font-medium text-gray-900 mb-4">
                Change Password for {getUserDisplayName(selectedUser)}
              </h3>
              <input
                type="password"
                placeholder="New Password"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <div className="flex justify-end space-x-3 mt-4">
                <Button
                  variant="outline"
                  onClick={() => {
                    setShowPasswordModal(false);
                    setNewPassword('');
                    setSelectedUser(null);
                  }}
                >
                  Cancel
                </Button>
                <Button onClick={handleUpdatePassword}>
                  Update Password
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Warning Modal */}
      {showWarningModal && selectedUser && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
          <div className="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
            <div className="mt-3">
              <h3 className="text-lg font-medium text-gray-900 mb-4">
                Send Warning to {getUserDisplayName(selectedUser)}
              </h3>
              <textarea
                placeholder="Warning message"
                value={warningMessage}
                onChange={(e) => setWarningMessage(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                rows={4}
              />
              <div className="flex justify-end space-x-3 mt-4">
                <Button
                  variant="outline"
                  onClick={() => {
                    setShowWarningModal(false);
                    setWarningMessage('');
                    setSelectedUser(null);
                  }}
                >
                  Cancel
                </Button>
                <Button variant="danger" onClick={handleWarnUser}>
                  Send Warning
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* User Details Modal */}
      {showUserDetailsModal && selectedUser && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
          <div className="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
            <div className="mt-3">
              <h3 className="text-lg font-medium text-gray-900 mb-4">
                User Details: {getUserDisplayName(selectedUser)}
              </h3>
              <div className="space-y-3 text-sm">
                <div>
                  <span className="font-medium">Full Name:</span> {getUserDisplayName(selectedUser)}
                </div>
                <div>
                  <span className="font-medium">Username:</span> @{selectedUser.userName}
                </div>
                <div>
                  <span className="font-medium">Email:</span> {selectedUser.email}
                </div>
                <div>
                  <span className="font-medium">Email Verified:</span> 
                  <span className={selectedUser.emailConfirmed ? 'text-green-600' : 'text-red-600'}>
                    {selectedUser.emailConfirmed ? ' Yes' : ' No'}
                  </span>
                </div>
                <div>
                  <span className="font-medium">Role:</span> 
                  <span className={selectedUser.isAdmin ? 'text-red-600' : 'text-green-600'}>
                    {selectedUser.isAdmin ? ' Admin' : ' User'}
                  </span>
                </div>
                <div>
                  <span className="font-medium">Status:</span> 
                  <span className={selectedUser.isDeleted ? 'text-red-600' : 'text-green-600'}>
                    {selectedUser.isDeleted ? ' Deleted' : ' Active'}
                  </span>
                </div>
                {selectedUser.profilePictureUrl && (
                  <div>
                    <span className="font-medium">Profile Picture:</span> Yes
                  </div>
                )}
              </div>
              <div className="flex justify-end mt-4">
                <Button
                  variant="outline"
                  onClick={() => {
                    setShowUserDetailsModal(false);
                    setSelectedUser(null);
                  }}
                >
                  Close
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminUsers; 