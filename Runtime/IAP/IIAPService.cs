using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Spyke.SDKs.IAP
{
    /// <summary>
    /// In-app purchase service interface.
    /// </summary>
    public interface IIAPService
    {
        /// <summary>
        /// Whether the IAP service is initialized and ready.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Event fired when initialization completes.
        /// </summary>
        event Action<bool> OnInitialized;

        /// <summary>
        /// Event fired when a purchase completes.
        /// </summary>
        event Action<IAPResult> OnPurchaseCompleted;

        /// <summary>
        /// Event fired when a purchase fails.
        /// </summary>
        event Action<string, IAPFailureReason> OnPurchaseFailed;

        /// <summary>
        /// Initialize with product catalog.
        /// </summary>
        void Initialize(IEnumerable<IAPProduct> products);

        /// <summary>
        /// Initialize async with product catalog.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        UniTask<bool> InitializeAsync(IEnumerable<IAPProduct> products, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a product by ID.
        /// </summary>
        IAPProduct GetProduct(string productId);

        /// <summary>
        /// Get all available products.
        /// </summary>
        IReadOnlyList<IAPProduct> GetAllProducts();

        /// <summary>
        /// Purchase a product by ID.
        /// </summary>
        void Purchase(string productId);

        /// <summary>
        /// Purchase a product async by ID.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        UniTask<IAPResult> PurchaseAsync(string productId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Restore previous purchases (iOS).
        /// </summary>
        void RestorePurchases();

        /// <summary>
        /// Restore purchases async.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        UniTask<bool> RestorePurchasesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Confirm a pending purchase (for deferred purchases).
        /// </summary>
        void ConfirmPurchase(string productId);
    }
}
