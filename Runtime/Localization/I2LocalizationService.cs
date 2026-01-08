using System;
using System.Collections.Generic;
using Spyke.Services.Localization;
using UnityEngine;

#if I2_LOCALIZATION
using I2.Loc;
#endif

namespace Spyke.SDKs.Localization
{
    /// <summary>
    /// I2 Localization wrapper implementing ILocalizationService.
    /// Requires I2_LOCALIZATION define and I2 Localization asset to be installed.
    /// </summary>
    public class I2LocalizationService : ILocalizationService
    {
        private List<string> _availableLanguages;

        public event Action<string> OnLanguageChanged;

#if I2_LOCALIZATION
        public string CurrentLanguage => LocalizationManager.CurrentLanguageCode;

        public IReadOnlyList<string> AvailableLanguages
        {
            get
            {
                if (_availableLanguages == null)
                {
                    _availableLanguages = new List<string>();
                    foreach (var lang in LocalizationManager.GetAllLanguages())
                    {
                        var code = LocalizationManager.GetLanguageCode(lang);
                        if (!string.IsNullOrEmpty(code))
                        {
                            _availableLanguages.Add(code);
                        }
                    }
                }
                return _availableLanguages;
            }
        }

        public bool IsInitialized => LocalizationManager.Sources.Count > 0;
#else
        public string CurrentLanguage => "en";
        public IReadOnlyList<string> AvailableLanguages => new List<string> { "en" };
        public bool IsInitialized => false;
#endif

        public I2LocalizationService()
        {
#if I2_LOCALIZATION
            LocalizationManager.OnLocalizeEvent += HandleLocalizationChanged;
#endif
        }

        public string GetText(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

#if I2_LOCALIZATION
            var translation = LocalizationManager.GetTranslation(key);
            return !string.IsNullOrEmpty(translation) ? translation : key;
#else
            return key;
#endif
        }

        public string GetText(string key, params object[] args)
        {
            var text = GetText(key);

            if (args == null || args.Length == 0)
            {
                return text;
            }

            try
            {
                return string.Format(text, args);
            }
            catch (FormatException)
            {
#if SPYKE_DEV
                Debug.LogWarning($"[I2LocalizationService] Format error for key: {key}");
#endif
                return text;
            }
        }

        public bool HasKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

#if I2_LOCALIZATION
            return LocalizationManager.GetTranslation(key) != null;
#else
            return false;
#endif
        }

        public bool SetLanguage(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                return false;
            }

#if I2_LOCALIZATION
            var languageName = LocalizationManager.GetLanguageFromCode(languageCode);
            if (string.IsNullOrEmpty(languageName))
            {
#if SPYKE_DEV
                Debug.LogWarning($"[I2LocalizationService] Language not found: {languageCode}");
#endif
                return false;
            }

            LocalizationManager.CurrentLanguage = languageName;
            return true;
#else
            return false;
#endif
        }

        public string GetLanguageDisplayName(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                return string.Empty;
            }

#if I2_LOCALIZATION
            var languageName = LocalizationManager.GetLanguageFromCode(languageCode);
            return !string.IsNullOrEmpty(languageName) ? languageName : languageCode;
#else
            return languageCode;
#endif
        }

#if I2_LOCALIZATION
        private void HandleLocalizationChanged()
        {
            OnLanguageChanged?.Invoke(CurrentLanguage);
        }
#endif
    }
}
