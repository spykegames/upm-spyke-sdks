using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

#if APPSFLYER_SDK
using AppsFlyerSDK;
#endif

namespace Spyke.SDKs.AppsFlyer
{
    /// <summary>
    /// AppsFlyer service implementation with conditional compilation.
    /// Requires APPSFLYER_SDK define and AppsFlyer SDK installed.
    /// </summary>
    public class AppsFlyerService : IAppsFlyerService, IAppsFlyerConversionService, IInitializable, IDisposable
#if APPSFLYER_SDK
        , IAppsFlyerConversionData
#endif
    {
        private bool _isReady;
        private bool _isInitializing;
        private Action<bool> _onInitComplete;
        private AppsFlyerConversionData _lastConversionData;

        public bool IsReady => _isReady;
        public AppsFlyerConversionData LastConversionData => _lastConversionData;
        public bool HasConversionData => _lastConversionData != null;

        public event Action<AppsFlyerConversionData> OnConversionDataReceived;
        public event Action<string> OnConversionDataFailed;
        public event Action<Dictionary<string, object>> OnAppOpenAttribution;

        public void Initialize()
        {
            // Zenject Initialize - requires manual Initialize call with keys
        }

        public void Initialize(string devKey, string appId, Action<bool> onComplete = null)
        {
            if (_isReady)
            {
                onComplete?.Invoke(true);
                return;
            }

            if (_isInitializing)
            {
                _onInitComplete += onComplete;
                return;
            }

            _isInitializing = true;
            _onInitComplete = onComplete;

#if APPSFLYER_SDK
            try
            {
                AppsFlyer.initSDK(devKey, appId, this);
                _isReady = true;
                Log($"AppsFlyer initialized with app ID: {appId}");
            }
            catch (Exception ex)
            {
                Log($"AppsFlyer initialization failed: {ex.Message}", true);
                _isReady = false;
            }
#else
            Log("AppsFlyer SDK not installed (APPSFLYER_SDK not defined)");
            _isReady = false;
#endif

            _isInitializing = false;
            _onInitComplete?.Invoke(_isReady);
            _onInitComplete = null;
        }

        public void Start()
        {
#if APPSFLYER_SDK
            if (_isReady)
            {
                AppsFlyer.startSDK();
                Log("AppsFlyer SDK started");
            }
#endif
        }

        public void Stop()
        {
#if APPSFLYER_SDK
            if (_isReady)
            {
                AppsFlyer.stopSDK(true);
                Log("AppsFlyer SDK stopped");
            }
#endif
        }

        public void SetCustomerUserId(string userId)
        {
            if (!_isReady) return;

#if APPSFLYER_SDK
            AppsFlyer.setCustomerUserId(userId);
#endif
        }

        public void LogEvent(string eventName, Dictionary<string, string> parameters = null)
        {
            if (!_isReady) return;

#if APPSFLYER_SDK
            AppsFlyer.sendEvent(eventName, parameters);
#endif
        }

        public void LogPurchase(string productId, string currency, double price, string transactionId = null)
        {
            if (!_isReady) return;

#if APPSFLYER_SDK
            var parameters = new Dictionary<string, string>
            {
                { AFInAppEvents.CONTENT_ID, productId },
                { AFInAppEvents.CURRENCY, currency },
                { AFInAppEvents.REVENUE, price.ToString("F2") }
            };

            if (!string.IsNullOrEmpty(transactionId))
            {
                parameters[AFInAppEvents.ORDER_ID] = transactionId;
            }

            AppsFlyer.sendEvent(AFInAppEvents.PURCHASE, parameters);
#endif
        }

        public void LogAdRevenue(string source, string currency, double revenue)
        {
            if (!_isReady) return;

#if APPSFLYER_SDK
            var adRevenueData = new Dictionary<string, string>
            {
                { "af_ad_revenue_ad_type", source },
                { AFInAppEvents.CURRENCY, currency },
                { AFInAppEvents.REVENUE, revenue.ToString("F6") }
            };

            AppsFlyer.sendEvent("af_ad_revenue", adRevenueData);
#endif
        }

        public void Dispose()
        {
#if APPSFLYER_SDK
            // Cleanup if needed
#endif
        }

#if APPSFLYER_SDK
        // IAppsFlyerConversionData implementation
        public void onConversionDataSuccess(string conversionData)
        {
            Log($"Conversion data received: {conversionData}");

            try
            {
                var dataDict = AppsFlyer.CallbackStringToDictionary(conversionData);
                _lastConversionData = new AppsFlyerConversionData(dataDict);
                OnConversionDataReceived?.Invoke(_lastConversionData);
            }
            catch (Exception ex)
            {
                Log($"Error parsing conversion data: {ex.Message}", true);
            }
        }

        public void onConversionDataFail(string error)
        {
            Log($"Conversion data failed: {error}", true);
            OnConversionDataFailed?.Invoke(error);
        }

        public void onAppOpenAttribution(string attributionData)
        {
            Log($"App open attribution: {attributionData}");

            try
            {
                var dataDict = AppsFlyer.CallbackStringToDictionary(attributionData);
                OnAppOpenAttribution?.Invoke(dataDict);
            }
            catch (Exception ex)
            {
                Log($"Error parsing attribution data: {ex.Message}", true);
            }
        }

        public void onAppOpenAttributionFailure(string error)
        {
            Log($"App open attribution failed: {error}", true);
        }
#endif

        private static void Log(string message, bool isError = false)
        {
#if SPYKE_DEV
            if (isError)
                Debug.LogError($"[AppsFlyerService] {message}");
            else
                Debug.Log($"[AppsFlyerService] {message}");
#endif
        }
    }
}
