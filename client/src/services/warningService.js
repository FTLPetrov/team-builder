import api from './api';

export const warningService = {
  async getUserWarnings() {
    const response = await api.get('/user/warnings');
    return response.data;
  }
}; 