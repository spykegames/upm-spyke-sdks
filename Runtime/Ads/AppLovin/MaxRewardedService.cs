using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Spyke.SDKs.Ads.AppLovin
{
    /// <summary>
    /// AppLovin MAX rewarded ad service implementation.
    /// Requires APPLOVIN_MAX define and MAX SDK installed.
    /// </summary>
    public class MaxRewardedService : IRewardedAdService, IInitializable, IDisposable
    {
        private readonly string _adUnitId;
        private bool _isReady;
        private bool _rewardEarned;
        private UniTaskCompletionSource<AdResult> _showTcs;
        private string _currentPlacement;

        public bool IsReady => _isReady;

        public event Action<string> OnAdLoaded;
        public event Action<string, string> OnAdLoadFailed;
        public event Action<string> OnAdDisplayed;
        public event Action<string, string> OnAdDisplayFailed;
        public event Action<string> OnAdHidden;
        public event Action<string> OnUserRewardEarned;
        public event Action<AdRevenueData> OnAdRevenue;

        public MaxRewardedService(string adUnitId)
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
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdDisplayFailed;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHidden;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedReward;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaid;

            Load();
            Log("MaxRewardedService initialized");
#else
            Log("AppLovin MAX SDK not installed (APPLOVIN_MAX not defined)");
#endif
        }

        public void Dispose()
        {
#if APPLOVIN_MAX
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnRewardedAdLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnRewardedAdLoadFailed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent -= OnRewardedAdDisplayed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnRewardedAdDisplayFailed;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardedAdHidden;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardedAdReceivedReward;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnRewardedAdRevenuePaid;
#endif
        }

        public void Load()
        {
#if APPLOVIN_MAX
            MaxSdk.LoadRewardedAd(_adUnitId);
            Log($"Loading rewarded ad: {_adUnitId}");
#endif
        }

        public void Show(string placement)
        {
            ShowAsync(placement).Forget();
        }

        public async UniTask<AdResult> ShowAsync(string placement, CancellationToken cancellationToken = default)
        {
            if (!_isReady)
            {
                Log($"Rewarded ad not ready for placement: {placement}");
                return AdResult.NotReady(placement);
            }

            _showTcs = new UniTaskCompletionSource<AdResult>();
            _currentPlacement = placement;
            _rewardEarned = false;

#if APPLOVIN_MAX
            MaxSdk.ShowRewardedAd(_adUnitId, placement);
#else
            _showTcs.TrySetResult(AdResult.Failed(placement, "MAX SDK not installed"));
#endif

            return await _showTcs.Task;
        }

#if APPLOVIN_MAX
        private void OnRewardedAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != _adUnitId) return;

            _isReady = true;
            Log($"Rewarded ad loaded: {adUnitId}");
            OnAdLoaded?.Invoke(adUnitId);
        }

        private void OnRewardedAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            if (adUnitId != _adUnitId) return;

            _isReady = false;
            Log($"Rewarded ad failed to load: {errorInfo.Message}", true);
            OnAdLoadFailed?.Invoke(adUnitId, errorInfo.Message);

            // Retry after delay
            RetryLoad().Forget();
        }

        private async UniTaskVoid RetryLoad()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            Load();
        }

        private void OnRewardedAdDisplayed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != _adUnitId) return;

            Log($"Rewarded ad displayed: {adUnitId}");
            OnAdDisplayed?.Invoke(adUnitId);
        }

        private void OnRewardedAdDisplayFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != _adUnitId) return;

            Log($"Rewarded ad failed to display: {errorInfo.Message}", true);
            OnAdDisplayFailed?.Invoke(adUnitId, errorInfo.Message);

            _showTcs?.TrySetResult(AdResult.Failed(_currentPlacement, errorInfo.Message));
            Load();
        }

        private void OnRewardedAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != _adUnitId) return;

            Log($"Rewarded ad hidden: {adUnitId}, rewardEarned: {_rewardEarned}");
            OnAdHidden?.Invoke(adUnitId);

            var result = _rewardEarned
                ? AdResult.Success(_currentPlacement, adUnitId)
                : AdResult.Cancelled(_currentPlacement);

            _showTcs?.TrySetResult(result);
            _isReady = false;
            Load();
        }

        private void OnRewardedAdReceivedReward(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != _adUnitId) return;

            _rewardEarned = true;
            Log($"Rewarded ad reward received: {reward.Label} x {reward.Amount}");
            OnUserRewardEarned?.Invoke(adUnitId);
        }

        private void OnRewardedAdRevenuePaid(string adUnitId, MaxSdkBase.AdInfo adInfo)
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

            Log($"Rewarded ad revenue: ${adInfo.Revenue} from {adInfo.NetworkName}");
            OnAdRevenue?.Invoke(revenueData);
        }
#endif

        private static void Log(string message, bool isError = false)
        {
#if SPYKE_DEV
            if (isError)
                Debug.LogError($"[MaxRewardedService] {message}");
            else
                Debug.Log($"[MaxRewardedService] {message}");
#endif
        }
    }
}
