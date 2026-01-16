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

1. **Render.com PostgreSQL Database**: You have this set up ✅
2. **Render.com Account**: Sign up at [render.com](https://render.com)
3. **Git Repository**: Push your code to GitHub/GitLab
4. **Accept Docker requirement** for .NET backend on Render.com

## Step 1: Prepare Your Render.com PostgreSQL Database

Since you're using Render.com's managed PostgreSQL:

1. Go to Render.com dashboard → PostgreSQL → Your database
2. Copy the **"External Database URL"**
3. Use this URL directly as your `DATABASE_URL`

**Your connection string should look like:**
`postgresql://username:password@host.region-postgres.render.com/database`

**Benefits of Render.com PostgreSQL:**

- ✅ Both services on same platform = better performance
- ✅ Automatic backups and scaling
- ✅ No external network latency
- ✅ Managed by Render.com
- ✅ Your setup is already optimized!

## Step 1.5: Generate JWT Secret

Your JWT secret should be a secure, randomly generated string of at least 32 characters. You can generate one using:

**Option 1: Use Render.com's built-in generator**

- When setting environment variables, Render.com can auto-generate secure values
- Set `JWT_SECRET` as "generated" and Render.com will create a secure random string

**Option 2: Generate locally**

```bash
# On Linux/Mac
openssl rand -base64 32

# Or use Node.js
node -e "console.log(require('crypto').randomBytes(32).toString('base64'))"

# Or use Python
python3 -c "import secrets; print(secrets.token_urlsafe(32))"
```

**Store this securely** - never commit real JWT secrets to your repository!

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

4. **Environment Variables** (REQUIRED):

   ```
   ASPNETCORE_ENVIRONMENT=Production
   DATABASE_URL=postgresql://username:password@host.region-postgres.render.com/database
   JWT_SECRET=[GENERATE_SECURELY_OR_USE_RENDER_GENERATOR]
   JWT_ISSUER=trainr-api
   JWT_AUDIENCE=trainr-frontend
   JWT_EXPIRATION_DAYS=7
   ALLOWED_ORIGINS=https://your-frontend-service.onrender.com
   ```

   **Environment Variable Setup in Render.com**:

   - `DATABASE_URL`: **REQUIRED** - Copy "External Database URL" from your Render.com PostgreSQL service
   - `JWT_SECRET`: Click "Generate" in Render.com to auto-create a secure 32+ character secret
   - `ALLOWED_ORIGINS`: Set after frontend is deployed (use frontend's external URL)

   **Note**: In production, the app uses `DATABASE_URL` environment variable exclusively. The `DefaultConnection` in appsettings is only used for development.

   **⚠️ CRITICAL**: The `DATABASE_URL` environment variable is required in production. Without it, the application will fail to start with a database connection error.

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
DATABASE_URL=postgresql://username:password@host.region-postgres.render.com/database
JWT_SECRET=[GENERATE_SECURELY_OR_USE_RENDER_GENERATOR]
ALLOWED_ORIGINS=https://trainr-frontend.onrender.com

# Frontend
REACT_APP_API_URL=https://trainr-api.onrender.com
```

## Service Communication: Internal vs External URLs

### When to Use Internal vs External URLs

**Internal URLs** (recommended for service-to-service communication):

- Used when one Render.com service calls another Render.com service
- Faster and more secure (not exposed to the internet)
- Format: `https://your-service-name.onrender.com` (same as external but used internally)

**External URLs** (used by end users and external services):

- Public URLs that users access
- Used by your frontend to call the backend API
- Same format as internal URLs but accessed from outside Render.com

### In Your Case:

- **Frontend → Backend**: Use **external URL** for `REACT_APP_API_URL`

  - Users access your frontend externally, and it needs to call the backend
  - Set: `REACT_APP_API_URL=https://your-backend-service.onrender.com`

- **Backend → Database**: Use Railway's connection string (external to Render.com)

- **Future services**: If you add more services that communicate with each other, use internal URLs

## Troubleshooting

### Database Connection Errors

**Error**: "Format of the initialization string does not conform to specification starting at index 0"

**Cause**: The `DATABASE_URL` environment variable is not set or is invalid.

**Solution**:

1. Go to your Railway PostgreSQL database → "Connect" tab
2. Copy the full PostgreSQL connection URL
3. **Replace any placeholders** like `{pass}` with your actual password
4. In Render.com, go to your backend service → Environment
5. Set `DATABASE_URL` to your complete Railway connection string
6. Redeploy the service

**Render.com PostgreSQL Connection String Example**:
`postgresql://trainr:GENERATED_PASSWORD@dpg-d5kf4immcj7s73d5joo0-a.frankfurt-postgres.render.com/trainr_izku`

**Debug Steps for Connection String Issues**:

1. Check Render.com environment variables are set correctly
2. Verify password doesn't contain special characters that need URL encoding
3. Test connection string format: `postgresql://username:password@host/database`
4. Ensure no extra spaces or characters in the environment variable value

### CORS Errors

**Error**: CORS policy blocking requests from frontend

**Solution**: Update the `ALLOWED_ORIGINS` environment variable with your frontend URL after both services are deployed.

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
