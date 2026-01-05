using System;
using Cysharp.Threading.Tasks;

namespace Spyke.SDKs.Ads
{
    /// <summary>
    /// Combined ad service interface for all ad types.
    /// </summary>
    public interface IAdService
    {
        /// <summary>
        /// Whether the ad SDK is initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Event fired when ad revenue is received.
        /// </summary>
        event Action<AdRevenueData> OnAdRevenue;

        /// <summary>
        /// Initialize the ad SDK.
        /// </summary>
        void Initialize(string sdkKey = null);

        /// <summary>
        /// Initialize async.
        /// </summary>
        UniTask<bool> InitializeAsync(string sdkKey = null);

        /// <summary>
        /// Show a rewarded ad.
        /// </summary>
        UniTask<AdResult> ShowRewardedAsync(string placement);

        /// <summary>
        /// Show an interstitial ad.
        /// </summary>
        UniTask<AdResult> ShowInterstitialAsync(string placement);

        /// <summary>
        /// Check if a rewarded ad is ready.
        /// </summary>
        bool IsRewardedReady(string placement);

        /// <summary>
        /// Check if an interstitial ad is ready.
        /// </summary>
        bool IsInterstitialReady(string placement);

        /// <summary>
        /// Load a rewarded ad for placement.
        /// </summary>
        void LoadRewarded(string placement);

        /// <summary>
        /// Load an interstitial ad for placement.
        /// </summary>
        void LoadInterstitial(string placement);
    }
}
