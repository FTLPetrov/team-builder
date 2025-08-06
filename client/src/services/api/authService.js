import { api } from './api';

export const authService = {
  async login(credentials) {
    const response = await api.post('/user/login', credentials);
    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
    }
    return response.data;
  },

  async register(userData) {
    const response = await api.post('/user', userData);
    return response.data;
  },

  async getCurrentUser() {
    const response = await api.get('/user/me');
    return response.data;
  },

  async updateProfile(userData) {
    const response = await api.put('/user/profile', userData);
    return response.data;
  },

  async uploadProfilePicture(formData) {
    const response = await api.put('/user/profile-picture', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },

  async changePassword(passwordData) {
    const response = await api.put('/user/change-password', passwordData);
    return response.data;
  },

  logout() {
    localStorage.removeItem('token');
  },

  isAuthenticated() {
    return !!localStorage.getItem('token');
  }
}; 