using System;
using Cysharp.Threading.Tasks;

namespace Spyke.SDKs.OneSignal
{
    /// <summary>
    /// Push notification permission status.
    /// </summary>
    public enum PushPermissionStatus
    {
        Unknown,
        NotDetermined,
        Denied,
        Authorized,
        Provisional
    }

    /// <summary>
    /// OneSignal push notification service interface.
    /// </summary>
    public interface IOneSignalService
    {
        /// <summary>
        /// Whether OneSignal is initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Current permission status.
        /// </summary>
        PushPermissionStatus PermissionStatus { get; }

        /// <summary>
        /// Whether push notifications are enabled.
        /// </summary>
        bool IsPushEnabled { get; }

        /// <summary>
        /// OneSignal player ID.
        /// </summary>
        string PlayerId { get; }

        /// <summary>
        /// Event fired when a notification is received.
        /// </summary>
        event Action<NotificationData> OnNotificationReceived;

        /// <summary>
        /// Event fired when a notification is opened/clicked.
        /// </summary>
        event Action<NotificationData> OnNotificationOpened;

        /// <summary>
        /// Event fired when permission status changes.
        /// </summary>
        event Action<PushPermissionStatus> OnPermissionStatusChanged;

        /// <summary>
        /// Initialize OneSignal with app ID.
        /// </summary>
        void Initialize(string appId);

        /// <summary>
        /// Set the external user ID for player identification.
        /// </summary>
        void SetExternalUserId(string userId);

        /// <summary>
        /// Remove the external user ID.
        /// </summary>
        void RemoveExternalUserId();

        /// <summary>
        /// Set a tag value.
        /// </summary>
        void SetTag(string key, string value);

        /// <summary>
        /// Remove a tag.
        /// </summary>
        void RemoveTag(string key);

        /// <summary>
        /// Request push notification permission.
        /// </summary>
        UniTask<bool> RequestPermissionAsync();

        /// <summary>
        /// Enable or disable push notifications.
        /// </summary>
        void SetPushEnabled(bool enabled);
    }
}
