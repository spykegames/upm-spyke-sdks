using System;
using Cysharp.Threading.Tasks;

namespace Spyke.SDKs.Ads
{
    /// <summary>
    /// Rewarded ad service interface.
    /// </summary>
    public interface IRewardedAdService
    {
        /// <summary>
        /// Whether a rewarded ad is ready.
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
        /// Event fired when user earned reward.
        /// </summary>
        event Action<string> OnUserRewardEarned;

        /// <summary>
        /// Event fired when ad revenue is received.
        /// </summary>
        event Action<AdRevenueData> OnAdRevenue;

        /// <summary>
        /// Load a rewarded ad.
        /// </summary>
        void Load();

        /// <summary>
        /// Show a rewarded ad.
        /// </summary>
        void Show(string placement);

        /// <summary>
        /// Show a rewarded ad async.
        /// </summary>
        UniTask<AdResult> ShowAsync(string placement);
    }
}
