import { api } from './api';

export const eventService = {

  async getAllEvents() {
    const response = await api.get('/Event');
    return response.data;
  },


  async getUserEvents() {
    const response = await api.get('/Event/user');
    return response.data;
  },


  async getTeamEvents(teamId) {
    const response = await api.get(`/Event/team/${teamId}`);
    return response.data;
  },


  async getEventById(id) {
    const response = await api.get(`/Event/${id}`);
    return response.data;
  },


  async createEvent(eventData) {
    const response = await api.post('/Event', eventData);
    return response.data;
  },

  async updateEvent(id, eventData) {
    const response = await api.put(`/Event/${id}`, eventData);
    return response.data;
  },


  async deleteEvent(id) {
    const response = await api.delete(`/Event/${id}`);
    return response.data;
  },


  async joinEvent(eventId) {
    const response = await api.post(`/Event/${eventId}/join`);
    return response.data;
  },


  async leaveEvent(eventId) {
    const response = await api.post(`/Event/${eventId}/leave`);
    return response.data;
  },


  async updateMemberRole(eventId, userId, role) {
    const response = await api.post(`/Event/${eventId}/member-role`, {
      userId,
      role
    });
    return response.data;
  }
}; 