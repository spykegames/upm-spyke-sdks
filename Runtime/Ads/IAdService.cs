using System;
using System.Threading;
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
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        UniTask<bool> InitializeAsync(string sdkKey = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Show a rewarded ad.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        UniTask<AdResult> ShowRewardedAsync(string placement, CancellationToken cancellationToken = default);

        /// <summary>
        /// Show an interstitial ad.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        UniTask<AdResult> ShowInterstitialAsync(string placement, CancellationToken cancellationToken = default);

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
