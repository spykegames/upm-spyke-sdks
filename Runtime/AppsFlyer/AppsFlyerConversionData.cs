using System.Collections.Generic;

namespace Spyke.SDKs.AppsFlyer
{
    /// <summary>
    /// AppsFlyer conversion data model.
    /// </summary>
    public class AppsFlyerConversionData
    {
        /// <summary>
        /// Whether this is an organic install.
        /// </summary>
        public bool IsOrganic { get; set; }

        /// <summary>
        /// Media source attribution.
        /// </summary>
        public string MediaSource { get; set; }

        /// <summary>
        /// Campaign name.
        /// </summary>
        public string Campaign { get; set; }

        /// <summary>
        /// Campaign ID.
        /// </summary>
        public string CampaignId { get; set; }

        /// <summary>
        /// Ad set name.
        /// </summary>
        public string AdSet { get; set; }

        /// <summary>
        /// Ad set ID.
        /// </summary>
        public string AdSetId { get; set; }

        /// <summary>
        /// Ad name.
        /// </summary>
        public string Ad { get; set; }

        /// <summary>
        /// Ad ID.
        /// </summary>
        public string AdId { get; set; }

        /// <summary>
        /// Install time.
        /// </summary>
        public string InstallTime { get; set; }

        /// <summary>
        /// Click time.
        /// </summary>
        public string ClickTime { get; set; }

        /// <summary>
        /// Raw conversion data dictionary.
        /// </summary>
        public Dictionary<string, object> RawData { get; set; }

        public AppsFlyerConversionData()
        {
            RawData = new Dictionary<string, object>();
        }

        public AppsFlyerConversionData(Dictionary<string, object> data) : this()
        {
            if (data == null) return;

            RawData = data;

            if (data.TryGetValue("af_status", out var status))
            {
                IsOrganic = status?.ToString() == "Organic";
            }

            if (data.TryGetValue("media_source", out var source))
            {
                MediaSource = source?.ToString();
            }

            if (data.TryGetValue("campaign", out var campaign))
            {
                Campaign = campaign?.ToString();
            }

            if (data.TryGetValue("campaign_id", out var campaignId))
            {
                CampaignId = campaignId?.ToString();
            }

            if (data.TryGetValue("adset", out var adSet))
            {
                AdSet = adSet?.ToString();
            }

            if (data.TryGetValue("adset_id", out var adSetId))
            {
                AdSetId = adSetId?.ToString();
            }

            if (data.TryGetValue("ad", out var ad))
            {
                Ad = ad?.ToString();
            }

            if (data.TryGetValue("ad_id", out var adId))
            {
                AdId = adId?.ToString();
            }

            if (data.TryGetValue("install_time", out var installTime))
            {
                InstallTime = installTime?.ToString();
            }

            if (data.TryGetValue("click_time", out var clickTime))
            {
                ClickTime = clickTime?.ToString();
            }
        }
    }
}
