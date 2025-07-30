import api from './api';

export const teamService = {
  // Get all teams
  async getAllTeams() {
    const response = await api.get('/Team');
    return response.data;
  },

  // Get user's teams
  async getUserTeams() {
    const response = await api.get('/Team/user');
    return response.data;
  },

  // Get team by ID
  async getTeamById(id) {
    const response = await api.get(`/Team/${id}`);
    return response.data;
  },

  // Create new team
  async createTeam(teamData) {
    const response = await api.post('/Team', teamData);
    return response.data;
  },

  // Update team
  async updateTeam(id, teamData) {
    const response = await api.put(`/Team/${id}`, teamData);
    return response.data;
  },

  // Delete team
  async deleteTeam(id) {
    const response = await api.delete(`/Team/${id}`);
    return response.data;
  },

  // Join team
  async joinTeam(teamId) {
    const response = await api.post(`/Team/${teamId}/join`);
    return response.data;
  },

  // Leave team
  async leaveTeam(teamId) {
    const response = await api.post(`/Team/${teamId}/leave`);
    return response.data;
  },

  // Kick member from team
  async kickMember(teamId, userId) {
    const response = await api.post(`/Team/${teamId}/kick?userId=${userId}`);
    return response.data;
  },

  // Assign role to team member
  async assignRole(teamId, userId, role) {
    const response = await api.post(`/Team/${teamId}/assign-role?userId=${userId}&role=${role}`);
    return response.data;
  },

  // Transfer team ownership
  async transferOwnership(teamId, newOrganizerId) {
    const response = await api.post(`/Team/${teamId}/transfer-ownership?newOrganizerId=${newOrganizerId}`);
    return response.data;
  }
}; 