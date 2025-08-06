import { api } from './api';

export const announcementService = {
  async getActiveAnnouncements() {
    const response = await api.get('/announcement');
    return response.data;
  },

  async getAllAnnouncements() {
    const response = await api.get('/admin/announcements');
    return response.data;
  },

  async createAnnouncement(announcementData) {
    const response = await api.post('/admin/announcements', announcementData);
    return response.data;
  },

  async deleteAnnouncement(announcementId) {
    const response = await api.delete(`/admin/announcements/${announcementId}`);
    return response.data;
  },

  async toggleAnnouncementActive(announcementId) {
    const response = await api.put(`/admin/announcements/${announcementId}/toggle`);
    return response.data;
  }
}; 