# Spyke SDKs

Third-party SDK wrappers UPM package for Spyke games.

## Features

- **IAP** - Unity IAP wrapper, receipt validation
- **Ads** - AppLovin MAX wrapper, ad placements
- **Firebase** - Analytics, Crashlytics wrappers
- **AppsFlyer** - Attribution tracking
- **OneSignal** - Push notifications

## Installation

Add to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.spykegames.sdks": "https://github.com/spykegames/upm-spyke-sdks.git#v1.0.0"
  }
}
```

## Requirements

- Unity 6.3 LTS (6000.0)
- com.spykegames.core
- com.spykegames.services

### External SDKs (install separately)
- Unity IAP
- AppLovin MAX SDK
- Firebase SDK
- AppsFlyer SDK
- OneSignal SDK

## Documentation

See [CLAUDE.md](CLAUDE.md) for detailed documentation.
