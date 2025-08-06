import { api } from './api';

export const supportService = {
  async createSupportMessage(supportData) {
    const response = await api.post('/support-messages', supportData);
    return response.data;
  },

  async getAllSupportMessages() {
    const response = await api.get('/admin/support-messages');
    return response.data;
  },

  async updateSupportMessage(messageId, updateData) {
    const response = await api.put(`/admin/support-messages/${messageId}`, updateData);
    return response.data;
  },

  async deleteSupportMessage(messageId) {
    const response = await api.delete(`/admin/support-messages/${messageId}`);
    return response.data;
  }
};
