# trAInr Deployment Guide - Render.com

This guide will help you deploy your full-stack trAInr application to Render.com.

## Important Note: Docker Required for .NET Backend

**Render.com requires Docker for .NET applications.** While your frontend can be deployed without Docker as a static site, your .NET backend needs Docker deployment on Render.com.

If you absolutely want to avoid Docker, consider these alternatives:

- **Azure App Service** (has native .NET support without Docker)
- **AWS Elastic Beanstalk**
- **DigitalOcean App Platform**
- **Heroku** (with .NET buildpack)

But for Render.com deployment, Docker is required for the backend.

## What I've Prepared

✅ **Backend Configuration**:

- Updated `Program.cs` to handle environment variables
- Created `appsettings.Production.json` for production settings
- Fixed `backend/Dockerfile` to work with Render.com's build context

✅ **Frontend Configuration**:

- Updated `package.json` with production build script
- Frontend is configured to use `REACT_APP_API_URL` environment variable

✅ **Deployment Guide**: This comprehensive guide

## Prerequisites

1. **Railway PostgreSQL Database**: You mentioned you already have this set up
2. **Render.com Account**: Sign up at [render.com](https://render.com)
3. **Git Repository**: Push your code to GitHub/GitLab
4. **Accept Docker requirement** for .NET backend on Render.com

## Step 1: Prepare Your Railway Database

1. Go to your Railway dashboard
2. Select your PostgreSQL database
3. Go to the "Connect" tab
4. Copy the PostgreSQL connection URL (it should look like: `postgresql://postgres:[PASSWORD]@[HOST]:[PORT]/railway`)

## Step 2: Deploy to Render.com

### Manual Service Creation

**Important Note**: Render.com currently requires Docker for .NET applications. Since you want to avoid Docker, you have a few options:

1. **Use Docker anyway** (recommended for Render.com deployment)
2. **Deploy to a different platform** like Azure, AWS, or DigitalOcean
3. **Use Render.com for frontend only** and deploy backend elsewhere

For completeness, here are the manual setup instructions:

#### Backend Service (Using Docker)

1. Click "New" → "Web Service"
2. Connect your repository
3. Configure build settings:

   - **Environment**: `Docker`
   - **Dockerfile Path**: `backend/Dockerfile`

4. **Environment Variables**:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   DATABASE_URL=your_railway_connection_string
   JWT_SECRET=your_super_secret_key_at_least_32_chars
   JWT_ISSUER=trainr-api
   JWT_AUDIENCE=trainr-frontend
   JWT_EXPIRATION_DAYS=7
   ALLOWED_ORIGINS=https://your-frontend-service.onrender.com
   ```

#### Alternative: Backend on Different Platform

If you want to avoid Docker completely, consider deploying your .NET backend to:

- **Azure App Service** (free tier available)
- **AWS Elastic Beanstalk**
- **DigitalOcean App Platform**
- **Heroku** (with .NET buildpack)

#### Frontend Service

1. Click "New" → "Static Site"
2. Connect your repository
3. Configure build settings:

   - **Build Command**: `cd frontend/trainr && npm install && npm run build:production`
   - **Publish Directory**: `frontend/trainr/build`

4. **Environment Variables**:
   ```
   REACT_APP_API_URL=https://your-backend-service-url
   ```

### Quick Deployment Steps:

1. **Deploy Frontend First**:

   - Create Static Site as described above
   - Note the URL (e.g., `https://trainr-frontend.onrender.com`)

2. **Deploy Backend** (choose one option):

   **Option A: Use Docker on Render.com**

   - Create Web Service with Docker environment
   - Use your existing Dockerfile: `backend/trAInr.API/Dockerfile`

   **Option B: Deploy Backend Elsewhere**

   - Use Azure App Service, AWS, or DigitalOcean for .NET without Docker
   - Update `REACT_APP_API_URL` with your backend URL

## Step 3: Database Migration

Your backend will automatically run database migrations on startup. The exercise definitions will also be seeded automatically if the table is empty.

## Step 4: Update CORS (After Deployment)

Once both services are deployed:

1. Get your frontend URL (e.g., `https://trainr-frontend.onrender.com`)
2. Update the `ALLOWED_ORIGINS` environment variable in your backend service
3. Redeploy the backend service

## Step 5: Verify Deployment

1. **Backend API**: Visit `https://your-backend-service.onrender.com/scalar/v1` to see the API documentation
2. **Frontend**: Visit your frontend URL and try logging in/registering
3. **Database**: Check your Railway logs to ensure migrations ran successfully

## Troubleshooting

### Backend Issues

- **Database connection fails**: Double-check your Railway connection string
- **Migrations fail**: Check Render logs for specific error messages
- **JWT secret too short**: Make sure your JWT_SECRET is at least 32 characters

### Frontend Issues

- **API calls fail**: Check that REACT_APP_API_URL is set correctly
- **CORS errors**: Ensure ALLOWED_ORIGINS includes your frontend URL
- **Build fails**: Make sure all dependencies are listed in package.json

### Common Environment Variables

```
# Backend
DATABASE_URL=postgresql://postgres:password@host:5432/railway
JWT_SECRET=your_32_character_minimum_secret_key_here
ALLOWED_ORIGINS=https://trainr-frontend.onrender.com

# Frontend
REACT_APP_API_URL=https://trainr-api.onrender.com
```

## Security Notes

- Never commit real secrets to your repository
- Use Render's environment variable system for all sensitive data
- Consider enabling HTTPS-only traffic in Render settings
- Regularly rotate your JWT secret

## Performance Tips

- Render free tier has sleep mode - services may take a few seconds to wake up
- Consider upgrading to paid plans for better performance
- Monitor your Railway database usage and upgrade if needed

## Need Help?

Check the Render.com documentation or their community forums for additional support.
