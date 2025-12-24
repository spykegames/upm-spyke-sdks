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
| `Runtime/IAP/` | In-app purchase handling | To port |
| `Runtime/Ads/` | Ad mediation (AppLovin MAX) | To port |
| `Runtime/Firebase/` | Firebase Analytics & Crashlytics | To port |
| `Runtime/AppsFlyer/` | Attribution tracking | To port |
| `Runtime/OneSignal/` | Push notifications | To port |

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
IN DEVELOPMENT - Structure ready, waiting for code port
