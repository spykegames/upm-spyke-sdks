# CLAUDE.md - Spyke SDKs Package

## What This Does
Third-party SDK wrappers providing unified interfaces for IAP, Ads, Firebase, AppsFlyer, and OneSignal across all Spyke games.

## Package Structure

```
upm-spyke-sdks/
├── Runtime/
│   ├── IAP/              ← Unity IAP wrapper, receipt validation
│   ├── Ads/              ← AppLovin MAX wrapper, ad placements
│   ├── Firebase/         ← Analytics, Crashlytics wrappers
│   ├── AppsFlyer/        ← Attribution wrapper
│   ├── OneSignal/        ← Push notification wrapper
│   ├── Facebook/         ← Facebook SDK auth wrapper
│   ├── Localization/     ← I2 Localization wrapper
│   └── Spyke.SDKs.asmdef
├── Editor/
│   └── Spyke.SDKs.Editor.asmdef
├── Tests/
│   ├── Runtime/
│   └── Editor/
├── package.json
└── CLAUDE.md
```

## Key Files

| Folder | Purpose | Status |
|--------|---------|--------|
| `Runtime/IAP/` | In-app purchase handling | Done |
| `Runtime/Ads/` | Ad mediation (AppLovin MAX) | Done |
| `Runtime/Firebase/` | Firebase Analytics & Crashlytics | Done |
| `Runtime/AppsFlyer/` | Attribution tracking | Done |
| `Runtime/OneSignal/` | Push notifications | Done |
| `Runtime/Facebook/` | Facebook SDK auth | Done |
| `Runtime/Localization/` | I2 Localization wrapper | Done |

## How to Use

### Installation
```json
// Packages/manifest.json
{
  "dependencies": {
    "com.spykegames.sdks": "https://github.com/spykegames/upm-spyke-sdks.git#v1.0.0"
  }
}
```

### Basic Usage
```csharp
using Spyke.SDKs.IAP;
using Spyke.SDKs.Ads;

// IAP
[Inject] private readonly IIAPService _iap;
var result = await _iap.PurchaseAsync("gems_100");

// Ads
[Inject] private readonly IAdService _ads;
await _ads.ShowRewardedAsync("double_coins");
```

## Dependencies
- com.spykegames.core (required)
- com.spykegames.services (required)
- Unity IAP (external)
- AppLovin MAX SDK (external)
- Firebase SDK (external)
- AppsFlyer SDK (external)
- OneSignal SDK (external)

## Depends On This
- All game projects

## Source Files to Port

From `client-bootstrap`:
| Source | Destination |
|--------|-------------|
| `SpykeLib/.../Core/Purchase/` | `Runtime/IAP/` |
| `CubeBusters/AdMonetization/` | `Runtime/Ads/` |
| `CubeBusters/AppsFlyers/` | `Runtime/AppsFlyer/` |
| `CubeBusters/OneSignal/` | `Runtime/OneSignal/` |
| Firebase integration | `Runtime/Firebase/` |

## Status
✅ **COMPLETE** - All SDK wrappers implemented (29 files)

### Architecture Decision: Redesigned vs Direct Port
This package was **intentionally redesigned** from client-bootstrap, NOT directly ported:
- Uses UniTask instead of Task (consistent with Project Genesis)
- Cleaner interfaces with proper events (vs Action properties)
- Simplified file structure (29 files vs 60+ in client-bootstrap)
- Game-specific code left to game layer (credential sync, analytics caches, ad handlers)

### Completed
- **Firebase** (5 files): IFirebaseService, FirebaseService, FirebaseAnalyticsProvider, FirebaseCrashlyticsService, FirebaseRemoteConfigService
- **AppsFlyer** (5 files): IAppsFlyerService, IAppsFlyerConversionService, AppsFlyerConversionData, AppsFlyerAnalyticsProvider, AppsFlyerService
- **IAP** (5 files): IIAPService, IAPProduct, IAPResult, IReceiptValidator, IAPService
- **Ads** (8 files): IAdService, IRewardedAdService, IInterstitialAdService, IAdConsentService, AdResult, AdPlacement, MaxRewardedService, MaxInterstitialService
- **OneSignal** (4 files): IOneSignalService, IPushPermissionHelper, NotificationData, OneSignalService
- **Facebook** (1 file): FacebookAuthProvider
- **Localization** (1 file): I2LocalizationService

### Files NOT Ported (By Design - Game Layer)

| client-bootstrap File | Reason | Where to Implement |
|----------------------|--------|-------------------|
| `AppsFlyerInviteLinkService.cs` | Game-specific invite link generation | Game project |
| `AppsFlyerCredentialsSyncController.cs` | Uses game's server endpoints | Game project |
| `OneSignalCredentialsSyncController.cs` | Uses game's server endpoints | Game project |
| `AdPlacementHandler/*` (5 files) | Game-specific ad placement logic | Game project |
| `AdAnalytics.cs`, `AdWatchCountAnalytics.cs` | Game-specific analytics | Game project |
| `PurchaseAnalyticsCache.cs` | Game-specific purchase tracking | Game project |
| `AdSyncController.cs`, `AdSyncWebTask.cs` | Server sync for ad rewards | Game project |

### Conditional Compilation
All SDK wrappers use conditional compilation to avoid compile errors when native SDKs aren't installed:
- `#if FIREBASE_ANALYTICS` - Firebase Analytics SDK
- `#if FIREBASE_CRASHLYTICS` - Firebase Crashlytics SDK
- `#if FIREBASE_REMOTE_CONFIG` - Firebase Remote Config SDK
- `#if APPSFLYER_SDK` - AppsFlyer SDK
- `#if UNITY_PURCHASING` - Unity IAP
- `#if APPLOVIN_MAX` - AppLovin MAX SDK
- `#if ONESIGNAL_SDK` - OneSignal SDK
- `#if FACEBOOK_SDK` - Facebook SDK
- `#if I2_LOCALIZATION` - I2 Localization

Games define these symbols in Player Settings when they have the SDKs installed.
