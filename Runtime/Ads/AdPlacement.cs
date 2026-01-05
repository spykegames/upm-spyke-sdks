namespace Spyke.SDKs.Ads
{
    /// <summary>
    /// Ad type.
    /// </summary>
    public enum AdType
    {
        Rewarded,
        Interstitial,
        Banner
    }

    /// <summary>
    /// Ad placement configuration.
    /// </summary>
    public class AdPlacement
    {
        /// <summary>
        /// Placement identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Ad type for this placement.
        /// </summary>
        public AdType Type { get; set; }

        /// <summary>
        /// Ad unit ID for this placement.
        /// </summary>
        public string AdUnitId { get; set; }

        /// <summary>
        /// Whether this placement is enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        public AdPlacement()
        {
        }

        public AdPlacement(string id, AdType type, string adUnitId)
        {
            Id = id;
            Type = type;
            AdUnitId = adUnitId;
            IsEnabled = true;
        }
    }
}
