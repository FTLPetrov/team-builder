import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { teamService } from '../services/teamService';
import Button from '../components/Button';
import Card, { CardHeader, CardTitle, CardContent } from '../components/Card';

const Teams = () => {
  const navigate = useNavigate();
  const { user } = useAuth();
  const [teams, setTeams] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [joiningTeams, setJoiningTeams] = useState(new Set());

  useEffect(() => {
    fetchTeams();
  }, []);

  const fetchTeams = async () => {
    try {
      setLoading(true);
      const teamsData = await teamService.getAllTeams();
      setTeams(teamsData);
    } catch (err) {
      setError('Failed to load teams');
      console.error('Error fetching teams:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleViewTeam = (teamId) => {
    navigate(`/teams/${teamId}`);
  };

  const handleCreateTeam = () => {
    navigate('/teams/create');
  };

  const handleJoinTeam = async (teamId) => {
    if (!user) {
      navigate('/login');
      return;
    }

    setJoiningTeams(prev => new Set(prev).add(teamId));
    
    try {
      const result = await teamService.joinTeam(teamId);
      if (result.success) {
        // Refresh teams to update the UI
        await fetchTeams();
        alert('Successfully joined the team!');
      } else {
        alert(result.message || 'Failed to join team');
      }
    } catch (error) {
      console.error('Error joining team:', error);
      alert('Failed to join team. Please try again.');
    } finally {
      setJoiningTeams(prev => {
        const newSet = new Set(prev);
        newSet.delete(teamId);
        return newSet;
      });
    }
  };

  const handleLeaveTeam = async (teamId) => {
    if (!user) {
      navigate('/login');
      return;
    }

    if (!confirm('Are you sure you want to leave this team?')) {
      return;
    }

    setJoiningTeams(prev => new Set(prev).add(teamId));
    
    try {
      const result = await teamService.leaveTeam(teamId);
      if (result.success) {
        // Refresh teams to update the UI
        await fetchTeams();
        alert('Successfully left the team!');
      } else {
        alert(result.message || 'Failed to leave team');
      }
    } catch (error) {
      console.error('Error leaving team:', error);
      alert('Failed to leave team. Please try again.');
    } finally {
      setJoiningTeams(prev => {
        const newSet = new Set(prev);
        newSet.delete(teamId);
        return newSet;
      });
    }
  };

  const isUserInTeam = (team) => {
    return team.members?.some(member => member.userId === user?.id);
  };

  const canJoinTeam = (team) => {
    return team.isOpen && !isUserInTeam(team);
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
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
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">All Teams</h1>
              <p className="text-gray-600 mt-2">
                Discover and join teams or create your own
              </p>
            </div>
            <Button onClick={handleCreateTeam}>
              <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
              Create Team
            </Button>
          </div>
        </div>

        {/* Error State */}
        {error && (
          <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-md">
            <p className="text-red-600">{error}</p>
          </div>
        )}

        {/* Teams Grid */}
        {teams.length > 0 ? (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {teams.map((team) => {
              const userInTeam = isUserInTeam(team);
              const canJoin = canJoinTeam(team);
              const isJoining = joiningTeams.has(team.id);

              return (
                <Card key={team.id} className="hover:shadow-lg transition-shadow">
                  <CardHeader>
                    <div className="flex items-center justify-between">
                      <CardTitle className="text-lg">{team.name}</CardTitle>
                      <span className={`px-2 py-1 rounded-full text-xs ${
                        team.isOpen ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
                      }`}>
                        {team.isOpen ? 'Open' : 'Closed'}
                      </span>
                    </div>
                  </CardHeader>
                  <CardContent>
                    <p className="text-gray-600 mb-4 line-clamp-2">
                      {team.description}
                    </p>
                    <div className="flex items-center justify-between text-sm text-gray-500 mb-4">
                      <span>{team.members?.length || 0} members</span>
                      <span>{team.events?.length || 0} events</span>
                    </div>
                    <div className="flex space-x-2">
                      <Button
                        onClick={() => handleViewTeam(team.id)}
                        variant="outline"
                        className="flex-1"
                      >
                        View Team
                      </Button>
                                             {userInTeam ? (
                         <Button
                           onClick={() => handleLeaveTeam(team.id)}
                           disabled={isJoining}
                           className="flex-1 bg-red-600 hover:bg-red-700 text-white"
                         >
                           {isJoining ? (
                             <div className="flex items-center">
                               <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                               Leaving...
                             </div>
                           ) : (
                             'Leave Team'
                           )}
                         </Button>
                      ) : (
                        <Button
                          onClick={() => handleJoinTeam(team.id)}
                          disabled={!canJoin || isJoining}
                          className={`flex-1 ${
                            canJoin 
                              ? 'bg-green-600 hover:bg-green-700 text-white' 
                              : 'bg-gray-300 text-gray-500 cursor-not-allowed'
                          }`}
                        >
                          {isJoining ? (
                            <div className="flex items-center">
                              <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                              Joining...
                            </div>
                          ) : canJoin ? (
                            'Join Team'
                          ) : (
                            'Closed'
                          )}
                        </Button>
                      )}
                    </div>
                  </CardContent>
                </Card>
              );
            })}
          </div>
        ) : (
          <div className="text-center py-12">
            <div className="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <svg className="w-8 h-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
            </div>
            <h3 className="text-lg font-medium text-gray-900 mb-2">No teams found</h3>
            <p className="text-gray-600 mb-6">Be the first to create a team!</p>
            <Button onClick={handleCreateTeam}>
              Create Your First Team
            </Button>
          </div>
        )}
      </div>
    </div>
  );
};

export default Teams; 