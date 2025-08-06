import { api } from './api';

export const teamService = {

  async getAllTeams() {
    const response = await api.get('/Team');
    return response.data;
  },


  async getUserTeams() {
    const response = await api.get('/Team/user');
    return response.data;
  },


  async getUserOrganizedTeams() {
    const response = await api.get('/Team/user/organized');
    return response.data;
  },


  async getTeamById(id) {
    const response = await api.get(`/Team/${id}`);
    return response.data;
  },


  async createTeam(teamData) {
    const response = await api.post('/Team', teamData);
    return response.data;
  },


  async updateTeam(id, teamData) {
    const response = await api.put(`/Team/${id}`, teamData);
    return response.data;
  },


  async deleteTeam(id) {
    const response = await api.delete(`/Team/${id}`);
    return response.data;
  },


  async joinTeam(teamId) {
    const response = await api.post(`/Team/${teamId}/join`);
    return response.data;
  },


  async leaveTeam(teamId) {
    const response = await api.post(`/Team/${teamId}/leave`);
    return response.data;
  },


  async kickMember(teamId, userId) {
    const response = await api.post(`/Team/${teamId}/kick?userId=${userId}`);
    return response.data;
  },


  async assignRole(teamId, userId, role) {
    const response = await api.post(`/Team/${teamId}/assign-role?userId=${userId}&role=${role}`);
    return response.data;
  },


  async transferOwnership(teamId, newOrganizerId) {
    const response = await api.post(`/Team/${teamId}/transfer-ownership?newOrganizerId=${newOrganizerId}`);
    return response.data;
  }
}; 