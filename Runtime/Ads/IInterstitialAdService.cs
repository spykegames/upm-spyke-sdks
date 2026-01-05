using System;
using Cysharp.Threading.Tasks;

namespace Spyke.SDKs.Ads
{
    /// <summary>
    /// Interstitial ad service interface.
    /// </summary>
    public interface IInterstitialAdService
    {
        /// <summary>
        /// Whether an interstitial ad is ready.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Event fired when ad is loaded.
        /// </summary>
        event Action<string> OnAdLoaded;

        /// <summary>
        /// Event fired when ad failed to load.
        /// </summary>
        event Action<string, string> OnAdLoadFailed;

        /// <summary>
        /// Event fired when ad is displayed.
        /// </summary>
        event Action<string> OnAdDisplayed;

        /// <summary>
        /// Event fired when ad failed to display.
        /// </summary>
        event Action<string, string> OnAdDisplayFailed;

        /// <summary>
        /// Event fired when ad is hidden/closed.
        /// </summary>
        event Action<string> OnAdHidden;

        /// <summary>
        /// Event fired when ad revenue is received.
        /// </summary>
        event Action<AdRevenueData> OnAdRevenue;

        /// <summary>
        /// Load an interstitial ad.
        /// </summary>
        void Load();

        /// <summary>
        /// Show an interstitial ad.
        /// </summary>
        void Show(string placement);

        /// <summary>
        /// Show an interstitial ad async.
        /// </summary>
        UniTask<AdResult> ShowAsync(string placement);
    }
}
