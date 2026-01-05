using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

#if ONESIGNAL_SDK
using OneSignalSDK;
#endif

namespace Spyke.SDKs.OneSignal
{
    /// <summary>
    /// OneSignal push notification service implementation.
    /// Requires ONESIGNAL_SDK define and OneSignal SDK installed.
    /// </summary>
    public class OneSignalService : IOneSignalService, IPushPermissionHelper, IInitializable, IDisposable
    {
        private bool _isInitialized;
        private string _appId;
        private string _playerId;
        private PushPermissionStatus _permissionStatus = PushPermissionStatus.Unknown;
        private UniTaskCompletionSource<bool> _permissionTcs;

        public bool IsInitialized => _isInitialized;
        public PushPermissionStatus PermissionStatus => _permissionStatus;
        public PushPermissionStatus CurrentStatus => _permissionStatus;
        public bool IsPushEnabled { get; private set; }
        public string PlayerId => _playerId;
        public bool HasRequestedPermission { get; private set; }

        public event Action<NotificationData> OnNotificationReceived;
        public event Action<NotificationData> OnNotificationOpened;
        public event Action<PushPermissionStatus> OnPermissionStatusChanged;
        public event Action<PushPermissionStatus> OnStatusChanged;

        public void Initialize()
        {
            // Zenject Initialize - requires manual Initialize call with app ID
        }

        public void Initialize(string appId)
        {
            if (_isInitialized)
            {
                Log("OneSignal already initialized");
                return;
            }

            _appId = appId;

#if ONESIGNAL_SDK
            try
            {
                OneSignal.Initialize(appId);

                // Subscribe to events
                OneSignal.Notifications.Clicked += OnNotificationClicked;
                OneSignal.Notifications.ForegroundWillDisplay += OnNotificationWillDisplay;
                OneSignal.User.PushSubscription.Changed += OnPushSubscriptionChanged;

                _isInitialized = true;
                UpdatePermissionStatus();
                UpdatePlayerId();

                Log($"OneSignal initialized with app ID: {appId}");
            }
            catch (Exception ex)
            {
                Log($"OneSignal initialization failed: {ex.Message}", true);
            }
#else
            Log("OneSignal SDK not installed (ONESIGNAL_SDK not defined)");
#endif
        }

        public void SetExternalUserId(string userId)
        {
            if (!_isInitialized) return;

#if ONESIGNAL_SDK
            OneSignal.Login(userId);
            Log($"Set external user ID: {userId}");
#endif
        }

        public void RemoveExternalUserId()
        {
            if (!_isInitialized) return;

#if ONESIGNAL_SDK
            OneSignal.Logout();
            Log("Removed external user ID");
#endif
        }

        public void SetTag(string key, string value)
        {
            if (!_isInitialized) return;

#if ONESIGNAL_SDK
            OneSignal.User.AddTag(key, value);
            Log($"Set tag: {key}={value}");
#endif
        }

        public void RemoveTag(string key)
        {
            if (!_isInitialized) return;

#if ONESIGNAL_SDK
            OneSignal.User.RemoveTag(key);
            Log($"Removed tag: {key}");
#endif
        }

        public async UniTask<bool> RequestPermissionAsync()
        {
            if (!_isInitialized) return false;

            _permissionTcs = new UniTaskCompletionSource<bool>();
            HasRequestedPermission = true;

#if ONESIGNAL_SDK
            var result = await OneSignal.Notifications.RequestPermissionAsync(true);
            _permissionTcs.TrySetResult(result);
            UpdatePermissionStatus();
            return result;
#else
            _permissionTcs.TrySetResult(false);
            return false;
#endif
        }

        public void SetPushEnabled(bool enabled)
        {
            if (!_isInitialized) return;

#if ONESIGNAL_SDK
            OneSignal.User.PushSubscription.OptIn();
            IsPushEnabled = enabled;
            Log($"Push enabled: {enabled}");
#endif
        }

        public bool ShouldShowPrePermissionPrompt()
        {
            return !HasRequestedPermission && _permissionStatus == PushPermissionStatus.NotDetermined;
        }

        public void OpenAppSettings()
        {
#if UNITY_IOS && !UNITY_EDITOR
            var url = new System.Uri("app-settings:");
            Application.OpenURL(url.ToString());
#elif UNITY_ANDROID && !UNITY_EDITOR
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var intent = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS");
            using var uri = new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("parse", "package:" + Application.identifier);
            intent.Call<AndroidJavaObject>("setData", uri);
            currentActivity.Call("startActivity", intent);
#endif
        }

        public void Dispose()
        {
#if ONESIGNAL_SDK
            if (_isInitialized)
            {
                OneSignal.Notifications.Clicked -= OnNotificationClicked;
                OneSignal.Notifications.ForegroundWillDisplay -= OnNotificationWillDisplay;
                OneSignal.User.PushSubscription.Changed -= OnPushSubscriptionChanged;
            }
#endif
        }

#if ONESIGNAL_SDK
        private void OnNotificationClicked(NotificationClickedResult result)
        {
            var data = ConvertNotification(result.Notification);
            data.ActionId = result.Action?.ActionId;

            Log($"Notification opened: {data.Title}");
            OnNotificationOpened?.Invoke(data);
        }

        private void OnNotificationWillDisplay(NotificationWillDisplayEventArgs args)
        {
            var data = ConvertNotification(args.Notification);

            Log($"Notification received: {data.Title}");
            OnNotificationReceived?.Invoke(data);
        }

        private void OnPushSubscriptionChanged(PushSubscriptionChangedState state)
        {
            _playerId = state.Current.Id;
            IsPushEnabled = state.Current.OptedIn;

            Log($"Push subscription changed: ID={_playerId}, OptedIn={IsPushEnabled}");
            UpdatePermissionStatus();
        }

        private NotificationData ConvertNotification(INotification notification)
        {
            return new NotificationData
            {
                NotificationId = notification.NotificationId,
                Title = notification.Title,
                Body = notification.Body,
                AdditionalData = notification.AdditionalData != null
                    ? new Dictionary<string, object>(notification.AdditionalData)
                    : new Dictionary<string, object>(),
                RawPayload = notification
            };
        }

        private void UpdatePermissionStatus()
        {
            var hasPermission = OneSignal.Notifications.Permission;
            var newStatus = hasPermission ? PushPermissionStatus.Authorized : PushPermissionStatus.Denied;

            if (newStatus != _permissionStatus)
            {
                _permissionStatus = newStatus;
                OnPermissionStatusChanged?.Invoke(_permissionStatus);
                OnStatusChanged?.Invoke(_permissionStatus);
            }
        }

        private void UpdatePlayerId()
        {
            _playerId = OneSignal.User.PushSubscription.Id;
            IsPushEnabled = OneSignal.User.PushSubscription.OptedIn;
        }
#endif

        private static void Log(string message, bool isError = false)
        {
#if SPYKE_DEV
            if (isError)
                Debug.LogError($"[OneSignalService] {message}");
            else
                Debug.Log($"[OneSignalService] {message}");
#endif
        }
    }
}
