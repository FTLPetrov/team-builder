import api from './api';

export const chatService = {
  // Create chat
  async createChat(chatData) {
    const response = await api.post('/Chat', chatData);
    return response.data;
  },

  // Get team chat
  async getTeamChat(teamId) {
    const response = await api.get(`/Chat/team/${teamId}`);
    return response.data;
  },

  // Get chat by ID
  async getChatById(id) {
    const response = await api.get(`/Chat/${id}`);
    return response.data;
  },

  // Send message to chat
  async sendMessage(chatId, messageData) {
    const response = await api.post(`/Chat/${chatId}/messages`, messageData);
    return response.data;
  },

  // Get chat messages
  async getChatMessages(chatId) {
    const response = await api.get(`/Chat/${chatId}/messages`);
    return response.data;
  }
}; 