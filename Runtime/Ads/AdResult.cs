namespace Spyke.SDKs.Ads
{
    /// <summary>
    /// Ad result status.
    /// </summary>
    public enum AdResultStatus
    {
        Success,
        Failed,
        UserCancelled,
        NotReady,
        NotLoaded
    }

    /// <summary>
    /// Ad revenue data.
    /// </summary>
    public class AdRevenueData
    {
        public string AdUnitId { get; set; }
        public string NetworkName { get; set; }
        public string Placement { get; set; }
        public double Revenue { get; set; }
        public string Currency { get; set; }
        public string CountryCode { get; set; }
    }

    /// <summary>
    /// Ad result model.
    /// </summary>
    public class AdResult
    {
        /// <summary>
        /// Result status.
        /// </summary>
        public AdResultStatus Status { get; set; }

        /// <summary>
        /// Whether the ad was completed successfully.
        /// </summary>
        public bool IsSuccess => Status == AdResultStatus.Success;

        /// <summary>
        /// Placement that was shown.
        /// </summary>
        public string Placement { get; set; }

        /// <summary>
        /// Ad unit ID.
        /// </summary>
        public string AdUnitId { get; set; }

        /// <summary>
        /// Error message if failed.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Revenue data if available.
        /// </summary>
        public AdRevenueData Revenue { get; set; }

        public static AdResult Success(string placement, string adUnitId = null, AdRevenueData revenue = null)
        {
            return new AdResult
            {
                Status = AdResultStatus.Success,
                Placement = placement,
                AdUnitId = adUnitId,
                Revenue = revenue
            };
        }

        public static AdResult Failed(string placement, string errorMessage = null)
        {
            return new AdResult
            {
                Status = AdResultStatus.Failed,
                Placement = placement,
                ErrorMessage = errorMessage
            };
        }

        public static AdResult Cancelled(string placement)
        {
            return new AdResult
            {
                Status = AdResultStatus.UserCancelled,
                Placement = placement
            };
        }

        public static AdResult NotReady(string placement)
        {
            return new AdResult
            {
                Status = AdResultStatus.NotReady,
                Placement = placement
            };
        }
    }
}
