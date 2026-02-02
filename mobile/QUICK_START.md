# Quick Start Guide

## Installation Complete! ✅

Dependencies have been successfully installed with compatible versions for Expo SDK 54.

## Start the App Now

```bash
cd /Users/jasoncohen/Development/projects/trAInr/mobile
npm start
```

This will:

1. Start the Expo development server
2. Show a QR code in your terminal
3. Open Expo Dev Tools in your browser

## Test on Device

### Option 1: Physical Device (Recommended for Quick Testing)

1. Install **Expo Go** app:

   - iOS: [App Store](https://apps.apple.com/app/expo-go/id982107779)
   - Android: [Play Store](https://play.google.com/store/apps/details?id=host.exp.exponent)

2. Scan the QR code shown in terminal:

   - iOS: Use Camera app
   - Android: Use Expo Go app

3. **Important**: Configure API URL in `.env`:

   ```bash
   # Get your computer's IP address
   ifconfig | grep "inet " | grep -v 127.0.0.1

   # Update .env file
   EXPO_PUBLIC_API_URL=http://YOUR_IP_HERE:5000/api/v1
   ```

### Option 2: iOS Simulator (Mac Only)

```bash
npm run ios
```

### Option 3: Android Emulator

```bash
npm run android
```

## Troubleshooting

### Package Version Compatibility Warnings from Expo

These warnings are now fixed! All packages have been updated to match Expo SDK 54 recommendations:

**Fixed:** ✅

- React updated to 19.1.0
- React Native updated to 0.81.5
- Expo Router updated to 6.0.23
- All expo-\* packages updated to latest compatible versions

See `EXPO_SDK54_COMPATIBILITY_FIXED.md` for complete details.

### "npm error ERESOLVE could not resolve" or "UNMET DEPENDENCY" errors

This was a peer dependency conflict with `@types/react`. It's now fixed!

**Status:** ✅ RESOLVED - All 978 packages installed successfully

If you still encounter install errors:

```bash
cd /Users/jasoncohen/Development/projects/trAInr/mobile
rm -rf node_modules package-lock.json
npm install
```

See `NPM_INSTALL_FIXED.md` for technical details.

### "Cannot connect to server"

- Ensure backend is running: `cd backend && dotnet run`
- Check `.env` has correct IP address (use your network IP, not localhost, for physical devices)
- Firewall might be blocking connections

### "Module not found" errors

```bash
npm start -- --clear
```

### App crashes on launch

- Check terminal for specific error messages
- Ensure backend API is accessible

### App crashes or shows native errors on iPhone

**Status:** ✅ FIXED

Common errors like "UIViewControllerBasedStatusBarAppearance" and "RNSModalScreenShadowNode" are now resolved by using stable versions compatible with Expo Go.

**Fixes applied:**

- Reverted to React 18.3.1 (from 19.1.0)
- Reverted to React Native 0.76.5 (from 0.81.5)
- Reverted to Expo Router 5.0 (from 6.0.23)
- Removed problematic native status bar configuration
- Simplified screen options

See `IOS_STATUS_BAR_FIX.md` for complete details.

### "PlatformConstants could not be found" / TurboModuleRegistry error on iPhone

**This is a known Expo Go cache/dependency issue. Follow these exact steps:**

1. **Stop Expo** (press Ctrl+C in terminal)

2. **Clean everything:**

   ```bash
   cd /Users/jasoncohen/Development/projects/trAInr/mobile
   rm -rf node_modules .expo dist web-build
   npm install
   ```

3. **Start with cache clear:**

   ```bash
   npx expo start --clear
   ```

4. **Scan QR code again** with iPhone Camera app

**If still failing**, also run:

```bash
watchman watch-del-all  # Clear watchman cache
rm -rf ~/.expo  # Clear Expo global cache
npm install
npx expo start --clear
```

See **`FIX_PLATFORMCONSTANTS_ERROR.md`** for complete troubleshooting steps and alternative solutions.

## What's Working

✅ Fixed StatusBar type error (React Native New Architecture compatible)
✅ Dependencies installed with correct versions
✅ Authentication UI ready (login/register)
✅ Tab navigation structure
✅ Dark mode support
✅ Secure storage configured
✅ Error handling implemented

## What's Next

1. **Start backend** (if not running):

   ```bash
   cd backend
   dotnet run
   ```

2. **Start mobile app**:

   ```bash
   npm start
   ```

3. **Test authentication flow**:

   - Register new account
   - Login with credentials
   - Navigate between tabs
   - Toggle dark mode

4. **Review checklist**: See `MVP_TESTING_CHECKLIST.md`

## Key Files Modified

- `package.json` - Fixed version incompatibilities
- `app/_layout.tsx` - Fixed StatusBar and dependencies
- `.env` - API URL configuration
- `lib/api/client.ts` - Better error messages
- `app.json` - Set `newArchEnabled: false` for Expo Go compatibility

## Tech Stack

- Expo SDK 54
- React Native 0.76.5
- React 18.3.1
- Expo Router 5.x
- TypeScript 5.9
- Zustand + React Query

## Need Help?

- Check `README.md` for detailed setup
- Check `MVP_TESTING_CHECKLIST.md` for testing guide
- Check `FIXES_AND_NEXT_STEPS.md` for implementation details
- Check `FIX_PLATFORMCONSTANTS_ERROR.md` for TurboModuleRegistry error solution

---

**Ready to test!** Run `npm start` to begin.
