import { api } from './api';
import * as signalR from '@microsoft/signalr';

class ChatService {
  constructor() {
    this.connection = null;
    this.messageHandlers = new Set();
  }

  async connect(token) {
    console.log('SignalR: Attempting to connect with token:', token ? 'Token present' : 'No token');
    if (this.connection && this.connection.state === 'Connected') {
      console.log('SignalR: Already connected');
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`/chatHub?access_token=${token}`)
      .withAutomaticReconnect()
      .build();

    this.connection.on('ReceiveMessage', (message) => {
      console.log('SignalR: Received message:', message);
      this.messageHandlers.forEach(handler => handler(message));
    });

    try {
      await this.connection.start();
      console.log('SignalR Connected successfully');
      console.log('SignalR Connection State:', this.connection.state);
    } catch (err) {
      console.error('SignalR Connection Error:', err);
      console.error('SignalR Error Details:', err.message);
    }
  }

  disconnect() {
    if (this.connection) {
      this.connection.stop();
      this.connection = null;
    }
  }

  onMessageReceived(handler) {
    this.messageHandlers.add(handler);
    return () => this.messageHandlers.delete(handler);
  }

  async joinTeam(teamId) {
    console.log('SignalR: Attempting to join team:', teamId);
    console.log('SignalR: Connection state:', this.connection?.state);
    if (this.connection && this.connection.state === 'Connected') {
      try {
        await this.connection.invoke('JoinTeam', teamId);
        console.log('SignalR: Successfully joined team:', teamId);
      } catch (err) {
        console.error('SignalR: Failed to join team:', err);
      }
    } else {
      console.error('SignalR: Cannot join team - connection not ready');
    }
  }

  async leaveTeam(teamId) {
    if (this.connection && this.connection.state === 'Connected') {
      await this.connection.invoke('LeaveTeam', teamId);
    }
  }

  async getTeamMessages(teamId, page = 1, pageSize = 20) {
    try {
      const response = await api.get(`/chat/team/${teamId}?page=${page}&pageSize=${pageSize}`);
      return response.data;
    } catch (error) {
      console.error('Error fetching team messages:', error);
      throw error;
    }
  }

  async sendMessage(teamId, message) {
    try {
      console.log('Sending message:', { teamId, message });
      const response = await api.post('/chat', {
        teamId,
        message
      });
      console.log('Message sent successfully:', response.data);
      return response.data;
    } catch (error) {
      console.error('Error sending message:', error);
      throw error;
    }
  }
}

export const chatService = new ChatService(); 