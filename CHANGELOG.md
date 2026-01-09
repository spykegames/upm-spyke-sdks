# Changelog

All notable changes to this project will be documented in this file.

## [1.0.1] - 2025-01-09

### Changed
- Updated CLAUDE.md with PORT-CHECK verification results
- Documented architecture decision: Redesigned vs Direct Port
- Listed files intentionally NOT ported (game-specific implementations)

### Verified
- Firebase: 5 files (Analytics, Crashlytics, Remote Config)
- AppsFlyer: 5 files (Analytics provider, conversion data)
- IAP: 5 files (UniTask-based purchase service)
- Ads: 8 files (AppLovin MAX rewarded/interstitial)
- OneSignal: 4 files (push notifications)
- Facebook: 1 file (auth provider)
- Localization: 1 file (I2 wrapper)

## [1.0.0] - 2025-01-08

### Added
- Full SDK wrapper implementation (29 files)
- Firebase/IFirebaseService, FirebaseService, FirebaseAnalyticsProvider
- Firebase/FirebaseCrashlyticsService, FirebaseRemoteConfigService
- AppsFlyer/IAppsFlyerService, IAppsFlyerConversionService, AppsFlyerService
- AppsFlyer/AppsFlyerConversionData, AppsFlyerAnalyticsProvider
- IAP/IIAPService, IAPService, IAPProduct, IAPResult, IReceiptValidator
- Ads/IAdService, IRewardedAdService, IInterstitialAdService, IAdConsentService
- Ads/AdResult, AdPlacement, MaxRewardedService, MaxInterstitialService
- OneSignal/IOneSignalService, IPushPermissionHelper, NotificationData, OneSignalService
- Facebook/FacebookAuthProvider
- Localization/I2LocalizationService
- Conditional compilation for all SDKs

## [0.0.1-preview] - 2024-12-24

### Added
- Initial package structure
- CLAUDE.md documentation
- Assembly definitions for Runtime, Editor, Tests
