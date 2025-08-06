import { api } from './api';

class AdminService {

  async getAllUsers() {
    const response = await api.get('/admin/users');
    return response.data;
  }

  async getUserById(userId) {
    const response = await api.get(`/admin/users/${userId}`);
    return response.data;
  }

  async updateUserPassword(userId, newPassword) {
    const response = await api.put(`/admin/users/${userId}/password`, newPassword);
    return response.data;
  }

  async updateUserDetails(userId, userData) {
    const response = await api.put(`/admin/users/${userId}/details`, userData);
    return response.data;
  }

  async deleteUser(userId) {
    const response = await api.delete(`/admin/users/${userId}`);
    return response.data;
  }

  async recoverUser(userId) {
    const response = await api.put(`/admin/users/${userId}/recover`);
    return response.data;
  }

  async warnUser(userId, warningMessage) {
    const response = await api.post(`/admin/users/${userId}/warnings`, {
      message: warningMessage
    });
    return response.data;
  }


  async getAllTeams() {
    const response = await api.get('/admin/teams');
    return response.data;
  }

  async getTeamById(teamId) {
    const response = await api.get(`/admin/teams/${teamId}`);
    return response.data;
  }

  async deleteTeam(teamId) {
    const response = await api.delete(`/admin/teams/${teamId}`);
    return response.data;
  }

  async joinTeam(teamId, userId) {
    const response = await api.post(`/admin/teams/${teamId}/join/${userId}`);
    return response.data;
  }

  async kickUserFromTeam(teamId, userId) {
    const response = await api.delete(`/admin/teams/${teamId}/members/${userId}`);
    return response.data;
  }

  async clearTeamChat(teamId) {
    const response = await api.delete(`/admin/teams/${teamId}/chat`);
    return response.data;
  }


  async getAllEvents() {
    const response = await api.get('/admin/events');
    return response.data;
  }

  async getEventById(eventId) {
    const response = await api.get(`/admin/events/${eventId}`);
    return response.data;
  }

  async deleteEvent(eventId) {
    const response = await api.delete(`/admin/events/${eventId}`);
    return response.data;
  }


  async getAllSupportMessages() {
    const response = await api.get('/admin/support-messages');
    return response.data;
  }

  async getSupportMessageById(messageId) {
    const response = await api.get(`/admin/support-messages/${messageId}`);
    return response.data;
  }

  async markSupportMessageAsRead(messageId) {
    const response = await api.put(`/admin/support-messages/${messageId}/read`);
    return response.data;
  }

  async toggleSupportMessageFavorite(messageId) {
    const response = await api.put(`/admin/support-messages/${messageId}/favorite`);
    return response.data;
  }

  async markSupportMessageAsCompleted(messageId) {
    const response = await api.put(`/admin/support-messages/${messageId}/completed`);
    return response.data;
  }

  async deleteSupportMessage(messageId) {
    const response = await api.delete(`/admin/support-messages/${messageId}`);
    return response.data;
  }


  async createAnnouncement(announcementData) {
    const response = await api.post('/admin/announcements', announcementData);
    return response.data;
  }

  async getAllAnnouncements() {
    const response = await api.get('/admin/announcements');
    return response.data;
  }

  async deleteAnnouncement(announcementId) {
    const response = await api.delete(`/admin/announcements/${announcementId}`);
    return response.data;
  }

  async toggleAnnouncementActive(announcementId) {
    const response = await api.put(`/admin/announcements/${announcementId}/toggle`);
    return response.data;
  }


  async getAllWarnings() {
    const response = await api.get('/admin/warnings');
    return response.data;
  }

  async deleteWarning(warningId) {
    const response = await api.delete(`/admin/warnings/${warningId}`);
    return response.data;
  }
}

export const adminService = new AdminService(); 