namespace Spyke.SDKs.IAP
{
    /// <summary>
    /// IAP failure reasons.
    /// </summary>
    public enum IAPFailureReason
    {
        Unknown,
        UserCancelled,
        PaymentDeclined,
        ProductUnavailable,
        PurchasingUnavailable,
        SignatureInvalid,
        DuplicateTransaction,
        ExistingPurchasePending
    }

    /// <summary>
    /// IAP purchase result.
    /// </summary>
    public class IAPResult
    {
        /// <summary>
        /// Whether the purchase was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Product ID that was purchased.
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Transaction ID from the store.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Purchase receipt (for validation).
        /// </summary>
        public string Receipt { get; set; }

        /// <summary>
        /// Failure reason if not successful.
        /// </summary>
        public IAPFailureReason FailureReason { get; set; }

        /// <summary>
        /// Error message if failed.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The purchased product.
        /// </summary>
        public IAPProduct Product { get; set; }

        public static IAPResult Success(string productId, string transactionId, string receipt, IAPProduct product = null)
        {
            return new IAPResult
            {
                IsSuccess = true,
                ProductId = productId,
                TransactionId = transactionId,
                Receipt = receipt,
                Product = product
            };
        }

        public static IAPResult Failure(string productId, IAPFailureReason reason, string message = null)
        {
            return new IAPResult
            {
                IsSuccess = false,
                ProductId = productId,
                FailureReason = reason,
                ErrorMessage = message
            };
        }
    }
}
