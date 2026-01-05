using System;
using System.Collections.Generic;

namespace Spyke.SDKs.Firebase
{
    /// <summary>
    /// Firebase service interface for analytics and crashlytics.
    /// </summary>
    public interface IFirebaseService
    {
        /// <summary>
        /// Whether Firebase is initialized and ready.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Initialize Firebase asynchronously.
        /// </summary>
        void Initialize(Action<bool> onComplete = null);

        /// <summary>
        /// Set the user ID for analytics.
        /// </summary>
        void SetUserId(string userId);

        /// <summary>
        /// Set a user property for analytics.
        /// </summary>
        void SetUserProperty(string name, string value);

        /// <summary>
        /// Log an analytics event.
        /// </summary>
        void LogEvent(string eventName, Dictionary<string, object> parameters = null);

        /// <summary>
        /// Log an exception to Crashlytics.
        /// </summary>
        void LogException(Exception exception);

        /// <summary>
        /// Log a custom message to Crashlytics.
        /// </summary>
        void LogCrashlyticsMessage(string message);

        /// <summary>
        /// Set analytics data collection enabled state (GDPR consent).
        /// </summary>
        void SetAnalyticsCollectionEnabled(bool enabled);
    }
}
