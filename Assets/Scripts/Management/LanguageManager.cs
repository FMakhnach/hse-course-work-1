﻿using System;
using UnityEngine;

/// <summary>
/// Singleton for managing language switching.
/// </summary>
public class LanguageManager : Singleton<LanguageManager>
{
    private Language currentLanguage;
    public Language CurrentLanguage => currentLanguage;

    /// <summary>
    /// All labels should subscribe it to change language in game.
    /// </summary>
    public event Action onLanguageChanged;

    public void SetLanguage(Language newLanguage)
    {
        if (newLanguage != currentLanguage)
        {
            currentLanguage = newLanguage;
            onLanguageChanged?.Invoke();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        currentLanguage = (Language)PlayerPrefs.GetInt("language");
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("language", (int)CurrentLanguage);
    }
}
