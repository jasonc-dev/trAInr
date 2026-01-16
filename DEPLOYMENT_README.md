# trAInr Deployment Guide - Render.com (Without Docker)

This guide will help you deploy your full-stack trAInr application to Render.com without using Docker containers.

## Prerequisites

1. **Railway PostgreSQL Database**: You mentioned you already have this set up
2. **Render.com Account**: Sign up at [render.com](https://render.com)
3. **Git Repository**: Your code should be in a Git repository

## Step 1: Prepare Your Railway Database

1. Go to your Railway dashboard
2. Select your PostgreSQL database
3. Go to the "Connect" tab
4. Copy the PostgreSQL connection URL (it should look like: `postgresql://postgres:[PASSWORD]@[HOST]:[PORT]/railway`)

## Step 2: Deploy to Render.com

### Option A: Using render.yaml (Recommended)

1. **Push your code to GitHub/GitLab**
   - Make sure all the files we created/modified are committed:
     - `render.yaml`
     - `backend/trAInr.API/appsettings.Production.json`
     - Updated `Program.cs`
     - Updated `package.json`

2. **Connect your repository to Render.com**
   - Go to [render.com](https://render.com) and sign in
   - Click "New" → "Blueprint"
   - Connect your Git repository
   - Render should automatically detect the `render.yaml` file

3. **Configure Environment Variables**
   - After connecting, you'll need to set the `DATABASE_URL` environment variable
   - Go to your backend service settings
   - Add environment variable: `DATABASE_URL` with your Railway connection string

### Option B: Manual Service Creation

If render.yaml doesn't work, create services manually:

#### Backend Service
1. Click "New" → "Web Service"
2. Connect your repository
3. Configure build settings:
   - **Runtime**: `Dotnet`
   - **Build Command**: `cd backend/trAInr.API && dotnet publish -c Release -o published`
   - **Start Command**: `cd backend/trAInr.API/published && dotnet trAInr.API.dll`

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

#### Frontend Service
1. Click "New" → "Static Site"
2. Connect your repository (same repo)
3. Configure build settings:
   - **Build Command**: `cd frontend/trainr && npm install && npm run build:production`
   - **Publish Directory**: `frontend/trainr/build`

4. **Environment Variables**:
   ```
   REACT_APP_API_URL=https://your-backend-service.onrender.com
   ```

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