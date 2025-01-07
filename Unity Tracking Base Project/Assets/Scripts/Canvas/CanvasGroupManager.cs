using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CanvasGroupManager : MonoBehaviour
{
    public CanvasGroupVisibility[] canvasGroups;
    public string currentCanvasGroup = "Tracking Menú";

    public static CanvasGroupManager Instance { get; private set; }

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
        canvasGroups = FindObjectsOfType<CanvasGroupVisibility>();
        SwitchCanvas(currentCanvasGroup);
    }

    public void SwitchCanvas(string canvasGroupName)
    {
        currentCanvasGroup = canvasGroupName;

        // Update the UI
        ChangeCanvasVisibility();
    }

    public string GetCurrentCanvasGroup()
    {
        return currentCanvasGroup;
    }

    private void ChangeCanvasVisibility()
    {
        foreach (CanvasGroupVisibility canvasGroup in canvasGroups)
        {
            canvasGroup.ChangeVisibility();
        }
    }
}