using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
#endif

namespace Spyke.SDKs.IAP
{
    /// <summary>
    /// Unity IAP service implementation with conditional compilation.
    /// Requires UNITY_PURCHASING define and Unity IAP package installed.
    /// </summary>
    public class IAPService : IIAPService, IInitializable, IDisposable
#if UNITY_PURCHASING
        , IDetailedStoreListener
#endif
    {
        private bool _isReady;
        private bool _isInitializing;
        private readonly List<IAPProduct> _products = new();
        private UniTaskCompletionSource<bool> _initTcs;
        private UniTaskCompletionSource<IAPResult> _purchaseTcs;
        private UniTaskCompletionSource<bool> _restoreTcs;

#if UNITY_PURCHASING
        private IStoreController _storeController;
        private IExtensionProvider _extensionProvider;
#endif

        public bool IsReady => _isReady;

        public event Action<bool> OnInitialized;
        public event Action<IAPResult> OnPurchaseCompleted;
        public event Action<string, IAPFailureReason> OnPurchaseFailed;

        public void Initialize()
        {
            // Zenject Initialize - requires manual Initialize call with products
        }

        public void Initialize(IEnumerable<IAPProduct> products)
        {
            InitializeAsync(products).Forget();
        }

        public async UniTask<bool> InitializeAsync(IEnumerable<IAPProduct> products)
        {
            if (_isReady) return true;
            if (_isInitializing)
            {
                return await _initTcs.Task;
            }

            _isInitializing = true;
            _initTcs = new UniTaskCompletionSource<bool>();

#if UNITY_PURCHASING
            try
            {
                var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

                foreach (var product in products)
                {
                    _products.Add(product);
                    var productType = ConvertProductType(product.Type);
                    builder.AddProduct(product.Id, productType);
                }

                UnityPurchasing.Initialize(this, builder);
                Log($"Initializing Unity IAP with {_products.Count} products");
            }
            catch (Exception ex)
            {
                Log($"Unity IAP initialization failed: {ex.Message}", true);
                _isReady = false;
                _isInitializing = false;
                _initTcs.TrySetResult(false);
                OnInitialized?.Invoke(false);
                return false;
            }
#else
            Log("Unity Purchasing not installed (UNITY_PURCHASING not defined)");
            _isReady = false;
            _isInitializing = false;
            _initTcs.TrySetResult(false);
            OnInitialized?.Invoke(false);
            return false;
#endif

            return await _initTcs.Task;
        }

        public IAPProduct GetProduct(string productId)
        {
            return _products.Find(p => p.Id == productId);
        }

        public IReadOnlyList<IAPProduct> GetAllProducts()
        {
            return _products.AsReadOnly();
        }

        public void Purchase(string productId)
        {
            PurchaseAsync(productId).Forget();
        }

        public async UniTask<IAPResult> PurchaseAsync(string productId)
        {
            if (!_isReady)
            {
                var notReadyResult = IAPResult.Failure(productId, IAPFailureReason.PurchasingUnavailable, "IAP not initialized");
                OnPurchaseFailed?.Invoke(productId, IAPFailureReason.PurchasingUnavailable);
                return notReadyResult;
            }

            _purchaseTcs = new UniTaskCompletionSource<IAPResult>();

#if UNITY_PURCHASING
            var storeProduct = _storeController.products.WithID(productId);
            if (storeProduct == null || !storeProduct.availableToPurchase)
            {
                var unavailableResult = IAPResult.Failure(productId, IAPFailureReason.ProductUnavailable);
                _purchaseTcs.TrySetResult(unavailableResult);
                OnPurchaseFailed?.Invoke(productId, IAPFailureReason.ProductUnavailable);
                return unavailableResult;
            }

            _storeController.InitiatePurchase(storeProduct);
#else
            var noIapResult = IAPResult.Failure(productId, IAPFailureReason.PurchasingUnavailable);
            _purchaseTcs.TrySetResult(noIapResult);
            return noIapResult;
#endif

            return await _purchaseTcs.Task;
        }

        public void RestorePurchases()
        {
            RestorePurchasesAsync().Forget();
        }

        public async UniTask<bool> RestorePurchasesAsync()
        {
            if (!_isReady) return false;

            _restoreTcs = new UniTaskCompletionSource<bool>();

#if UNITY_PURCHASING && (UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS)
            var apple = _extensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((success, error) =>
            {
                Log($"Restore completed: {success}, error: {error}");
                _restoreTcs.TrySetResult(success);
            });
#else
            _restoreTcs.TrySetResult(true);
#endif

            return await _restoreTcs.Task;
        }

        public void ConfirmPurchase(string productId)
        {
#if UNITY_PURCHASING
            var product = _storeController?.products.WithID(productId);
            if (product != null)
            {
                _storeController.ConfirmPendingPurchase(product);
            }
#endif
        }

        public void Dispose()
        {
            // Cleanup
        }

#if UNITY_PURCHASING
        // IStoreListener implementation
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Log("Unity IAP initialized successfully");
            _storeController = controller;
            _extensionProvider = extensions;
            _isReady = true;
            _isInitializing = false;

            // Update products with store metadata
            foreach (var storeProduct in controller.products.all)
            {
                var product = _products.Find(p => p.Id == storeProduct.definition.id);
                if (product != null)
                {
                    product.Title = storeProduct.metadata.localizedTitle;
                    product.Description = storeProduct.metadata.localizedDescription;
                    product.PriceString = storeProduct.metadata.localizedPriceString;
                    product.Price = storeProduct.metadata.localizedPrice;
                    product.CurrencyCode = storeProduct.metadata.isoCurrencyCode;
                    product.IsAvailable = storeProduct.availableToPurchase;
                    product.Metadata = storeProduct.metadata;
                }
            }

            _initTcs?.TrySetResult(true);
            OnInitialized?.Invoke(true);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Log($"Unity IAP initialization failed: {error}", true);
            _isReady = false;
            _isInitializing = false;
            _initTcs?.TrySetResult(false);
            OnInitialized?.Invoke(false);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Log($"Unity IAP initialization failed: {error} - {message}", true);
            _isReady = false;
            _isInitializing = false;
            _initTcs?.TrySetResult(false);
            OnInitialized?.Invoke(false);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            var productId = args.purchasedProduct.definition.id;
            var product = GetProduct(productId);

            var result = IAPResult.Success(
                productId,
                args.purchasedProduct.transactionID,
                args.purchasedProduct.receipt,
                product
            );

            Log($"Purchase completed: {productId}");
            _purchaseTcs?.TrySetResult(result);
            OnPurchaseCompleted?.Invoke(result);

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            var reason = ConvertFailureReason(failureReason);
            var result = IAPResult.Failure(product.definition.id, reason, failureReason.ToString());

            Log($"Purchase failed: {product.definition.id} - {failureReason}", true);
            _purchaseTcs?.TrySetResult(result);
            OnPurchaseFailed?.Invoke(product.definition.id, reason);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            var reason = ConvertFailureReason(failureDescription.reason);
            var result = IAPResult.Failure(product.definition.id, reason, failureDescription.message);

            Log($"Purchase failed: {product.definition.id} - {failureDescription.message}", true);
            _purchaseTcs?.TrySetResult(result);
            OnPurchaseFailed?.Invoke(product.definition.id, reason);
        }

