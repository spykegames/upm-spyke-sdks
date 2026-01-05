using System;
using Cysharp.Threading.Tasks;

namespace Spyke.SDKs.Ads
{
    /// <summary>
    /// Consent status.
    /// </summary>
    public enum ConsentStatus
    {
        Unknown,
        NotApplicable,
        Required,
        Obtained
    }

    /// <summary>
    /// Ad consent service interface for GDPR and ATT.
    /// </summary>
    public interface IAdConsentService
    {
        /// <summary>
        /// Current consent status.
        /// </summary>
        ConsentStatus ConsentStatus { get; }

        /// <summary>
        /// Whether consent is required.
        /// </summary>
        bool IsConsentRequired { get; }

        /// <summary>
        /// Whether user has given consent.
        /// </summary>
        bool HasConsent { get; }

        /// <summary>
        /// Event fired when consent status changes.
        /// </summary>
        event Action<ConsentStatus> OnConsentStatusChanged;

        /// <summary>
        /// Show consent dialog if required.
        /// </summary>
        UniTask<bool> ShowConsentDialogAsync();

        /// <summary>
        /// Set consent directly (for custom consent flows).
        /// </summary>
        void SetConsent(bool hasConsent);

        /// <summary>
        /// Request App Tracking Transparency permission (iOS 14+).
        /// </summary>
        UniTask<bool> RequestTrackingAuthorizationAsync();
    }
}
