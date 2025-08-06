import { api } from './api';

export const warningService = {
  async getUserWarnings() {
    const response = await api.get('/user/warnings');
    return response.data;
  },

  async getAllWarnings() {
    const response = await api.get('/admin/warnings');
    return response.data;
  },

  async createWarning(userId, warningData) {
    const response = await api.post(`/admin/users/${userId}/warnings`, warningData);
    return response.data;
  },

  async deleteWarning(warningId) {
    const response = await api.delete(`/admin/warnings/${warningId}`);
    return response.data;
  }
}; 