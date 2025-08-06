import { useNavigate } from 'react-router-dom';

export const handleApiError = (error, navigate) => {
  console.error('API Error:', error);
  
  const status = error.response?.status;
  
  switch (status) {
    case 401:
      navigate('/error/401');
      break;
    case 403:
      navigate('/error/403');
      break;
    case 404:
      navigate('/error/404');
      break;
    case 500:
      navigate('/error/500');
      break;
    default:

      console.error('Unhandled API error:', error);
      break;
  }
};

export const getErrorMessage = (error) => {
  if (error.response?.data?.message) {
    return error.response.data.message;
  }
  
  if (error.message) {
    return error.message;
  }
  
  return 'An unexpected error occurred';
};

export const isNetworkError = (error) => {
  return !error.response && error.request;
};

export const isServerError = (error) => {
  return error.response?.status >= 500;
};

export const isClientError = (error) => {
  return error.response?.status >= 400 && error.response?.status < 500;
};
