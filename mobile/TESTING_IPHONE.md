## iPhone Testing Guide

### Prereqs

- Install the Expo Go app on your iPhone.
- Ensure your iPhone and dev machine are on the same Wi-Fi network.
- Confirm the backend API is running and reachable from your phone.

### 1. Start the backend so the iPhone can reach it

The API must listen on **all interfaces** (not just localhost) and on the **same port** the app uses (5001).

From `backend/trAInr.API/` run:

```bash
dotnet run --launch-profile http-mobile
```

This uses the `http-mobile` profile and listens on `http://0.0.0.0:5001`. (Port 5001 is used to avoid conflict with macOS AirPlay on 5000.) If you use the default `dotnet run` (port 8080), the app’s default URL won’t match and requests may go nowhere or to the wrong place.

### 2. Set the API base URL for the device

The app reads `EXPO_PUBLIC_API_URL` when the Metro bundler starts (not at runtime). For a physical iPhone, use your computer’s LAN IP and port **5001** (same as `http-mobile`).

Example (replace with your machine’s IP):

```
http://192.168.0.92:5001/api/v1
```

Start Expo with the env var set:

```bash
EXPO_PUBLIC_API_URL="http://192.168.0.92:5001/api/v1" npm run start
```

If you change `EXPO_PUBLIC_API_URL`, restart Expo (stop and run the command again).

### Run on iPhone (Expo Go)

1. From `mobile/`, install dependencies if needed:
   ```
   npm install
   ```
2. Start Expo:
   ```
   EXPO_PUBLIC_API_URL="http://192.168.1.25:5001/api/v1" npm run start
   ```
3. In the Expo DevTools terminal output, scan the QR code with your iPhone camera.
4. Tap the banner to open in Expo Go.

### If you get 403 Forbidden and the request doesn’t appear in the backend terminal

The request is likely **not reaching your backend** (wrong host/port or backend not listening on the network).

1. **Confirm the app’s API URL**  
   In the Metro bundler terminal you should see a log like:  
   `[trAInr API] Base URL: http://....`  
   That must be your machine’s IP and port **5001**, e.g. `http://192.168.0.92:5001/api/v1`. If it shows `localhost` or a different port, set `EXPO_PUBLIC_API_URL` and restart Expo.

2. **Run the backend so the phone can connect**  
   Use the profile that listens on all interfaces and port 5001:

   ```bash
   cd backend/trAInr.API && dotnet run --launch-profile http-mobile
   ```

   Then try login/register again; the request should appear in this terminal.

3. **If you still get 403 and the request does hit the backend**  
   Then it’s usually CORS. Add your Metro/Expo origin (e.g. `http://YOUR_IP:8081`) to `AllowedOrigins` in `appsettings.Development.json` or set `ALLOWED_ORIGINS` when starting the API.

### If the QR code won't load

- Switch Metro to Tunnel mode from the Expo DevTools.
- Ensure your firewall allows incoming connections on the Expo port.
- Re-check that the API URL is reachable from your iPhone browser.

### Quick Test Checklist

- Login with a known account.
- Register a new account (use valid email and password).
- Navigate Today → Programs → Profile tabs.
- Sign out and confirm you return to the login screen.
