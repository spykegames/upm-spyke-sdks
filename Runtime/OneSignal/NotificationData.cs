using System.Collections.Generic;

namespace Spyke.SDKs.OneSignal
{
    /// <summary>
    /// Notification data model.
    /// </summary>
    public class NotificationData
    {
        /// <summary>
        /// Notification ID.
        /// </summary>
        public string NotificationId { get; set; }

        /// <summary>
        /// Notification title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Notification body/message.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Additional data sent with the notification.
        /// </summary>
        public Dictionary<string, object> AdditionalData { get; set; }

        /// <summary>
        /// Action ID if a button was clicked.
        /// </summary>
        public string ActionId { get; set; }

        /// <summary>
        /// Raw notification payload.
        /// </summary>
        public object RawPayload { get; set; }

        public NotificationData()
        {
            AdditionalData = new Dictionary<string, object>();
        }

        /// <summary>
        /// Get additional data value.
        /// </summary>
        public T GetData<T>(string key, T defaultValue = default)
        {
            if (AdditionalData != null && AdditionalData.TryGetValue(key, out var value))
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }

                try
                {
                    return (T)System.Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }
    }
}
