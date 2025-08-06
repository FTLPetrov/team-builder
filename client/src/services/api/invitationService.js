import { api } from './api';

export const invitationService = {

  async getUserInvitations() {
    try {
      const response = await api.get('/Invitation/user');
      console.log('InvitationService: Fetched invitations:', response.data);
      return response.data;
    } catch (error) {
      console.error('Error fetching invitations:', error);
      throw error;
    }
  },


  async createInvitation(invitationData) {
    try {
      console.log('InvitationService: Sending invitation request:', invitationData);
      const response = await api.post('/Invitation/by-email', invitationData);
      console.log('InvitationService: Received response:', response.data);
      return response.data;
    } catch (error) {
      console.error('Error creating invitation:', error);
      console.error('Error response:', error.response?.data);
      console.error('Error status:', error.response?.status);
      throw error;
    }
  },


  async respondToInvitation(invitationId, response) {
    try {
      console.log('InvitationService: Sending response request:', { invitationId, response });
      const result = await api.post('/Invitation/respond', { 
        InvitationId: invitationId, 
        Accept: response === 'accept' 
      });
      console.log('InvitationService: Received response:', result.data);
      return result.data;
    } catch (error) {
      console.error('Error responding to invitation:', error);
      console.error('Error response:', error.response?.data);
      console.error('Error status:', error.response?.status);
      throw error;
    }
  }
}; 