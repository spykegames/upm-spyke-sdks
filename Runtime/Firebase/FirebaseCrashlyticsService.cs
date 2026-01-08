using System;
using Spyke.Services.Crashlytics;
using UnityEngine;

#if FIREBASE_CRASHLYTICS
using Firebase.Crashlytics;
#endif

namespace Spyke.SDKs.Firebase
{
    /// <summary>
    /// Firebase Crashlytics wrapper implementing ICrashlyticsService.
    /// Requires FIREBASE_CRASHLYTICS define and Firebase Crashlytics SDK to be installed.
    /// </summary>
    public class FirebaseCrashlyticsService : ICrashlyticsService
    {
#if FIREBASE_CRASHLYTICS
        public bool IsEnabled => Crashlytics.IsCrashlyticsCollectionEnabled;
#else
        public bool IsEnabled => false;
#endif

        public void SetUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

#if FIREBASE_CRASHLYTICS
            Crashlytics.SetUserId(userId);
#if SPYKE_DEV
            Debug.Log($"[FirebaseCrashlyticsService] SetUserId: {userId}");
#endif
#endif
        }

        public void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

#if FIREBASE_CRASHLYTICS
            Crashlytics.Log(message);
#endif
        }

        public void RecordException(Exception exception)
        {
            if (exception == null)
            {
                return;
            }

#if FIREBASE_CRASHLYTICS
            Crashlytics.LogException(exception);
#if SPYKE_DEV
            Debug.LogWarning($"[FirebaseCrashlyticsService] RecordException: {exception.Message}");
#endif
#endif
        }

        public void RecordException(string message, Exception exception = null)
        {
            if (string.IsNullOrEmpty(message) && exception == null)
            {
                return;
            }

#if FIREBASE_CRASHLYTICS
            if (exception != null)
            {
                Crashlytics.Log(message);
                Crashlytics.LogException(exception);
            }
            else
            {
                // Create a custom exception for the message
                Crashlytics.LogException(new Exception(message));
            }
#if SPYKE_DEV
            Debug.LogWarning($"[FirebaseCrashlyticsService] RecordException: {message}");
#endif
#endif
        }

        public void SetCustomKey(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

#if FIREBASE_CRASHLYTICS
            Crashlytics.SetCustomKey(key, value ?? string.Empty);
#endif
        }

        public void SetCustomKey(string key, int value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

#if FIREBASE_CRASHLYTICS
            Crashlytics.SetCustomKey(key, value);
#endif
        }

        public void SetCustomKey(string key, bool value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

#if FIREBASE_CRASHLYTICS
            Crashlytics.SetCustomKey(key, value);
#endif
        }

        public void ForceCrash()
        {
#if SPYKE_DEV && FIREBASE_CRASHLYTICS
            Debug.LogError("[FirebaseCrashlyticsService] Forcing crash for testing...");
            throw new Exception("Test crash from FirebaseCrashlyticsService.ForceCrash()");
#endif
        }
    }
}
