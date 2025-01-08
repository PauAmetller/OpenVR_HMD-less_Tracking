using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class doublescreenCameramanager : MonoBehaviour
{
    [Header("Size of the map (The plane must be centered around point (0, 0, 0)")]
    [SerializeField] private GameObject plane;
    private Vector2 sizeOfTheMap;

    [Header("If any object in your scene achieves a height superior than 50, input here the achieved height")]
    [SerializeField] private float height = 50f;

    [SerializeField] private Color backgroundColor;

    [Header("Assign Canvas")]
    [Tooltip("This canvas will be split and rendered accross the two cameras")]
    [SerializeField] private Canvas originalCanvas;

    /// <summary>
    /// This script just open the two screens on the same allication
    /// </summary>
    void Awake()
    {
        //NB: screen indexes start from 1
        for (int i = 0; i < GameObject.FindObjectsOfType<Camera>().Length; i++)
        {
            if (i < Display.displays.Length)
            {
                Display.displays[i].Activate();
            }
        }

        //Multiply by 10 since the default size of a plane is 10x10
        sizeOfTheMap = new Vector2(plane.transform.localScale.x * 10f, plane.transform.localScale.z * 10f);

        // Get cameras
        Camera camera1 = transform.GetChild(0).GetComponent<Camera>();
        Camera camera2 = transform.GetChild(1).GetComponent<Camera>();

        if (camera1 == null || camera2 == null)
        {
            Debug.LogError("Cameras not found on children of this GameObject.");
            return;
        }

        // Position and configure the cameras based on map size and height
        ConfigureCameras(camera1, camera2);

        // Configure the canvas display between the cameras
        ConfigureSplitCanvas();
    }

    private void ConfigureCameras(Camera camera1, Camera camera2)
    {

        // Set the cameras at quarter-width distances along the x-axis.
        Vector3 camera1Position = new Vector3(sizeOfTheMap.x / 4, height, 0f); // Camera for the right side
        Vector3 camera2Position = new Vector3(-sizeOfTheMap.x / 4, height, 0f); // Camera for the left side

        // Cameras look perpendicular to the plane
        Vector3 camera1LookAt = new Vector3(sizeOfTheMap.x / 4, 0f, 0f);
        Vector3 camera2LookAt = new Vector3(-sizeOfTheMap.x / 4, 0f, 0f);

        // Configure the first camera (right side)
        camera1.aspect = sizeOfTheMap.x / (sizeOfTheMap.y / 2f);  // Aspect ratio based on the plane's size
        camera1.orthographic = true;
        camera1.orthographicSize = sizeOfTheMap.y / 4f;  
        camera1.transform.localPosition = camera1Position;
        camera1.transform.LookAt(camera1LookAt);  // Look at the center (0, 0, 0)
        camera1.backgroundColor = backgroundColor;

        // Configure the second camera (left side)
        camera2.aspect = sizeOfTheMap.x / (sizeOfTheMap.y / 2f); // Aspect ratio based on the plane's size
        camera2.orthographic = true;
        camera2.orthographicSize = sizeOfTheMap.y / 4f; 
        camera2.transform.localPosition = camera2Position;
        camera2.transform.LookAt(camera2LookAt);  // Look at the center (0, 0, 0)
        camera2.backgroundColor = backgroundColor;

        // Display for each camera
        camera1.targetDisplay = 0;
        camera2.targetDisplay = 1;
    }

    private void ConfigureSplitCanvas()
    {
        if (originalCanvas != null)
        {
            GameObject duplicatedCanvasGO = Instantiate(originalCanvas.gameObject);
            Canvas duplicatedCanvas = duplicatedCanvasGO.GetComponent<Canvas>();

            // Set Render Mode to Overlay
            originalCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            duplicatedCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            duplicatedCanvasGO.transform.SetParent(originalCanvas.transform.parent);
            duplicatedCanvasGO.transform.SetSiblingIndex(originalCanvas.transform.GetSiblingIndex() + 1);

            originalCanvas.targetDisplay = 0; // Display 1
            duplicatedCanvas.targetDisplay = 1; // Display 2

            RectTransform originalRect = originalCanvas.GetComponent<RectTransform>();
            RectTransform duplicatedRect = duplicatedCanvasGO.GetComponent<RectTransform>();

            // Calculate half canvas height
            float halfCanvasHeight = originalRect.sizeDelta.y * 0.5f;
            float halfCanvasWidth = originalRect.sizeDelta.x * 0.5f;

            float scaleFactor = (halfCanvasHeight / halfCanvasWidth) * 2;

            // Move and scale child objects of original canvas
            foreach (Transform child in originalCanvas.transform)
            {
                RectTransform childRect = child.GetComponent<RectTransform>();
                if (childRect != null)
                {
                    childRect.anchoredPosition = new Vector2(childRect.anchoredPosition.x,
                                                          childRect.anchoredPosition.y + halfCanvasHeight);
                    childRect.localScale = new Vector3(childRect.localScale.x,
                                                     childRect.localScale.y * scaleFactor,
                                                     childRect.localScale.z);
                }
            }

            // Move and scale child objects of duplicated canvas
            foreach (Transform child in duplicatedCanvas.transform)
            {
                RectTransform childRect = child.GetComponent<RectTransform>();
                if (childRect != null)
                {
                    childRect.anchoredPosition = new Vector2(childRect.anchoredPosition.x,
                                                          childRect.anchoredPosition.y - halfCanvasHeight);
                    childRect.localScale = new Vector3(childRect.localScale.x,
                                                     childRect.localScale.y * scaleFactor,
                                                     childRect.localScale.z);
                }
            }

            // Force UI update
            Canvas.ForceUpdateCanvases();
        }
    }
}
