using System;
using System.Collections.Generic;

namespace Spyke.SDKs.AppsFlyer
{
    /// <summary>
    /// AppsFlyer service interface for attribution and analytics.
    /// </summary>
    public interface IAppsFlyerService
    {
        /// <summary>
        /// Whether AppsFlyer is initialized and ready.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Initialize AppsFlyer with dev key and app ID.
        /// </summary>
        void Initialize(string devKey, string appId, Action<bool> onComplete = null);

        /// <summary>
        /// Set the customer user ID.
        /// </summary>
        void SetCustomerUserId(string userId);

        /// <summary>
        /// Log an event with parameters.
        /// </summary>
        void LogEvent(string eventName, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Log a purchase event.
        /// </summary>
        void LogPurchase(string productId, string currency, double price, string transactionId = null);

        /// <summary>
        /// Log ad revenue event.
        /// </summary>
        void LogAdRevenue(string source, string currency, double revenue);

        /// <summary>
        /// Start SDK tracking.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop SDK tracking.
        /// </summary>
        void Stop();
    }
}
