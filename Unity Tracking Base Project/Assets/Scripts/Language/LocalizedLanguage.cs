using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LocalizedText : MonoBehaviour
{
    List<Translations> translationsScripts;

    private void Awake()
    {
        translationsScripts = FindAllTextTranslatingScriptsInChildren();
    }

    public void UpdateText()
    {
        if (LanguageManager.Instance == null)
        {
            Debug.LogWarning("LanguageManager is null. Please ensure LanguageManager is in the scene.");
            return;
        }

        foreach (Translations translationsScript in translationsScripts)
        {
            translationsScript.UpdateText();
        }
    }

    private List<Translations> FindAllTextTranslatingScriptsInChildren()
    {
        List<Translations> translationsScripts = new List<Translations>();
        translationsScripts.AddRange(GetComponentsInChildren<Translations>(true));
        return translationsScripts;
    }
}

