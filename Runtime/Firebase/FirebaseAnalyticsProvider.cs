using System.Collections.Generic;
using Spyke.Services.Analytics;
using Zenject;

namespace Spyke.SDKs.Firebase
{
    /// <summary>
    /// Firebase Analytics provider implementation.
    /// Registers with AnalyticsService to receive all analytics events.
    /// </summary>
    public class FirebaseAnalyticsProvider : IAnalyticsProvider, IInitializable
    {
        private readonly IFirebaseService _firebaseService;

        public string Name => "Firebase";

        public bool IsReady => _firebaseService?.IsReady ?? false;

        [Inject]
        public FirebaseAnalyticsProvider(IFirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        public void Initialize()
        {
            // Firebase is initialized by FirebaseService
            // This provider just delegates to it
        }

        public void SetUserId(string userId)
        {
            if (!IsReady) return;
            _firebaseService.SetUserId(userId);
        }

        public void SetUserProperty(string name, string value)
        {
            if (!IsReady) return;
            _firebaseService.SetUserProperty(name, value);
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters)
        {
            if (!IsReady) return;
            _firebaseService.LogEvent(eventName, parameters);
        }
    }
}
