import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { adminService } from '../services/api/adminService';
import Card, { CardHeader, CardTitle, CardContent } from '../components/Card';
import Button from '../components/Button';

const AdminSupport = () => {
  const { isAdmin } = useAuth();
  const [messages, setMessages] = useState([]);
  const [filteredMessages, setFilteredMessages] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(true);
  const [selectedMessage, setSelectedMessage] = useState(null);
  const [showMessageModal, setShowMessageModal] = useState(false);

  useEffect(() => {
    if (!isAdmin) return;
    fetchMessages();
  }, [isAdmin]);

  useEffect(() => {
    const filtered = messages.filter(message => 
      message.subject?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      message.message?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      message.userFirstName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      message.userLastName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      message.userEmail?.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredMessages(filtered);
  }, [messages, searchTerm]);

  const fetchMessages = async () => {
    try {
      const response = await adminService.getAllSupportMessages();
      setMessages(response);
    } catch (error) {
      console.error('Error fetching support messages:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleMarkAsRead = async (messageId) => {
    try {
      await adminService.markSupportMessageAsRead(messageId);
      setMessages(messages.map(msg => 
        msg.id === messageId ? { ...msg, isRead: true } : msg
      ));
    } catch (error) {
      console.error('Error marking message as read:', error);
    }
  };

  const handleToggleFavorite = async (messageId) => {
    try {
      await adminService.toggleSupportMessageFavorite(messageId);
      setMessages(messages.map(msg => 
        msg.id === messageId ? { ...msg, isFavorite: !msg.isFavorite } : msg
      ));
    } catch (error) {
      console.error('Error toggling favorite:', error);
    }
  };

  const handleMarkAsCompleted = async (messageId) => {
    try {
      await adminService.markSupportMessageAsCompleted(messageId);
      setMessages(messages.map(msg => 
        msg.id === messageId ? { ...msg, isCompleted: !msg.isCompleted } : msg
      ));
    } catch (error) {
      console.error('Error marking as completed:', error);
    }
  };

  const handleDeleteMessage = async (messageId) => {
    if (!window.confirm('Are you sure you want to delete this support message?')) return;
    
    try {
      await adminService.deleteSupportMessage(messageId);
      setMessages(messages.filter(msg => msg.id !== messageId));
    } catch (error) {
      console.error('Error deleting message:', error);
      alert('Failed to delete message');
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
          <p className="mt-4 text-gray-600">Loading support messages...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Support Messages</h1>
          <p className="text-gray-600 mt-2">Review and manage user support requests</p>
        </div>

        {/* Search */}
        <div className="mb-6">
          <div className="max-w-md">
            <label htmlFor="search" className="block text-sm font-medium text-gray-700 mb-2">
              Search Messages
            </label>
            <input
              type="text"
              id="search"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="Search by subject, message, or user..."
              className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-red-500"
            />
          </div>
        </div>

        {/* Messages Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredMessages.map((message) => (
            <Card key={message.id} className={`${!message.isRead ? 'border-l-4 border-l-blue-500' : ''}`}>
              <CardContent className="p-6">
                <div className="mb-4">
                  <div className="flex items-start justify-between mb-2">
                    <h3 className="text-lg font-medium text-gray-900">{message.subject}</h3>
                    <div className="flex space-x-1">
                      {message.isFavorite && (
                        <svg className="w-5 h-5 text-yellow-500" fill="currentColor" viewBox="0 0 20 20">
                          <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                        </svg>
                      )}
                      {message.isCompleted && (
                        <svg className="w-5 h-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                          <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                        </svg>
                      )}
                    </div>
                  </div>
                  
                  <p className="text-gray-600 text-sm mb-3 line-clamp-3">{message.message}</p>
                  
                  <div className="space-y-2 text-sm">
                    <div className="flex items-center">
                      <svg className="w-4 h-4 text-gray-400 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                      </svg>
                      <span className="text-gray-600">
                        {message.userFirstName} {message.userLastName}
                      </span>
                    </div>
                    
                    <div className="flex items-center">
                      <svg className="w-4 h-4 text-gray-400 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 4.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                      </svg>
                      <span className="text-gray-600">{message.userEmail}</span>
                    </div>
                    
                    <div className="text-xs text-gray-500">
                      {formatDate(message.createdAt)}
                    </div>
                  </div>
                </div>

                <div className="flex flex-wrap gap-2">
                  {!message.isRead && (
                    <Button
                      size="small"
                      variant="outline"
                      onClick={() => handleMarkAsRead(message.id)}
                    >
                      Mark Read
                    </Button>
                  )}
                  <Button
                    size="small"
                    variant="outline"
                    onClick={() => handleToggleFavorite(message.id)}
                  >
                    {message.isFavorite ? 'Unfavorite' : 'Favorite'}
                  </Button>
                  <Button
                    size="small"
                    variant="outline"
                    onClick={() => handleMarkAsCompleted(message.id)}
                  >
                    {message.isCompleted ? 'Mark Incomplete' : 'Mark Complete'}
                  </Button>
                  <Button
                    size="small"
                    variant="outline"
                    onClick={() => {
                      setSelectedMessage(message);
                      setShowMessageModal(true);
                    }}
                  >
                    View Full
                  </Button>
                  <Button
                    size="small"
                    variant="destructive"
                    onClick={() => handleDeleteMessage(message.id)}
                  >
                    Delete
                  </Button>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>

        {filteredMessages.length === 0 && (
          <div className="text-center py-12">
            <p className="text-gray-500">No support messages found matching your search criteria.</p>
          </div>
        )}

        {/* Message Detail Modal */}
        {showMessageModal && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg p-6 w-full max-w-2xl max-h-[80vh] overflow-y-auto">
              <div className="flex justify-between items-start mb-4">
                <h3 className="text-lg font-medium text-gray-900">{selectedMessage?.subject}</h3>
                <Button 
                  variant="outline" 
                  size="small"
                  onClick={() => {
                    setShowMessageModal(false);
                    setSelectedMessage(null);
                  }}
                >
                  Close
                </Button>
              </div>
              
              <div className="space-y-4">
                <div>
                  <h4 className="text-sm font-medium text-gray-700 mb-2">Message</h4>
                  <p className="text-gray-600 whitespace-pre-wrap">{selectedMessage?.message}</p>
                </div>
                
                <div className="grid grid-cols-2 gap-4 text-sm">
                  <div>
                    <span className="font-medium text-gray-700">From:</span>
                    <p className="text-gray-600">{selectedMessage?.userFirstName} {selectedMessage?.userLastName}</p>
                    <p className="text-gray-600">{selectedMessage?.userEmail}</p>
                  </div>
                  <div>
                    <span className="font-medium text-gray-700">Date:</span>
                    <p className="text-gray-600">{formatDate(selectedMessage?.createdAt)}</p>
                  </div>
                </div>
                
                <div className="flex space-x-2">
                  {!selectedMessage?.isRead && (
                    <Button
                      size="small"
                      variant="outline"
                      onClick={() => {
                        handleMarkAsRead(selectedMessage.id);
                        setSelectedMessage({ ...selectedMessage, isRead: true });
                      }}
                    >
                      Mark Read
                    </Button>
                  )}
                  <Button
                    size="small"
                    variant="outline"
                    onClick={() => {
                      handleToggleFavorite(selectedMessage.id);
                      setSelectedMessage({ ...selectedMessage, isFavorite: !selectedMessage.isFavorite });
                    }}
                  >
                    {selectedMessage?.isFavorite ? 'Unfavorite' : 'Favorite'}
                  </Button>
                  <Button
                    size="small"
                    variant="outline"
                    onClick={() => {
                      handleMarkAsCompleted(selectedMessage.id);
                      setSelectedMessage({ ...selectedMessage, isCompleted: !selectedMessage.isCompleted });
                    }}
                  >
                    {selectedMessage?.isCompleted ? 'Mark Incomplete' : 'Mark Complete'}
                  </Button>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default AdminSupport; 