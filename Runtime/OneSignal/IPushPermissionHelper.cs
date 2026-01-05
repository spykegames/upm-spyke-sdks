using System;
using Cysharp.Threading.Tasks;

namespace Spyke.SDKs.OneSignal
{
    /// <summary>
    /// Push notification permission helper interface.
    /// </summary>
    public interface IPushPermissionHelper
    {
        /// <summary>
        /// Current permission status.
        /// </summary>
        PushPermissionStatus CurrentStatus { get; }

        /// <summary>
        /// Whether permission has been requested before.
        /// </summary>
        bool HasRequestedPermission { get; }

        /// <summary>
        /// Event fired when permission status changes.
        /// </summary>
        event Action<PushPermissionStatus> OnStatusChanged;

        /// <summary>
        /// Request push notification permission.
        /// </summary>
        UniTask<bool> RequestPermissionAsync();

        /// <summary>
        /// Check if we should show a pre-permission prompt.
        /// </summary>
        bool ShouldShowPrePermissionPrompt();

        /// <summary>
        /// Open app settings for manual permission change.
        /// </summary>
        void OpenAppSettings();
    }
}
