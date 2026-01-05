using System.Collections.Generic;
using Spyke.Services.Analytics;
using Zenject;

namespace Spyke.SDKs.AppsFlyer
{
    /// <summary>
    /// AppsFlyer Analytics provider implementation.
    /// Registers with AnalyticsService to receive all analytics events.
    /// </summary>
    public class AppsFlyerAnalyticsProvider : IAnalyticsProvider, IInitializable
    {
        private readonly IAppsFlyerService _appsFlyerService;

        public string Name => "AppsFlyer";

        public bool IsReady => _appsFlyerService?.IsReady ?? false;

        [Inject]
        public AppsFlyerAnalyticsProvider(IAppsFlyerService appsFlyerService)
        {
            _appsFlyerService = appsFlyerService;
        }

        public void Initialize()
        {
            // AppsFlyer is initialized by AppsFlyerService
            // This provider just delegates to it
        }

        public void SetUserId(string userId)
        {
            if (!IsReady) return;
            _appsFlyerService.SetCustomerUserId(userId);
        }

        public void SetUserProperty(string name, string value)
        {
            // AppsFlyer doesn't have direct user properties like Firebase
            // Could log as event if needed
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters)
        {
            if (!IsReady) return;

            // Convert object parameters to string parameters for AppsFlyer
            Dictionary<string, string> stringParams = null;

            if (parameters != null && parameters.Count > 0)
            {
                stringParams = new Dictionary<string, string>();
                foreach (var kvp in parameters)
                {
                    stringParams[kvp.Key] = kvp.Value?.ToString() ?? "";
                }
            }

            _appsFlyerService.LogEvent(eventName, stringParams);
        }
    }
}
