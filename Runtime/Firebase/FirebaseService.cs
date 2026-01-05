using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

#if FIREBASE_ANALYTICS
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
#endif

namespace Spyke.SDKs.Firebase
{
    /// <summary>
    /// Firebase service implementation with conditional compilation.
    /// Requires FIREBASE_ANALYTICS define and Firebase SDK installed.
    /// </summary>
    public class FirebaseService : IFirebaseService, IInitializable, IDisposable
    {
        private bool _isReady;
        private bool _isInitializing;
        private Action<bool> _onInitComplete;

        public bool IsReady => _isReady;

        public void Initialize()
        {
            // Auto-initialize on Zenject Initialize
            Initialize(null);
        }

        public void Initialize(Action<bool> onComplete)
        {
            if (_isReady)
            {
                onComplete?.Invoke(true);
                return;
            }

            if (_isInitializing)
            {
                _onInitComplete += onComplete;
                return;
            }

            _isInitializing = true;
            _onInitComplete = onComplete;

#if FIREBASE_ANALYTICS
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _isReady = true;
                    Log("Firebase initialized successfully");
                }
                else
                {
                    Log($"Firebase dependencies error: {dependencyStatus}", true);
                }

                _isInitializing = false;
                _onInitComplete?.Invoke(_isReady);
                _onInitComplete = null;
            });
#else
            Log("Firebase SDK not installed (FIREBASE_ANALYTICS not defined)");
            _isInitializing = false;
            _isReady = false;
            _onInitComplete?.Invoke(false);
            _onInitComplete = null;
#endif
        }

        public void SetUserId(string userId)
        {
            if (!_isReady) return;

#if FIREBASE_ANALYTICS
            FirebaseAnalytics.SetUserId(userId);
            Crashlytics.SetUserId(userId);
#endif
        }

        public void SetUserProperty(string name, string value)
        {
            if (!_isReady) return;

#if FIREBASE_ANALYTICS
            FirebaseAnalytics.SetUserProperty(name, value);
#endif
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            if (!_isReady) return;

#if FIREBASE_ANALYTICS
            if (parameters == null || parameters.Count == 0)
            {
                FirebaseAnalytics.LogEvent(eventName);
            }
            else
            {
                var firebaseParams = ConvertToFirebaseParams(parameters);
                FirebaseAnalytics.LogEvent(eventName, firebaseParams);
            }
#endif
        }

        public void LogException(Exception exception)
        {
            if (!_isReady || exception == null) return;

#if FIREBASE_ANALYTICS
            Crashlytics.LogException(exception);
#endif
        }

        public void LogCrashlyticsMessage(string message)
        {
            if (!_isReady || string.IsNullOrEmpty(message)) return;

#if FIREBASE_ANALYTICS
            Crashlytics.Log(message);
#endif
        }

        public void SetAnalyticsCollectionEnabled(bool enabled)
        {
            if (!_isReady) return;

#if FIREBASE_ANALYTICS
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(enabled);
#endif
        }

        public void Dispose()
        {
            // Firebase cleanup if needed
        }

#if FIREBASE_ANALYTICS
        private static Parameter[] ConvertToFirebaseParams(Dictionary<string, object> parameters)
        {
            var result = new Parameter[parameters.Count];
            int i = 0;

            foreach (var kvp in parameters)
            {
                result[i] = kvp.Value switch
                {
                    int intVal => new Parameter(kvp.Key, intVal),
                    long longVal => new Parameter(kvp.Key, longVal),
                    float floatVal => new Parameter(kvp.Key, floatVal),
                    double doubleVal => new Parameter(kvp.Key, doubleVal),
                    _ => new Parameter(kvp.Key, kvp.Value?.ToString() ?? "")
                };
                i++;
            }

            return result;
        }
#endif

        private static void Log(string message, bool isError = false)
        {
#if SPYKE_DEV
            if (isError)
                Debug.LogError($"[FirebaseService] {message}");
            else
                Debug.Log($"[FirebaseService] {message}");
#endif
        }
    }
}
