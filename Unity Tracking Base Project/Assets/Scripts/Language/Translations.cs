using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Translations : MonoBehaviour
{

    public string english;
    public string spanish;
    public string catalan;

    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText()
    {
        if (textMeshPro != null && LanguageManager.Instance != null)
        {
            textMeshPro.text = GetTranslation(LanguageManager.Instance.GetCurrentLanguageIndex());
        }
    }

    private string GetTranslation(int languageIndex)
    {
        switch (languageIndex)
        {
            case 0: // English
                return GetFormatedTranslation(english);
            case 1: // Spanish
                return GetFormatedTranslation(spanish);
            case 2: // Catalan
                return GetFormatedTranslation(catalan);
            default:
                return GetFormatedTranslation(english); // Default to English
        }
    }

    private string GetFormatedTranslation(string translation)
    {
        return translation.Replace("\\n", "\n");
    }
}
