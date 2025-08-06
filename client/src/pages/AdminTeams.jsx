import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { adminService } from '../services/api/adminService';
import Card, { CardHeader, CardTitle, CardContent } from '../components/Card';
import Button from '../components/Button';
import { Link } from 'react-router-dom';

const AdminTeams = () => {
  const { isAdmin, user } = useAuth();
  const [teams, setTeams] = useState([]);
  const [filteredTeams, setFilteredTeams] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(true);
  const [selectedTeam, setSelectedTeam] = useState(null);
  const [showKickModal, setShowKickModal] = useState(false);
  const [selectedMember, setSelectedMember] = useState(null);

  useEffect(() => {
    if (!isAdmin) return;
    fetchTeams();
  }, [isAdmin]);

  useEffect(() => {
    const filtered = teams.filter(team => 
      team.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      team.description?.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredTeams(filtered);
  }, [teams, searchTerm]);

  const fetchTeams = async () => {
    try {
      const response = await adminService.getAllTeams();
      setTeams(response);
    } catch (error) {
      console.error('Error fetching teams:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteTeam = async (teamId) => {
    if (!window.confirm('Are you sure you want to delete this team? This action cannot be undone.')) return;
    
    try {
      await adminService.deleteTeam(teamId);
      setTeams(teams.filter(team => team.id !== teamId));
    } catch (error) {
      console.error('Error deleting team:', error);
      alert('Failed to delete team');
    }
  };

  const handleJoinTeam = async (teamId) => {
    try {
      await adminService.joinTeam(teamId, user.id);

      fetchTeams();
      alert('Successfully joined team');
    } catch (error) {
      console.error('Error joining team:', error);
      alert('Failed to join team');
    }
  };

  const handleKickMember = async () => {
    if (!selectedTeam || !selectedMember) return;
    
    try {
      await adminService.kickUserFromTeam(selectedTeam.id, selectedMember.userId);

      fetchTeams();
      setShowKickModal(false);
      setSelectedTeam(null);
      setSelectedMember(null);
      alert('Member kicked successfully');
    } catch (error) {
      console.error('Error kicking member:', error);
      alert('Failed to kick member');
    }
  };

  const handleClearChat = async (teamId) => {
    if (!window.confirm('Are you sure you want to clear all chat messages for this team?')) return;
    
    try {
      await adminService.clearTeamChat(teamId);
      alert('Chat cleared successfully');
    } catch (error) {
      console.error('Error clearing chat:', error);
      alert('Failed to clear chat');
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
          <p className="mt-4 text-gray-600">Loading teams...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Team Management</h1>
          <p className="text-gray-600 mt-2">Manage all teams in the system</p>
        </div>

        {/* Search */}
        <div className="mb-6">
          <div className="max-w-md">
            <label htmlFor="search" className="block text-sm font-medium text-gray-700 mb-2">
              Search Teams
            </label>
            <input
              type="text"
              id="search"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="Search by team name or description..."
              className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-red-500"
            />
          </div>
        </div>

        {/* Teams Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredTeams.map((team) => (
            <Card key={team.id}>
              <CardContent className="p-6">
                <div className="mb-4">
                  <h3 className="text-lg font-medium text-gray-900 mb-2">{team.name}</h3>
                  <p className="text-gray-600 text-sm mb-3">{team.description}</p>
                  <div className="text-sm text-gray-500">
                    Created: {new Date(team.createdAt).toLocaleDateString()}
                  </div>
                </div>

                <div className="mb-4">
                  <h4 className="text-sm font-medium text-gray-700 mb-2">Members ({team.members?.length || 0})</h4>
                  <div className="space-y-1">
                    {team.members?.slice(0, 3).map((member) => (
                      <div key={member.userId} className="flex items-center justify-between text-sm">
                        <div className="flex items-center">
                          <div className="w-6 h-6 bg-blue-500 rounded-full flex items-center justify-center mr-2 overflow-hidden">
                            {member.profilePictureUrl ? (
                              <img 
                                src={`${import.meta.env.VITE_BACKEND_URL || 'http://localhost:5000'}${member.profilePictureUrl}`}
                                alt="Profile" 
                                className="w-full h-full object-cover"
                              />
                            ) : (
                              <span className="text-white text-xs font-medium">
                                {member.firstName ? member.firstName.charAt(0).toUpperCase() : 'U'}
                              </span>
                            )}
                          </div>
                          <span className="text-gray-600">
                            {member.firstName} {member.lastName}
                          </span>
                        </div>
                        <span className="text-xs bg-gray-100 px-2 py-1 rounded">
                          {member.role}
                        </span>
                      </div>
                    ))}
                    {team.members?.length > 3 && (
                      <div className="text-xs text-gray-500">
                        +{team.members.length - 3} more members
                      </div>
                    )}
                  </div>
                </div>

                <div className="flex flex-wrap gap-2">
                  <Link to={`/teams/${team.id}`}>
                    <Button
                      size="small"
                      variant="outline"
                    >
                      View Team
                    </Button>
                  </Link>
                  <Button
                    size="small"
                    variant="outline"
                    onClick={() => handleJoinTeam(team.id)}
                  >
                    Join Team
                  </Button>
                  <Button
                    size="small"
                    variant="outline"
                    onClick={() => handleClearChat(team.id)}
                  >
                    Clear Chat
                  </Button>
                  <Button
                    size="small"
                    variant="destructive"
                    onClick={() => handleDeleteTeam(team.id)}
                  >
                    Delete Team
                  </Button>
                </div>

                {team.members && team.members.length > 0 && (
                  <div className="mt-4">
                    <h4 className="text-sm font-medium text-gray-700 mb-2">Manage Members</h4>
                    <div className="space-y-1">
                      {team.members.map((member) => (
                        <div key={member.userId} className="flex items-center justify-between text-sm">
                          <div className="flex items-center">
                            <div className="w-6 h-6 bg-blue-500 rounded-full flex items-center justify-center mr-2 overflow-hidden">
                              {member.profilePictureUrl ? (
                                <img 
                                  src={`${import.meta.env.VITE_BACKEND_URL || 'http://localhost:5000'}${member.profilePictureUrl}`}
                                  alt="Profile" 
                                  className="w-full h-full object-cover"
                                />
                              ) : (
                                <span className="text-white text-xs font-medium">
                                  {member.firstName ? member.firstName.charAt(0).toUpperCase() : 'U'}
                                </span>
                              )}
                            </div>
                            <span className="text-gray-600">
                              {member.firstName} {member.lastName}
                            </span>
                          </div>
                          <Button
                            size="small"
                            variant="outline"
                            onClick={() => {
                              setSelectedTeam(team);
                              setSelectedMember(member);
                              setShowKickModal(true);
                            }}
                          >
                            Kick
                          </Button>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>
          ))}
        </div>

        {filteredTeams.length === 0 && (
          <div className="text-center py-12">
            <p className="text-gray-500">No teams found matching your search criteria.</p>
          </div>
        )}

        {/* Kick Member Modal */}
        {showKickModal && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg p-6 w-full max-w-md">
              <h3 className="text-lg font-medium text-gray-900 mb-4">
                Kick Member from {selectedTeam?.name}
              </h3>
              <p className="text-gray-600 mb-4">
                Are you sure you want to kick {selectedMember?.firstName} {selectedMember?.lastName} from the team?
              </p>
              <div className="flex space-x-2">
                <Button onClick={handleKickMember} variant="destructive">
                  Kick Member
                </Button>
                <Button 
                  variant="outline" 
                  onClick={() => {
                    setShowKickModal(false);
                    setSelectedTeam(null);
                    setSelectedMember(null);
                  }}
                >
                  Cancel
                </Button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default AdminTeams; 