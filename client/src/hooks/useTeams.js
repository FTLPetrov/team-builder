import { useState, useEffect } from 'react';
import { teamService } from '../services/api/teamService';

export const useTeams = () => {
  const [teams, setTeams] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchTeams = async () => {
    try {
      setLoading(true);
      const data = await teamService.getUserTeams();
      setTeams(data);
      setError(null);
    } catch (err) {
      setError(err.message);
      console.error('Error fetching teams:', err);
    } finally {
      setLoading(false);
    }
  };

  const createTeam = async (teamData) => {
    try {
      const newTeam = await teamService.createTeam(teamData);
      setTeams(prev => [...prev, newTeam]);
      return { success: true, data: newTeam };
    } catch (err) {
      setError(err.message);
      return { success: false, error: err.message };
    }
  };

  const updateTeam = async (teamId, teamData) => {
    try {
      const updatedTeam = await teamService.updateTeam(teamId, teamData);
      setTeams(prev => prev.map(team => team.id === teamId ? updatedTeam : team));
      return { success: true, data: updatedTeam };
    } catch (err) {
      setError(err.message);
      return { success: false, error: err.message };
    }
  };

  const deleteTeam = async (teamId) => {
    try {
      await teamService.deleteTeam(teamId);
      setTeams(prev => prev.filter(team => team.id !== teamId));
      return { success: true };
    } catch (err) {
      setError(err.message);
      return { success: false, error: err.message };
    }
  };

  useEffect(() => {
    fetchTeams();
  }, []);

  useEffect(() => {
    const handleTeamsRefresh = () => {
      fetchTeams();
    };

    window.addEventListener('dashboardTeamsRefresh', handleTeamsRefresh);
    return () => {
      window.removeEventListener('dashboardTeamsRefresh', handleTeamsRefresh);
    };
  }, []);

  return {
    teams,
    loading,
    error,
    fetchTeams,
    createTeam,
    updateTeam,
    deleteTeam
  };
}; 