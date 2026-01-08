using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Spyke.Services.RemoteConfig;

#if FIREBASE_REMOTE_CONFIG
using Firebase.Extensions;
using Firebase.RemoteConfig;
#endif

namespace Spyke.SDKs.Firebase
{
    /// <summary>
    /// Firebase Remote Config implementation of IRemoteConfigService.
    /// Requires Firebase Unity SDK and FIREBASE_REMOTE_CONFIG scripting define.
    /// </summary>
    public class FirebaseRemoteConfigService : IRemoteConfigService
    {
        private bool _isInitialized;
        private bool _hasFetched;
        private DateTime _lastFetchTime = DateTime.MinValue;

        public bool IsInitialized => _isInitialized;
        public bool HasFetched => _hasFetched;
        public DateTime LastFetchTime => _lastFetchTime;

        public event Action OnConfigUpdated;

#if FIREBASE_REMOTE_CONFIG
        private FirebaseRemoteConfig _remoteConfig;

        public void Initialize()
        {
            _remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            _remoteConfig.OnConfigUpdateListener += OnConfigUpdateReceived;
            _isInitialized = true;
        }

        private void OnConfigUpdateReceived(object sender, ConfigUpdateEventArgs args)
        {
            if (args.Error != RemoteConfigError.None)
            {
                return;
            }

            _remoteConfig.ActivateAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    OnConfigUpdated?.Invoke();
                }
            });
        }

        public async UniTask<bool> FetchAsync(TimeSpan? cacheExpiration = null)
        {
            if (!_isInitialized) return false;

            try
            {
                var settings = new ConfigSettings();
                if (cacheExpiration.HasValue)
                {
                    settings.MinimumFetchInternalInMilliseconds = (ulong)cacheExpiration.Value.TotalMilliseconds;
                }
                await _remoteConfig.SetConfigSettingsAsync(settings);

                var task = await _remoteConfig.FetchAsync(cacheExpiration ?? TimeSpan.FromHours(12));
                _lastFetchTime = DateTime.UtcNow;
                _hasFetched = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async UniTask<bool> FetchAndActivateAsync(TimeSpan? cacheExpiration = null)
        {
            if (!await FetchAsync(cacheExpiration))
            {
                return false;
            }
            return await ActivateAsync();
        }

        public async UniTask<bool> ActivateAsync()
        {
            if (!_isInitialized) return false;

            try
            {
                var activated = await _remoteConfig.ActivateAsync();
                if (activated)
                {
                    OnConfigUpdated?.Invoke();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetString(string key, string defaultValue = "")
        {
            if (!_isInitialized) return defaultValue;
            var value = _remoteConfig.GetValue(key);
            return value.Source == ValueSource.DefaultValue ? defaultValue : value.StringValue;
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            if (!_isInitialized) return defaultValue;
            var value = _remoteConfig.GetValue(key);
            return value.Source == ValueSource.DefaultValue ? defaultValue : (int)value.LongValue;
        }

        public long GetLong(string key, long defaultValue = 0)
        {
            if (!_isInitialized) return defaultValue;
            var value = _remoteConfig.GetValue(key);
            return value.Source == ValueSource.DefaultValue ? defaultValue : value.LongValue;
        }

        public float GetFloat(string key, float defaultValue = 0f)
        {
            if (!_isInitialized) return defaultValue;
            var value = _remoteConfig.GetValue(key);
            return value.Source == ValueSource.DefaultValue ? defaultValue : (float)value.DoubleValue;
        }

        public double GetDouble(string key, double defaultValue = 0.0)
        {
            if (!_isInitialized) return defaultValue;
            var value = _remoteConfig.GetValue(key);
            return value.Source == ValueSource.DefaultValue ? defaultValue : value.DoubleValue;
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            if (!_isInitialized) return defaultValue;
            var value = _remoteConfig.GetValue(key);
            return value.Source == ValueSource.DefaultValue ? defaultValue : value.BooleanValue;
        }

        public IReadOnlyList<string> GetKeys()
        {
            if (!_isInitialized) return Array.Empty<string>();
            return new List<string>(_remoteConfig.Keys);
        }

        public bool HasKey(string key)
        {
            if (!_isInitialized) return false;
            return _remoteConfig.Keys.Contains(key);
        }

        public void SetDefaults(IDictionary<string, object> defaults)
        {
            if (!_isInitialized) return;
            _remoteConfig.SetDefaultsAsync(defaults).ContinueWithOnMainThread(_ => { });
        }

        public void Dispose()
        {
            if (_remoteConfig != null)
            {
                _remoteConfig.OnConfigUpdateListener -= OnConfigUpdateReceived;
            }
        }
#else
        public void Initialize()
        {
            _isInitialized = true;
        }

        public UniTask<bool> FetchAsync(TimeSpan? cacheExpiration = null)
        {
            return UniTask.FromResult(false);
        }

        public UniTask<bool> FetchAndActivateAsync(TimeSpan? cacheExpiration = null)
        {
            return UniTask.FromResult(false);
        }

        public UniTask<bool> ActivateAsync()
        {
            return UniTask.FromResult(false);
        }

        public string GetString(string key, string defaultValue = "") => defaultValue;
        public int GetInt(string key, int defaultValue = 0) => defaultValue;
        public long GetLong(string key, long defaultValue = 0) => defaultValue;
        public float GetFloat(string key, float defaultValue = 0f) => defaultValue;
        public double GetDouble(string key, double defaultValue = 0.0) => defaultValue;
        public bool GetBool(string key, bool defaultValue = false) => defaultValue;
        public IReadOnlyList<string> GetKeys() => Array.Empty<string>();
        public bool HasKey(string key) => false;
        public void SetDefaults(IDictionary<string, object> defaults) { }
        public void Dispose() { }
#endif
    }
}
