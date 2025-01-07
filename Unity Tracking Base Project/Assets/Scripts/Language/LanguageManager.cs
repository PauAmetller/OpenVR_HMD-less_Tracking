using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public List<string> languages = new List<string> { "English", "Spanish", "Catalan" };
    public string currentLanguage = "English";

    public static LanguageManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SwitchLanguage(currentLanguage);
    }

    public void SwitchLanguage(string language)
    {
        currentLanguage = language;

        // Update the UI
        UpdateAllUI();
    }

    public int GetCurrentLanguageIndex()
    {
        return languages.IndexOf(currentLanguage);
    }

    private void UpdateAllUI()
    {
        var localizedTexts = FindObjectsOfType<LocalizedText>();
        foreach (LocalizedText localizedText in localizedTexts)
        {
            localizedText.UpdateText();
        }
    }
}

