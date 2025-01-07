using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGroupVisibility : MonoBehaviour
{
    public void ChangeVisibility()
    {
        if (gameObject.name == CanvasGroupManager.Instance.GetCurrentCanvasGroup())
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}

