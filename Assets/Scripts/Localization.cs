using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class Localization : MonoBehaviour
{
    // Optional: when set, ToggleLocale() will try to set this code first (e.g. "en", "pl").
    // You can also call SetLocaleByCode("en") directly from a UI Button (Button OnClick).
    [Tooltip("Optional: target locale code (e.g. 'en' or 'pl'). If empty, ToggleLocale cycles through available locales.")]
    public string targetLocaleCode = string.Empty;

    // Called from a UI Button to toggle/cycle locales or set the target code when provided.
    public void ToggleLocale()
    {
        StartCoroutine(ToggleLocaleCoroutine());
    }

    IEnumerator ToggleLocaleCoroutine()
    {
        // Wait for the localization system to initialize
        yield return LocalizationSettings.InitializationOperation;

        var locales = LocalizationSettings.AvailableLocales?.Locales;
        if (locales == null || locales.Count == 0)
        {
            Debug.LogWarning("Localization: No available locales found.");
            yield break;
        }

        // If a code is configured in the inspector, try to set that first
        if (!string.IsNullOrEmpty(targetLocaleCode))
        {
            var match = locales.FirstOrDefault(l => string.Equals(l.Identifier.Code, targetLocaleCode, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                LocalizationSettings.SelectedLocale = match;
                Debug.Log($"Localization: Selected locale set to '{match.Identifier.Code}' (from targetLocaleCode).");
                yield break;
            }
            else
            {
                Debug.LogWarning($"Localization: targetLocaleCode '{targetLocaleCode}' not found among available locales. Cycling instead.");
            }
        }

        // Otherwise cycle to the next locale
        var current = LocalizationSettings.SelectedLocale;
        int index = locales.IndexOf(current);
        if (index < 0) index = 0; // fallback if current isn't in the list
        int next = (index + 1) % locales.Count;
        LocalizationSettings.SelectedLocale = locales[next];
        Debug.Log($"Localization: Cycled locale to '{locales[next].Identifier.Code}'.");
    }

    // Public method to set locale by locale code (e.g. "en", "pl").
    // Wire this to a Button and pass the code string as parameter.
    public void SetLocaleByCode(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            Debug.LogWarning("Localization: SetLocaleByCode called with empty code.");
            return;
        }
        StartCoroutine(SetLocaleByCodeCoroutine(code));
    }

    IEnumerator SetLocaleByCodeCoroutine(string code)
    {
        yield return LocalizationSettings.InitializationOperation;

        var locales = LocalizationSettings.AvailableLocales?.Locales;
        if (locales == null || locales.Count == 0)
        {
            Debug.LogWarning("Localization: No available locales found.");
            yield break;
        }

        var match = locales.FirstOrDefault(l => string.Equals(l.Identifier.Code, code, StringComparison.OrdinalIgnoreCase));
        if (match != null)
        {
            LocalizationSettings.SelectedLocale = match;
            Debug.Log($"Localization: Selected locale set to '{match.Identifier.Code}'.");
        }
        else
        {
            Debug.LogWarning($"Localization: Locale with code '{code}' not found. Available: {string.Join(", ", locales.Select(l => l.Identifier.Code))}");
        }
    }

    // Public method to set locale by index in the AvailableLocales list.
    public void SetLocaleByIndex(int index)
    {
        StartCoroutine(SetLocaleByIndexCoroutine(index));
    }

    IEnumerator SetLocaleByIndexCoroutine(int index)
    {
        yield return LocalizationSettings.InitializationOperation;

        var locales = LocalizationSettings.AvailableLocales?.Locales;
        if (locales == null || locales.Count == 0)
        {
            Debug.LogWarning("Localization: No available locales found.");
            yield break;
        }

        if (index < 0 || index >= locales.Count)
        {
            Debug.LogWarning($"Localization: Index {index} out of range. Must be between 0 and {locales.Count - 1}.");
            yield break;
        }

        LocalizationSettings.SelectedLocale = locales[index];
        Debug.Log($"Localization: Selected locale set to '{locales[index].Identifier.Code}' (index {index}).");
    }

    // Helper: returns the list of available locale codes (useful for dynamic UI)
    public string[] GetAvailableLocaleCodes()
    {
        var locales = LocalizationSettings.AvailableLocales?.Locales;
        if (locales == null) return Array.Empty<string>();
        return locales.Select(l => l.Identifier.Code).ToArray();
    }
}
