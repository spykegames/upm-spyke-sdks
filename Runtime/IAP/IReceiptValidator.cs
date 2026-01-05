using Cysharp.Threading.Tasks;

namespace Spyke.SDKs.IAP
{
    /// <summary>
    /// Receipt validation result.
    /// </summary>
    public class ReceiptValidationResult
    {
        /// <summary>
        /// Whether the receipt is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Error message if validation failed.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Server response data (if any).
        /// </summary>
        public object ServerData { get; set; }

        public static ReceiptValidationResult Valid(object serverData = null)
        {
            return new ReceiptValidationResult
            {
                IsValid = true,
                ServerData = serverData
            };
        }

        public static ReceiptValidationResult Invalid(string errorMessage)
        {
            return new ReceiptValidationResult
            {
                IsValid = false,
                ErrorMessage = errorMessage
            };
        }
    }

    /// <summary>
    /// Receipt validator interface for server-side validation.
    /// </summary>
    public interface IReceiptValidator
    {
        /// <summary>
        /// Validate a purchase receipt.
        /// </summary>
        UniTask<ReceiptValidationResult> ValidateAsync(IAPResult purchaseResult);
    }
}
