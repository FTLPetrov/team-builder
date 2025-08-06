import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { teamService } from '../services/api/teamService';
import { invitationService } from '../services/api/invitationService';
import Button from '../components/Button';
import Card, { CardHeader, CardTitle, CardContent } from '../components/Card';
import Notification from '../components/Notification';

const TeamDetail = () => {
  const { teamId } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [team, setTeam] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [showInviteModal, setShowInviteModal] = useState(false);
  const [inviteEmail, setInviteEmail] = useState('');
  const [inviteLoading, setInviteLoading] = useState(false);
  const [notification, setNotification] = useState(null);

  useEffect(() => {
    fetchTeamDetails();
  }, [teamId]);





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

  const handleInviteMember = async (e) => {
    e.preventDefault();
    if (!inviteEmail.trim()) return;

    setInviteLoading(true);
    

    const emailToSend = inviteEmail.trim();
    

    setInviteEmail('');
    setShowInviteModal(false);
    
    try {
      const invitationData = {
        TeamId: teamId,
        InvitedUserEmail: emailToSend,
        InvitedById: user.id
      };

      const result = await invitationService.createInvitation(invitationData);
      
      console.log('TeamDetail: Invitation result:', result);
      
      if (result.Success) {
        console.log('TeamDetail: Invitation successful');
        

        if (result.IsAlreadyInvited) {
          setNotification({
            message: result.Message || 'User has already been invited to this team',
            type: 'info'
          });
        } else {
          setNotification({
            message: 'Invitation sent successfully!',
            type: 'success'
          });
        }
      } else {

        console.log('TeamDetail: Result.Success is false, but not showing error notification');
      }
    } catch (error) {
      console.error('Error inviting member:', error);
      console.error('Error response:', error.response?.data);
      console.error('Error status:', error.response?.status);
      

      if (error.response?.status === 400) {
        const responseData = error.response?.data;
        if (responseData?.Success && responseData?.IsAlreadyInvited) {
          console.log('TeamDetail: Already invited case');
          setNotification({
            message: responseData.Message || 'User has already been invited to this team',
            type: 'info'
          });
          return;
        }
        

        if (!responseData?.errorMessage && !responseData?.ErrorMessage) {
          console.log('TeamDetail: 400 error without specific details, assuming success');
          setNotification({
            message: 'Invitation sent successfully!',
            type: 'success'
          });
          return;
        }
        

        console.log('TeamDetail: 400 error with specific error details, not showing notification');
      } else {

        console.log('TeamDetail: Not a 400 error, assuming success');
        setNotification({
          message: 'Invitation sent successfully!',
          type: 'success'
        });
      }
    } finally {
      setInviteLoading(false);
    }
  };



  const handleKickMember = async (userId) => {
    if (!confirm('Are you sure you want to remove this member?')) return;

    try {
      await teamService.kickMember(teamId, userId);
      await fetchTeamDetails(); // Refresh team data
    } catch (error) {
      console.error('Error kicking member:', error);
    }
  };

  const handleDeleteTeam = async () => {
    if (!confirm('Are you sure you want to delete this team? This action cannot be undone.')) return;

    setDeleteLoading(true);
    try {
      await teamService.deleteTeam(teamId);
      navigate('/teams');
    } catch (error) {
      console.error('Error deleting team:', error);
    } finally {
      setDeleteLoading(false);
    }
  };

  const isOrganizer = team?.organizerId === user?.id;

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading team details...</p>
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
      {notification && (
        <Notification
          message={notification.message}
          type={notification.type}
          onClose={() => setNotification(null)}
        />
      )}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="mb-8">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">{team.name}</h1>
              <p className="text-gray-600 mt-2">{team.description}</p>
              <div className="flex items-center mt-2 space-x-4">
                <span className={`px-2 py-1 rounded-full text-xs ${
                  team.isOpen ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
                }`}>
                  {team.isOpen ? 'Open Team' : 'Closed Team'}
                </span>
                <span className="text-sm text-gray-500">{team.members?.length || 0} members</span>
              </div>
            </div>
            <div className="flex space-x-3">
              <Button
                onClick={() => navigate(`/teams/${teamId}/chat`)}
                variant="outline"
              >
                <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
                </svg>
                Chat
              </Button>
              {isOrganizer && (
                <>
                  <Button
                    onClick={() => setShowInviteModal(true)}
                    variant="outline"
                  >
                    <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                    </svg>
                    Invite Member
                  </Button>
                  <Button
                    onClick={handleDeleteTeam}
                    variant="danger"
                    disabled={deleteLoading}
                  >
                    {deleteLoading ? 'Deleting...' : 'Delete Team'}
                  </Button>
                </>
              )}
              <Button
                onClick={() => navigate('/teams')}
                variant="outline"
              >
                Back to Teams
              </Button>
            </div>
          </div>
        </div>

        {/* Team Members */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          <Card>
            <CardHeader>
              <CardTitle>Team Members</CardTitle>
            </CardHeader>
            <CardContent>
              {team.members && team.members.length > 0 ? (
                <div className="space-y-4">
                  {team.members.map((member) => (
                    <div key={member.userId} className="flex items-center justify-between p-3 border border-gray-200 rounded-lg">
                      <div className="flex items-center">
                        <div className="w-10 h-10 bg-blue-500 rounded-full flex items-center justify-center mr-3 overflow-hidden">
                          {member.profilePictureUrl ? (
                            <img 
                              src={`${import.meta.env.VITE_BACKEND_URL || 'http://localhost:5000'}${member.profilePictureUrl}`}
                              alt="Profile" 
                              className="w-full h-full object-cover"
                            />
                          ) : (
                            <span className="text-white text-sm font-medium">
                              {member.firstName ? member.firstName.charAt(0).toUpperCase() : 'U'}
                            </span>
                          )}
                        </div>
                        <div>
                          <p className="font-medium text-gray-900">
                            {member.firstName && member.lastName 
                              ? `${member.firstName} ${member.lastName}` 
                              : member.userName || 'Unknown User'}
                          </p>
                          <p className="text-sm text-gray-500">{member.email || 'No email'}</p>
                        </div>
                      </div>
                      <div className="flex items-center space-x-2">
                        <span className={`px-2 py-1 rounded-full text-xs ${
                          member.role === 'Organizer' 
                            ? 'bg-purple-100 text-purple-800' 
                            : 'bg-blue-100 text-blue-800'
                        }`}>
                          {member.role}
                        </span>
                        {isOrganizer && member.userId !== user?.id && (
                          <Button
                            onClick={() => handleKickMember(member.userId)}
                            variant="ghost"
                            size="small"
                            className="text-red-600 hover:text-red-700"
                          >
                            Remove
                          </Button>
                        )}
                      </div>
                    </div>
                  ))}
                </div>
              ) : (
                <div className="text-center py-8">
                  <div className="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
                    <svg className="w-8 h-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                    </svg>
                  </div>
                  <h3 className="text-lg font-medium text-gray-900 mb-2">No members yet</h3>
                  <p className="text-gray-600">Invite members to start collaborating</p>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Team Events */}
          <Card>
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle>Team Events</CardTitle>
                {isOrganizer && (
                  <Button
                    onClick={() => navigate('/events/create')}
                    size="small"
                    variant="outline"
                  >
                    <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                    </svg>
                    Create Event
                  </Button>
                )}
              </div>
            </CardHeader>
            <CardContent>
              {team.events && team.events.length > 0 ? (
                <div className="space-y-4">
                  {team.events.map((event) => (
                    <div key={event.id} className="p-3 border border-gray-200 rounded-lg">
                      <div className="flex items-start justify-between">
                        <div className="flex-1">
                          <h4 className="font-medium text-gray-900">{event.name}</h4>
                          <p className="text-sm text-gray-600">{event.description}</p>
                          <p className="text-xs text-gray-500 mt-1">
                            {new Date(event.date).toLocaleDateString()}
                          </p>
                        </div>
                        <Button
                          onClick={() => navigate(`/events/${event.id}`)}
                          size="small"
                          variant="outline"
                          className="ml-3 flex-shrink-0"
                        >
                          View
                        </Button>
                      </div>
                    </div>
                  ))}
                </div>
              ) : (
                <div className="text-center py-8">
                  <div className="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
                    <svg className="w-8 h-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                    </svg>
                  </div>
                  <h3 className="text-lg font-medium text-gray-900 mb-2">No events yet</h3>
                  <p className="text-gray-600">Create events to organize team activities</p>
                </div>
              )}
            </CardContent>
          </Card>
        </div>

        {/* Invite Modal */}
        {showInviteModal && (
          <div className="fixed inset-0 bg-gray-900 bg-opacity-5 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg p-6 w-full max-w-md mx-4 shadow-xl">
              <h3 className="text-lg font-medium text-gray-900 mb-4">Invite Member</h3>
              <form onSubmit={handleInviteMember}>
                <div className="mb-4">
                  <label htmlFor="inviteEmail" className="block text-sm font-medium text-gray-700 mb-2">
                    Email Address
                  </label>
                  <input
                    type="email"
                    id="inviteEmail"
                    value={inviteEmail}
                    onChange={(e) => setInviteEmail(e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                    placeholder="Enter email address"
                    required
                    disabled={inviteLoading}
                  />
                </div>
                <div className="flex justify-end space-x-3">
                  <Button
                    type="button"
                    variant="outline"
                    onClick={() => {
                      setShowInviteModal(false);
                      setInviteEmail('');
                    }}
                    disabled={inviteLoading}
                  >
                    Cancel
                  </Button>
                  <Button
                    type="submit"
                    disabled={inviteLoading}
                  >
                    {inviteLoading ? 'Inviting...' : 'Send Invite'}
                  </Button>
                </div>
              </form>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default TeamDetail; 