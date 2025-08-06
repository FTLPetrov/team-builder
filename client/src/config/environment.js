
const environment = {
  development: {
    apiBaseUrl: '/api', // Use proxy in development
    backendUrl: 'http://localhost:5000', // Backend URL for static files
    appName: 'TeamBuilder',
    appVersion: '1.0.0',
    environment: 'development'
  },
  production: {
    apiBaseUrl: 'https://your-production-domain.com/api',
    backendUrl: 'https://your-production-domain.com', // Backend URL for static files
    appName: 'TeamBuilder',
    appVersion: '1.0.0',
    environment: 'production'
  }
};


const currentEnv = import.meta.env.MODE || 'development';


export const config = environment[currentEnv] || environment.development;


export const API_BASE_URL = config.apiBaseUrl;
export const BACKEND_URL = config.backendUrl;
export const APP_NAME = config.appName;
export const APP_VERSION = config.appVersion;
export const ENVIRONMENT = config.environment;

export default config; 