using System;
using Cysharp.Threading.Tasks;
using Spyke.Services.Auth;
using UnityEngine;
using Zenject;

#if FACEBOOK_SDK
using Facebook.Unity;
#endif

namespace Spyke.SDKs.Facebook
{
    /// <summary>
    /// Facebook SDK wrapper implementing IFacebookAuthProvider.
    /// Requires FACEBOOK_SDK define and Facebook SDK to be installed.
    /// </summary>
    public class FacebookAuthProvider : IFacebookAuthProvider, IInitializable
    {
        private const string ProfilePictureUrlFormat = "https://graph.facebook.com/{0}/picture?width={1}&height={2}&access_token={3}|{4}";

        private string _appId;
        private string _clientToken;
        private UniTaskCompletionSource<FacebookAuthResult> _loginTcs;

#if FACEBOOK_SDK
        public bool IsInitialized => FB.IsInitialized;
        public bool IsLoggedIn => FB.IsLoggedIn;
        public bool IsLimitedLogin => FB.Mobile.CurrentAuthenticationToken() != null;

        public string AccessToken
        {
            get
            {
                if (IsLimitedLogin)
                {
                    return FB.Mobile.CurrentAuthenticationToken()?.TokenString;
                }
                return Facebook.Unity.AccessToken.CurrentAccessToken?.TokenString;
            }
        }

        public string UserId
        {
            get
            {
                if (IsLimitedLogin)
                {
                    return FB.Mobile.CurrentProfile()?.UserID;
                }
                return Facebook.Unity.AccessToken.CurrentAccessToken?.UserId;
            }
        }

        public bool HasFriendsPermission
        {
            get
            {
                var token = Facebook.Unity.AccessToken.CurrentAccessToken;
                return token != null && token.Permissions.Contains("user_friends");
            }
        }
#else
        public bool IsInitialized => false;
        public bool IsLoggedIn => false;
        public bool IsLimitedLogin => false;
        public string AccessToken => null;
        public string UserId => null;
        public bool HasFriendsPermission => false;
#endif

        public void Initialize()
        {
            // Auto-initialize if settings are available
#if FACEBOOK_SDK && !UNITY_STANDALONE
            if (!FB.IsInitialized)
            {
                FB.Init(OnInitComplete, OnHideUnity);
            }
#endif
        }

        public UniTask<bool> InitializeAsync(string appId = null, string clientToken = null)
        {
#if FACEBOOK_SDK && !UNITY_STANDALONE
            _appId = appId;
            _clientToken = clientToken;

            if (FB.IsInitialized)
            {
                return UniTask.FromResult(true);
            }

            var tcs = new UniTaskCompletionSource<bool>();
            FB.Init(() =>
            {
                tcs.TrySetResult(FB.IsInitialized);
            }, OnHideUnity);

            return tcs.Task;
#else
            return UniTask.FromResult(false);
#endif
        }

        public UniTask<FacebookAuthResult> LoginAsync()
        {
#if FACEBOOK_SDK && !UNITY_STANDALONE
            return LoginInternalAsync(new[] { "public_profile" });
#else
            return UniTask.FromResult(
                FacebookAuthResult.Failed("Facebook SDK not available", FacebookAuthError.SdkInitializationFailed));
#endif
        }

        public UniTask<FacebookAuthResult> LoginWithFriendsPermissionAsync()
        {
#if FACEBOOK_SDK && !UNITY_STANDALONE
            return LoginInternalAsync(new[] { "public_profile", "user_friends" });
#else
            return UniTask.FromResult(
                FacebookAuthResult.Failed("Facebook SDK not available", FacebookAuthError.SdkInitializationFailed));
#endif
        }

#if FACEBOOK_SDK && !UNITY_STANDALONE
        private async UniTask<FacebookAuthResult> LoginInternalAsync(string[] permissions)
        {
            if (!FB.IsInitialized)
            {
                var initialized = await InitializeAsync();
                if (!initialized)
                {
                    return FacebookAuthResult.Failed("SDK initialization failed", FacebookAuthError.SdkInitializationFailed);
                }
            }

            _loginTcs = new UniTaskCompletionSource<FacebookAuthResult>();

            FB.LogInWithReadPermissions(permissions, OnLoginComplete);

            return await _loginTcs.Task;
        }

        private void OnLoginComplete(ILoginResult result)
        {
            if (FB.IsLoggedIn)
            {
                var isLimited = IsLimitedLogin;
                var userId = UserId;
                var token = AccessToken;

#if SPYKE_DEV
                Debug.Log($"[FacebookAuthProvider] Login success - Limited: {isLimited}, UserId: {userId?[..5]}...");
#endif

                _loginTcs?.TrySetResult(FacebookAuthResult.Successful(token, userId, isLimited));
            }
            else if (result.Cancelled)
            {
#if SPYKE_DEV
                Debug.Log("[FacebookAuthProvider] Login cancelled by user");
#endif
                _loginTcs?.TrySetResult(FacebookAuthResult.UserCancelled());
            }
            else
            {
#if SPYKE_DEV
                Debug.LogWarning($"[FacebookAuthProvider] Login failed: {result.Error}");
#endif
                _loginTcs?.TrySetResult(FacebookAuthResult.Failed(result.Error, FacebookAuthError.LoginFailed));
            }
        }
#endif

        public UniTask<FacebookAuthResult> RefreshTokenAsync()
        {
#if FACEBOOK_SDK && !UNITY_STANDALONE
            if (!FB.IsInitialized || !FB.IsLoggedIn)
            {
                return UniTask.FromResult(
                    FacebookAuthResult.Failed("Not logged in", FacebookAuthError.TokenRefreshFailed));
            }

            var tcs = new UniTaskCompletionSource<FacebookAuthResult>();

            FB.Mobile.RefreshCurrentAccessToken(result =>
            {
                if (result.Error != null)
                {
                    tcs.TrySetResult(FacebookAuthResult.Failed(result.Error, FacebookAuthError.TokenRefreshFailed));
                }
                else
                {
                    tcs.TrySetResult(FacebookAuthResult.Successful(AccessToken, UserId, IsLimitedLogin));
                }
            });

            return tcs.Task;
#else
            return UniTask.FromResult(
                FacebookAuthResult.Failed("Facebook SDK not available", FacebookAuthError.SdkInitializationFailed));
#endif
        }

        public void Logout()
        {
#if FACEBOOK_SDK && !UNITY_STANDALONE
            if (FB.IsInitialized)
            {
                FB.LogOut();
            }
#endif
        }

        public string GetProfilePictureUrl(string userId, int width, int height)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(_appId) || string.IsNullOrEmpty(_clientToken))
            {
                return null;
            }

            return string.Format(ProfilePictureUrlFormat, userId, width, height, _appId, _clientToken);
        }

        public void SetAdvertiserTrackingEnabled(bool enabled)
        {
#if FACEBOOK_SDK && !UNITY_STANDALONE
            if (FB.IsInitialized)
            {
                FB.Mobile.SetAdvertiserTrackingEnabled(enabled);
            }
#endif
        }

#if FACEBOOK_SDK
        private void OnInitComplete()
        {
#if SPYKE_DEV
            Debug.Log($"[FacebookAuthProvider] SDK initialized: {FB.IsInitialized}");
#endif
        }

        private void OnHideUnity(bool isGameShown)
        {
            Time.timeScale = isGameShown ? 1f : 0f;
        }
#endif
    }
}
