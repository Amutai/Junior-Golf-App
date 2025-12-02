# Mobile Application

React Native mobile app for Junior Golfers Kenya membership.

## Features

- Cross-platform (iOS/Android)
- QR code scanning for club entry
- Push notifications
- Offline membership verification
- NFC support for smart cards

## Development

```bash
# Install dependencies
npm install

# Start Expo development server
npm start

# Run on Android
npm run android

# Run on iOS
npm run ios
```

## Environment Variables

```env
EXPO_PUBLIC_API_URL=http://localhost:8000
EXPO_PUBLIC_STRIPE_PUBLISHABLE_KEY=pk_test_...
```

## Build

```bash
# Build for production
expo build:android
expo build:ios
```