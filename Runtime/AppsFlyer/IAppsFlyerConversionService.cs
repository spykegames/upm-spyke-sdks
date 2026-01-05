using System;
using System.Collections.Generic;

namespace Spyke.SDKs.AppsFlyer
{
    /// <summary>
    /// AppsFlyer conversion data callback interface.
    /// </summary>
    public interface IAppsFlyerConversionService
    {
        /// <summary>
        /// Event fired when conversion data is received.
        /// </summary>
        event Action<AppsFlyerConversionData> OnConversionDataReceived;

        /// <summary>
        /// Event fired when conversion data request fails.
        /// </summary>
        event Action<string> OnConversionDataFailed;

        /// <summary>
        /// Event fired when app open attribution is received.
        /// </summary>
        event Action<Dictionary<string, object>> OnAppOpenAttribution;

        /// <summary>
        /// Get the last received conversion data.
        /// </summary>
        AppsFlyerConversionData LastConversionData { get; }

        /// <summary>
        /// Whether conversion data has been received.
        /// </summary>
        bool HasConversionData { get; }
    }
}
