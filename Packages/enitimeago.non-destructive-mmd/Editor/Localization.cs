using nadena.dev.ndmf.localization;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace enitimeago.NonDestructiveMMD
{
    [InitializeOnLoad]
    internal static class Localization
    {
        private static List<LocalizationAsset> _localizationAssets;
        private static string[] _localeCodes;
        private static string[] _localeNames;
        private static int _currentLocaleIndex;
        private static Localizer _localizer;

        static Localization()
        {
            _localizationAssets = Directory.GetFiles("Packages/enitimeago.non-destructive-mmd/Localization")
                .Select(AssetDatabase.LoadAssetAtPath<LocalizationAsset>)
                .Where(asset => asset != null)
                .OrderBy(asset => asset.localeIsoCode)
                .ToList();
            _localeCodes = _localizationAssets
                .Select(asset => asset.localeIsoCode.ToLower())
                .ToArray();
            _localeNames = _localizationAssets
                .Select(asset => asset.GetLocalizedString($"locale:{asset.localeIsoCode}"))
                .ToArray();
            _localizer = new Localizer("en-US", () => _localizationAssets);
            _currentLocaleIndex = Array.IndexOf(_localeCodes, LanguagePrefs.Language);
        }

        public static void DrawLanguagePicker()
        {
            int newLocaleIndex = EditorGUILayout.Popup(_currentLocaleIndex, _localeNames);
            if (newLocaleIndex != _currentLocaleIndex)
            {
                LanguagePrefs.Language = _localeCodes[newLocaleIndex];
                _currentLocaleIndex = newLocaleIndex;
            }
        }

        public static string Tr(string key)
        {
            return Tr(key, key);
        }

        public static string Tr(string key, string fallback)
        {
            if (_localizer.TryGetLocalizedString(key, out var value))
            {
                return value;
            }
            return fallback;
        }
    }
}