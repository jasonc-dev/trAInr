# trAInr Mobile App

React Native mobile application for trAInr, built with Expo.

## Prerequisites

- Node.js 18+
- npm or yarn
- Expo Go app on your mobile device (for testing)
- iOS Simulator (Mac only) or Android Emulator (optional)

## Quick Start

### 1. Install Dependencies

```bash
cd mobile
npm install
```

### 2. Configure API URL

Create a `.env` file (copy from `.env.example`):

```bash
cp .env.example .env
```

Update the `EXPO_PUBLIC_API_URL` in `.env`:

- **For physical device with Expo Go**: Use your computer's network IP address
  ```
  EXPO_PUBLIC_API_URL=http://192.168.1.XXX:5000/api/v1
  ```
- **For iOS Simulator**:
  ```
  EXPO_PUBLIC_API_URL=http://localhost:5000/api/v1
  ```
- **For Android Emulator**:
  ```
  EXPO_PUBLIC_API_URL=http://10.0.2.2:5000/api/v1
  ```

To find your computer's IP:

- **Mac**: `ifconfig | grep "inet " | grep -v 127.0.0.1`
- **Windows**: `ipconfig` (look for IPv4 Address)
- **Linux**: `ip addr show`

### 3. Start the Development Server

```bash
npm start
```

This will open Expo Dev Tools in your browser.

### 4. Run on Device/Simulator

#### Physical Device (Expo Go)

1. Install Expo Go from App Store (iOS) or Play Store (Android)
2. Scan the QR code shown in the terminal
3. Make sure your device is on the same network as your computer

#### iOS Simulator (Mac only)

Press `i` in the terminal

#### Android Emulator

Press `a` in the terminal

## Project Structure

```
mobile/
├── app/                    # App routes (expo-router)
│   ├── (auth)/            # Authentication screens
│   ├── (tabs)/            # Main tab navigation
│   ├── _layout.tsx        # Root layout
│   └── index.tsx          # Entry screen
├── components/            # Reusable components
│   └── ui/               # UI components (Button, Input, Card)
├── lib/                  # Core functionality
│   ├── api/              # API client and services
│   ├── db/               # SQLite database
│   ├── storage/          # Secure storage
│   └── sync/             # Offline sync engine
├── stores/               # Zustand state management
├── theme/                # Design tokens (colors, spacing, typography)
└── .env                  # Environment configuration
```

## Features

### MVP Features (Current)

- ✅ User authentication (login/register)
- ✅ Offline-first architecture with SQLite
- ✅ Secure token storage
- ✅ Dark mode support
- ✅ Tab navigation
- 🚧 Workout tracking
- 🚧 Program management
- 🚧 Offline sync

### Upcoming Features

- Push notifications
- Biometric authentication
- Progress charts and analytics
- Social features

## Testing

### Manual Testing

1. Start the backend server first (see backend/README.md)
2. Start the mobile app
3. Test authentication flow
4. Test offline functionality (enable airplane mode)

### E2E Testing (Maestro)

```bash
# Install Maestro
curl -Ls "https://get.maestro.mobile.dev" | bash

# Run tests
maestro test .maestro/flows/
```

## Troubleshooting

### "Network Error" or "Cannot connect to server"

- Verify the backend is running (`cd backend && dotnet run`)
- Check the `EXPO_PUBLIC_API_URL` in `.env` is correct
- Ensure your device and computer are on the same network
- Try disabling firewall temporarily

### "Module not found" errors

```bash
rm -rf node_modules
npm install
```

### "StatusBar type error" (New Architecture)

- Ensure you're using React Native's StatusBar, not expo-status-bar
- Check `translucent` prop is explicitly `{true}` not shorthand

### Clear cache and restart

```bash
npm start -- --clear
```

## Building for Production

### iOS (App Store)

```bash
eas build --platform ios
eas submit --platform ios
```

### Android (Play Store)

```bash
eas build --platform android
eas submit --platform android
```

See `eas.json` for build configuration.

## Environment Variables

| Variable              | Description     | Default                        |
| --------------------- | --------------- | ------------------------------ |
| `EXPO_PUBLIC_API_URL` | Backend API URL | `http://localhost:5000/api/v1` |

## Tech Stack

- **Framework**: Expo 54 / React Native 0.81
- **Navigation**: Expo Router (file-based)
- **State Management**: Zustand
- **Data Fetching**: TanStack Query (React Query)
- **HTTP Client**: Axios
- **Local Database**: expo-sqlite
- **Secure Storage**: expo-secure-store
- **Form Handling**: React Hook Form + Zod
- **Icons**: @expo/vector-icons (Ionicons)

## Development Notes

### New Architecture

This app uses React Native's New Architecture (`newArchEnabled: true`), which provides:

- Better performance
- Stricter type checking at the native layer
- Improved interop between JS and native code

**Note**: Props must be explicitly typed (e.g., `translucent={true}` not `translucent`)

### Offline-First Design

- All data is stored locally in SQLite
- Changes sync to server when online
- Idempotent operations prevent duplicates
- Optimistic UI updates

## License

Proprietary - trAInr 2026