        private static ProductType ConvertProductType(IAPProductType type)
        {
            return type switch
            {
                IAPProductType.Consumable => ProductType.Consumable,
                IAPProductType.NonConsumable => ProductType.NonConsumable,
                IAPProductType.Subscription => ProductType.Subscription,
                _ => ProductType.Consumable
            };
        }

        private static IAPFailureReason ConvertFailureReason(PurchaseFailureReason reason)
        {
            return reason switch
            {
                PurchaseFailureReason.UserCancelled => IAPFailureReason.UserCancelled,
                PurchaseFailureReason.PaymentDeclined => IAPFailureReason.PaymentDeclined,
                PurchaseFailureReason.ProductUnavailable => IAPFailureReason.ProductUnavailable,
                PurchaseFailureReason.PurchasingUnavailable => IAPFailureReason.PurchasingUnavailable,
                PurchaseFailureReason.SignatureInvalid => IAPFailureReason.SignatureInvalid,
                PurchaseFailureReason.DuplicateTransaction => IAPFailureReason.DuplicateTransaction,
                PurchaseFailureReason.ExistingPurchasePending => IAPFailureReason.ExistingPurchasePending,
                _ => IAPFailureReason.Unknown
            };
        }
#endif

        private static void Log(string message, bool isError = false)
        {
#if SPYKE_DEV
            if (isError)
                Debug.LogError($"[IAPService] {message}");
            else
                Debug.Log($"[IAPService] {message}");
#endif
        }
    }
}
