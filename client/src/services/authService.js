import api from './api';

export const authService = {
  // User registration
  async register(userData) {
    const response = await api.post('/User', userData);
    return response.data;
  },

  // User login
  async login(credentials) {
    const response = await api.post('/User/login', credentials);
    if (response.data.token) {
      localStorage.setItem('authToken', response.data.token);
    }
    return response.data;
  },

  // User logout
  logout() {
    localStorage.removeItem('authToken');
  },

  // Get current user
  async getCurrentUser() {
    const response = await api.get('/User/me');
    return response.data;
  },

  // Update user profile
  async updateProfile(userData) {
    const response = await api.put('/User', userData);
    return response.data;
  },

  // Forgot password
  async forgotPassword(email) {
    const response = await api.post('/User/forgot-password', { email });
    return response.data;
  },

  // Reset password
  async resetPassword(token, newPassword) {
    const response = await api.post('/User/reset-password', { token, newPassword });
    return response.data;
  },

  // Change password
  async changePassword(passwordData) {
    const response = await api.post('/User/change-password', passwordData);
    return response.data;
  },

  // Confirm email
  async confirmEmail(token) {
    const response = await api.post('/User/confirm-email', { token });
    return response.data;
  },

  // Check if user is authenticated
  isAuthenticated() {
    return !!localStorage.getItem('authToken');
  }
}; 