using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Spyke.SDKs.Ads.AppLovin
{
    /// <summary>
    /// AppLovin MAX interstitial ad service implementation.
    /// Requires APPLOVIN_MAX define and MAX SDK installed.
    /// </summary>
    public class MaxInterstitialService : IInterstitialAdService, IInitializable, IDisposable
    {
        private readonly string _adUnitId;
        private bool _isReady;
        private UniTaskCompletionSource<AdResult> _showTcs;
        private string _currentPlacement;

        public bool IsReady => _isReady;

        public event Action<string> OnAdLoaded;
        public event Action<string, string> OnAdLoadFailed;
        public event Action<string> OnAdDisplayed;
        public event Action<string, string> OnAdDisplayFailed;
        public event Action<string> OnAdHidden;
        public event Action<AdRevenueData> OnAdRevenue;

        public MaxInterstitialService(string adUnitId)
        {
            _adUnitId = adUnitId;
        }

        [Inject]
        public void Construct()
        {
            // Injected constructor
        }

        public void Initialize()
        {
#if APPLOVIN_MAX
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialAdLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialAdLoadFailed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialAdDisplayed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdDisplayFailed;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialAdHidden;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialAdRevenuePaid;

            Load();
            Log("MaxInterstitialService initialized");
#else
            Log("AppLovin MAX SDK not installed (APPLOVIN_MAX not defined)");
#endif
        }

        public void Dispose()
        {
#if APPLOVIN_MAX
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnInterstitialAdLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnInterstitialAdLoadFailed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent -= OnInterstitialAdDisplayed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= OnInterstitialAdDisplayFailed;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnInterstitialAdHidden;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= OnInterstitialAdRevenuePaid;
#endif
        }

        public void Load()
        {
#if APPLOVIN_MAX
            MaxSdk.LoadInterstitial(_adUnitId);
            Log($"Loading interstitial ad: {_adUnitId}");
#endif
        }

        public void Show(string placement)
        {
            ShowAsync(placement).Forget();
        }

        public async UniTask<AdResult> ShowAsync(string placement)
        {
            if (!_isReady)
            {
                Log($"Interstitial ad not ready for placement: {placement}");
                return AdResult.NotReady(placement);
            }

            _showTcs = new UniTaskCompletionSource<AdResult>();
            _currentPlacement = placement;

#if APPLOVIN_MAX
            MaxSdk.ShowInterstitial(_adUnitId, placement);
#else
            _showTcs.TrySetResult(AdResult.Failed(placement, "MAX SDK not installed"));
#endif

            return await _showTcs.Task;
        }

#if APPLOVIN_MAX
        private void OnInterstitialAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != _adUnitId) return;

            _isReady = true;
            Log($"Interstitial ad loaded: {adUnitId}");
            OnAdLoaded?.Invoke(adUnitId);
        }

        private void OnInterstitialAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            if (adUnitId != _adUnitId) return;

            _isReady = false;
            Log($"Interstitial ad failed to load: {errorInfo.Message}", true);
            OnAdLoadFailed?.Invoke(adUnitId, errorInfo.Message);

            // Retry after delay
            RetryLoad().Forget();
        }

        private async UniTaskVoid RetryLoad()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            Load();
        }

        private void OnInterstitialAdDisplayed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != _adUnitId) return;

            Log($"Interstitial ad displayed: {adUnitId}");
            OnAdDisplayed?.Invoke(adUnitId);
        }

        private void OnInterstitialAdDisplayFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != _adUnitId) return;

            Log($"Interstitial ad failed to display: {errorInfo.Message}", true);
            OnAdDisplayFailed?.Invoke(adUnitId, errorInfo.Message);

            _showTcs?.TrySetResult(AdResult.Failed(_currentPlacement, errorInfo.Message));
            Load();
        }

        private void OnInterstitialAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != _adUnitId) return;

            Log($"Interstitial ad hidden: {adUnitId}");
            OnAdHidden?.Invoke(adUnitId);

            _showTcs?.TrySetResult(AdResult.Success(_currentPlacement, adUnitId));
            _isReady = false;
            Load();
        }

        private void OnInterstitialAdRevenuePaid(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != _adUnitId) return;

            var revenueData = new AdRevenueData
            {
                AdUnitId = adUnitId,
                NetworkName = adInfo.NetworkName,
                Placement = adInfo.Placement,
                Revenue = adInfo.Revenue,
                Currency = "USD"
            };

            Log($"Interstitial ad revenue: ${adInfo.Revenue} from {adInfo.NetworkName}");
            OnAdRevenue?.Invoke(revenueData);
        }
#endif

        private static void Log(string message, bool isError = false)
        {
#if SPYKE_DEV
            if (isError)
                Debug.LogError($"[MaxInterstitialService] {message}");
            else
                Debug.Log($"[MaxInterstitialService] {message}");
#endif
        }
    }
}
