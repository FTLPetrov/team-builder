# TeamBuilder Setup Guide

## Port Configuration

### Frontend (React + Vite)
- **Port**: 5173 (default Vite port)
- **URL**: http://localhost:5173
- **API Proxy**: Configured to proxy `/api` requests to backend

### Backend (.NET Web API)
- **Port**: 5000
- **URL**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

## Development Setup

### 1. Backend Setup
```bash
cd server/WebApi/TeamBuilder.WebApi
dotnet restore
dotnet run
```

The backend will start on `http://localhost:5000` with Swagger UI available.

### 2. Frontend Setup
```bash
cd client
npm install
npm run dev
```

The frontend will start on `http://localhost:5173` and automatically proxy API calls to the backend.

## Configuration Files

### Backend Configuration
- `server/WebApi/TeamBuilder.WebApi/appsettings.json` - Base configuration
- `server/WebApi/TeamBuilder.WebApi/appsettings.Development.json` - Development settings
- `server/WebApi/TeamBuilder.WebApi/appsettings.Production.json` - Production settings
- `server/WebApi/TeamBuilder.WebApi/Properties/launchSettings.json` - Launch configuration

### Frontend Configuration
- `client/src/config/environment.js` - Environment-specific settings
- `client/vite.config.js` - Vite configuration with proxy setup
- `client/env.example` - Example environment variables

## CORS Configuration

The backend is configured to allow requests from:
- http://localhost:5173 (Vite development server)
- http://localhost:3000 (Alternative React dev server)
- http://127.0.0.1:5173
- http://127.0.0.1:3000

## API Proxy

In development, the frontend uses Vite's proxy feature to forward API requests:
- Frontend makes requests to `/api/*`
- Vite proxies these to `http://localhost:5000/api/*`
- This eliminates CORS issues in development

## Production Deployment

For production:
1. Update `client/src/config/environment.js` with your production API URL
2. Update `server/WebApi/TeamBuilder.WebApi/appsettings.Production.json` with your production settings
3. Build the frontend: `npm run build`
4. Deploy the backend to your server
5. Configure your web server to serve the frontend build files

## Troubleshooting

### Port Conflicts
If port 5000 is already in use:
1. Update `server/WebApi/TeamBuilder.WebApi/Properties/launchSettings.json`
2. Update `client/vite.config.js` proxy target
3. Update `client/src/config/environment.js` development API URL

### CORS Issues
If you encounter CORS errors:
1. Check that the backend is running on port 5000
2. Verify CORS settings in `appsettings.Development.json`
3. Ensure the frontend is using the proxy configuration

### Database Connection
The backend uses LocalDB by default. Make sure SQL Server LocalDB is installed and running. 