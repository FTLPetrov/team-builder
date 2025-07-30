import api from './api';

export const invitationService = {
  // Create invitation
  async createInvitation(invitationData) {
    const response = await api.post('/Invitation', invitationData);
    return response.data;
  },

  // Get all invitations for user
  async getUserInvitations() {
    const response = await api.get('/Invitation/user');
    return response.data;
  },

  // Respond to invitation (accept/decline)
  async respondToInvitation(invitationId, response) {
    const apiResponse = await api.post('/Invitation/respond', { 
      invitationId, 
      accept: response === 'accept' 
    });
    return apiResponse.data;
  },

  // Get invitation by ID
  async getInvitationById(id) {
    const response = await api.get(`/Invitation/${id}`);
    return response.data;
  }
}; 