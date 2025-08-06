import { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { teamService } from '../services/api/teamService';
import { chatService } from '../services/api/chatService';
import Button from '../components/Button';
import Card, { CardHeader, CardTitle, CardContent } from '../components/Card';
import { safeText } from '../utils/escapeHtml';

const Chat = () => {
  const { teamId } = useParams();
  const navigate = useNavigate();
  const { user, token } = useAuth();
  const [team, setTeam] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [messages, setMessages] = useState([]);
  const [sending, setSending] = useState(false);
  const [initialLoadComplete, setInitialLoadComplete] = useState(false);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [loadingMore, setLoadingMore] = useState(false);
  const messagesContainerRef = useRef(null);
  const inputRef = useRef(null);

  const scrollToBottom = () => {
    if (messagesContainerRef.current) {
      messagesContainerRef.current.scrollTop = messagesContainerRef.current.scrollHeight;
    }
  };

  useEffect(() => {
    fetchTeamDetails();
    loadMessages();
    setupSignalR();
    
    return () => {
      chatService.disconnect();
    };
  }, [teamId]);


  useEffect(() => {
    if (messages.length > 0 && initialLoadComplete) {
      setTimeout(() => scrollToBottom(), 100);
    }
  }, [messages, initialLoadComplete]);



  const setupSignalR = async () => {
    if (token) {
      console.log('Setting up SignalR connection...');
      await chatService.connect(token);
      await chatService.joinTeam(teamId);
      

      chatService.onMessageReceived((newMessage) => {
        console.log('Received new message via SignalR:', newMessage);
        setMessages(prev => {

          const messageExists = prev.some(msg => 
            msg.id === newMessage.id || 
            (msg.isTemp && msg.message === newMessage.message && msg.userId === newMessage.userId)
          );
          
          if (messageExists) {
            return prev;
          }
          
          const updatedMessages = [...prev, newMessage];

          setTimeout(() => scrollToBottom(), 50);
          return updatedMessages;
        });
      });
      console.log('SignalR setup complete');
    }
  };

  const fetchTeamDetails = async () => {
    try {
      setLoading(true);
      const teamData = await teamService.getTeamById(teamId);
      setTeam(teamData);
    } catch (err) {
      setError('Failed to load team details');
      console.error('Error fetching team:', err);
    } finally {
      setLoading(false);
    }
  };

  const loadMessages = async (pageNum = 1, append = false) => {
    try {
      if (pageNum === 1) {
        setLoading(true);
      } else {
        setLoadingMore(true);
      }
      
      const messagesData = await chatService.getTeamMessages(teamId, pageNum);
      
      if (append) {
        setMessages(prev => [...messagesData, ...prev]);
      } else {
        setMessages(messagesData);
      }
      
      setHasMore(messagesData.length === 20); // Assuming 20 messages per page
      setPage(pageNum);
      setInitialLoadComplete(true);
    } catch (err) {
      console.error('Error loading messages:', err);
      setInitialLoadComplete(true);
    } finally {
      setLoading(false);
      setLoadingMore(false);
    }
  };

  const handleSendMessage = async (e) => {
    e.preventDefault();
    if (!message.trim() || sending) return;

    const messageText = message.trim();
    setMessage('');


    const tempMessage = {
      id: Date.now(), // Temporary ID
      teamId: teamId,
      userId: user?.id,
      userFirstName: user?.firstName || '',
      userLastName: user?.lastName || '',
      userName: user?.userName || '',
      message: messageText,
      createdAt: new Date().toISOString(),
      isTemp: true // Flag to identify temporary messages
    };


    setMessages(prev => [...prev, tempMessage]);

    try {
      setSending(true);
      const response = await chatService.sendMessage(teamId, messageText);
      

      setMessages(prev => {
        const updatedMessages = prev.map(msg => 
          msg.isTemp && msg.id === tempMessage.id 
            ? { ...response, isTemp: false }
            : msg
        );

        setTimeout(() => scrollToBottom(), 50);

        setTimeout(() => inputRef.current?.focus(), 100);
        return updatedMessages;
      });
    } catch (err) {
      console.error('Error sending message:', err);
      setError('Failed to send message');
      

      setMessages(prev => prev.filter(msg => msg.id !== tempMessage.id));
    } finally {
      setSending(false);
    }
  };

  const formatTime = (timestamp) => {
    return new Date(timestamp).toLocaleTimeString([], { 
      hour: '2-digit', 
      minute: '2-digit' 
    });
  };

  const getUserDisplayName = (member) => {
    const firstName = member.firstName || '';
    const lastName = member.lastName || '';
    const fullName = `${firstName} ${lastName}`.trim();
    return fullName || member.userName || 'Unknown User';
  };

  const getMessageSenderName = (message) => {
    if (message.userId === user?.id) {
      return 'You';
    }
    return `${message.userFirstName} ${message.userLastName}`.trim() || message.userName || 'Unknown User';
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading chat...</p>
        </div>
      </div>
    );
  }

  if (error || !team) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-xl font-semibold text-gray-900 mb-2">Team Not Found</h2>
          <p className="text-gray-600 mb-4">{error || 'The team you are looking for does not exist.'}</p>
          <Button onClick={() => navigate('/teams')}>Back to Teams</Button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="mb-6">
          <div className="flex items-center justify-between">
            <div className="flex items-center space-x-4">
              <Button
                onClick={() => navigate(`/teams/${teamId}`)}
                variant="outline"
                size="small"
              >
                <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                </svg>
                Back to Team
              </Button>
              <div>
                <h1 className="text-2xl font-bold text-gray-900">{team.name} - Chat</h1>
                <p className="text-gray-600">{team.members?.length || 0} members</p>
              </div>
            </div>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-4 gap-6 h-[calc(100vh-200px)]">
          {/* Chat Messages */}
          <div className="lg:col-span-3">
            <Card className="h-full flex flex-col">
              <CardHeader>
                <CardTitle>Team Chat</CardTitle>
              </CardHeader>
              <CardContent className="flex-1 flex flex-col">
                                {/* Messages Area */}
                <div className="flex-1 flex flex-col min-h-0">
                  {/* Load More Button */}
                  {hasMore && (
                    <div className="text-center py-2 flex-shrink-0">
                      <button
                        onClick={() => loadMessages(page + 1, true)}
                        disabled={loadingMore}
                        className="px-4 py-2 text-sm text-blue-600 hover:text-blue-800 disabled:opacity-50"
                      >
                        {loadingMore ? 'Loading...' : 'Load More Messages'}
                      </button>
                    </div>
                  )}
                  
                  {/* Scrollable Messages Container */}
                  <div 
                    ref={messagesContainerRef}
                    className="flex-1 overflow-y-auto min-h-0" 
                    style={{ maxHeight: '400px' }}
                  >
                    <div className="space-y-4 p-4">
                      {messages.length === 0 ? (
                        <div className="text-center py-8">
                          <div className="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
                            <svg className="w-8 h-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
                            </svg>
                          </div>
                          <h3 className="text-lg font-medium text-gray-900 mb-2">No messages yet</h3>
                          <p className="text-gray-600">Start the conversation by sending a message!</p>
                        </div>
                      ) : (
                        messages.map((msg) => (
                          <div
                            key={msg.id}
                            className={`flex ${msg.userId === user?.id ? 'justify-end' : 'justify-start'}`}
                          >
                            <div
                              className={`max-w-xs lg:max-w-md px-4 py-2 rounded-lg ${
                                msg.userId === user?.id
                                  ? 'bg-blue-600 text-white'
                                  : 'bg-gray-100 text-gray-900'
                              }`}
                            >
                              <div className="flex items-center space-x-2 mb-1">
                                <span className="text-sm font-medium">
                                  {getMessageSenderName(msg)}
                                </span>
                                <span className={`text-xs ${
                                  msg.userId === user?.id ? 'text-blue-200' : 'text-gray-500'
                                }`}>
                                  {formatTime(msg.createdAt)}
                                </span>
                              </div>
                              <p className="text-sm">{safeText(msg.message)}</p>
                            </div>
                          </div>
                                                 ))
                       )}

                     </div>
                   </div>
                </div>

                {/* Message Input */}
                <form onSubmit={handleSendMessage} className="flex space-x-2">
                  <input
                    ref={inputRef}
                    type="text"
                    value={message}
                    onChange={(e) => setMessage(e.target.value)}
                    placeholder="Type your message..."
                    disabled={sending}
                    className="flex-1 px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:opacity-50"
                  />
                  <Button type="submit" disabled={!message.trim() || sending}>
                    {sending ? 'Sending...' : 'Send'}
                  </Button>
                </form>
              </CardContent>
            </Card>
          </div>

          {/* Team Participants */}
          <div className="lg:col-span-1">
            <Card className="h-full">
              <CardHeader>
                <CardTitle>Participants</CardTitle>
              </CardHeader>
              <CardContent>
                {team.members && team.members.length > 0 ? (
                  <div className="space-y-3">
                    {team.members.map((member) => (
                      <div key={member.userId} className="flex items-center space-x-3 p-2 rounded-lg hover:bg-gray-50">
                        <div className="w-8 h-8 bg-blue-500 rounded-full flex items-center justify-center overflow-hidden">
                          {member.profilePictureUrl ? (
                            <img 
                              src={`${import.meta.env.VITE_BACKEND_URL || 'http://localhost:5000'}${member.profilePictureUrl}`}
                              alt="Profile" 
                              className="w-full h-full object-cover"
                            />
                          ) : (
                            <span className="text-white text-sm font-medium">
                              {getUserDisplayName(member).charAt(0).toUpperCase()}
                            </span>
                          )}
                        </div>
                        <div className="flex-1 min-w-0">
                          <p className="text-sm font-medium text-gray-900 truncate">
                            {getUserDisplayName(member)}
                          </p>
                          <p className="text-xs text-gray-500 truncate">
                            {member.role}
                          </p>
                        </div>
                        <div className="w-2 h-2 bg-green-500 rounded-full"></div>
                      </div>
                    ))}
                  </div>
                ) : (
                  <div className="text-center py-8">
                    <div className="w-12 h-12 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-3">
                      <svg className="w-6 h-6 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                      </svg>
                    </div>
                    <p className="text-sm text-gray-500">No members</p>
                  </div>
                )}
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Chat; 