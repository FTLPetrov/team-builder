import api from './api';

export const announcementService = {
  async getActiveAnnouncements() {
    const response = await api.get('/announcement');
    return response.data;
  }
}; 