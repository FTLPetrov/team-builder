import api from './api';

export const eventService = {
  // Create event
  async createEvent(eventData) {
    const response = await api.post('/Event', eventData);
    return response.data;
  },

  // Get all events for a team
  async getTeamEvents(teamId) {
    const response = await api.get(`/Event/team/${teamId}`);
    return response.data;
  },

  // Get event by ID
  async getEventById(id) {
    const response = await api.get(`/Event/${id}`);
    return response.data;
  },

  // Update event
  async updateEvent(id, eventData) {
    const response = await api.put(`/Event/${id}`, eventData);
    return response.data;
  },

  // Delete event
  async deleteEvent(id) {
    const response = await api.delete(`/Event/${id}`);
    return response.data;
  }
}; 