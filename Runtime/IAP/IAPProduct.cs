namespace Spyke.SDKs.IAP
{
    /// <summary>
    /// Product type for IAP.
    /// </summary>
    public enum IAPProductType
    {
        Consumable,
        NonConsumable,
        Subscription
    }

    /// <summary>
    /// IAP product model.
    /// </summary>
    public class IAPProduct
    {
        /// <summary>
        /// Product ID (store ID).
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Product type.
        /// </summary>
        public IAPProductType Type { get; set; }

        /// <summary>
        /// Localized title from store.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Localized description from store.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Localized price string (e.g., "$0.99").
        /// </summary>
        public string PriceString { get; set; }

        /// <summary>
        /// Price in decimal.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Currency code (e.g., "USD").
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Whether the product is available for purchase.
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Raw store metadata.
        /// </summary>
        public object Metadata { get; set; }

        public IAPProduct()
        {
        }

        public IAPProduct(string id, IAPProductType type)
        {
            Id = id;
            Type = type;
        }
    }
}
