using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class doublescreenCameramanager : MonoBehaviour
{
    [Header("Size of the map (The plane must be centered around point (0, 0, 0)")]
    [SerializeField] private GameObject plane;
    private Vector2 sizeOfTheMap;

    public enum SplitByAxis
    {
        X_Axis,
        Z_Axis
    }

    [SerializeField] SplitByAxis splitByAxis;

    [Header("If any object in your scene achieves a height superior than 50, input here the achieved height")]
    [SerializeField] private float height = 50f;

    [SerializeField] private Color backgroundColor;

    [Header("Assign Canvas")]
    [Tooltip("This canvas will be split and rendered accross the two cameras")]
    [SerializeField] private Canvas originalCanvas;

    [SerializeField] private float projectedHeight;
    [SerializeField] private float desiredHeight;
    private float visibleHeightPercentatge;

    [SerializeField] private CalibrationUIManager calibrationUIManager;

    /// <summary>
    /// This script just open the two screens on the same allication
    /// </summary>
    private void Awake()
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

        visibleHeightPercentatge = desiredHeight / projectedHeight;

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

        // Configure the display dimensions
        ConfigureDisplay(camera1, camera2);

        calibrationUIManager.GetUIsReferences();
    }

    private void ConfigureCameras(Camera camera1, Camera camera2)
    {
        Vector3 camera1Position = new Vector3(0f, 0f, 0f);
        Vector3 camera2Position = new Vector3(0f, 0f, 0f);
        float aspect = 1f;
        float orthographicSize = 1f;
        Vector3 camera1LookAt = new Vector3(0f, 0f, 0f);

        if (splitByAxis == SplitByAxis.X_Axis)
        {
            // Set the cameras at quarter-width distances along the x-axis.
            camera1Position = new Vector3(sizeOfTheMap.x / 4, height, 0f); // Camera for the right side
            camera2Position = new Vector3(-sizeOfTheMap.x / 4, height, 0f); // Camera for the left side
            aspect = sizeOfTheMap.x / (sizeOfTheMap.y / 2f);
            orthographicSize = sizeOfTheMap.x / 4f;
            camera1LookAt = new Vector3(sizeOfTheMap.x / 4, 0f, 0f);
        }
        else if (splitByAxis == SplitByAxis.Z_Axis)
        {
            // Set the cameras at quarter-width distances along the z-axis.
            camera1Position = new Vector3(0f, height, sizeOfTheMap.y / 4); // Camera for the right side
            camera2Position = new Vector3(0f, height, -sizeOfTheMap.y / 4); // Camera for the left side
            aspect = sizeOfTheMap.y / (sizeOfTheMap.x / 2f);
            orthographicSize = sizeOfTheMap.y / 4f;
            camera1LookAt = new Vector3(0f, 0f, sizeOfTheMap.y / 4);
        }


        // Configure the first camera (right side)
        camera1.aspect = aspect;  // Aspect ratio based on the plane's size
        camera1.orthographic = true;
        camera1.orthographicSize = orthographicSize;
        camera1.transform.localPosition = camera1Position;
        camera1.transform.LookAt(camera1LookAt);  // Look at the center (0, 0, 0)
        if (splitByAxis == SplitByAxis.X_Axis)
        {
            camera1.transform.rotation = Quaternion.Euler(90f, -90f, 0f); ;
        }
        else if (splitByAxis == SplitByAxis.Z_Axis)
        {
            camera1.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
        }
        camera1.backgroundColor = backgroundColor;

        // Configure the second camera (left side)
        camera2.aspect = aspect; // Aspect ratio based on the plane's size
        camera2.orthographic = true;
        camera2.orthographicSize = orthographicSize;
        camera2.transform.localPosition = camera2Position;
        camera2.transform.rotation = camera1.transform.rotation * Quaternion.Euler(180f, 180f, 0f); //I can't understand why the 180 rotation at x axis (Review)
        camera2.backgroundColor = backgroundColor;

        // Display for each camera
        if (splitByAxis == SplitByAxis.X_Axis)
        {
            camera1.targetDisplay = 0;
            camera2.targetDisplay = 1;
        }
        else if (splitByAxis == SplitByAxis.Z_Axis)
        {
            camera1.targetDisplay = 1;
            camera2.targetDisplay = 0;
        }
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
            float extraCanvasHeightOffset = originalRect.sizeDelta.y * (1.0f - visibleHeightPercentatge) ;
            float halfCanvasWidth = originalRect.sizeDelta.x * 0.5f;

            float scaleFactor = ((halfCanvasHeight * visibleHeightPercentatge) / halfCanvasWidth) * 2;

            float canvasHeightOffset = halfCanvasHeight - extraCanvasHeightOffset;

            // Move and scale child objects of original canvas
            foreach (Transform child in originalCanvas.transform)
            {
                RectTransform childRect = child.GetComponent<RectTransform>();
                if (childRect != null)
                {
                    childRect.anchoredPosition = new Vector2(childRect.anchoredPosition.x,
                                                          childRect.anchoredPosition.y + canvasHeightOffset);
                    childRect.localScale = new Vector3(childRect.localScale.x,
                                                     childRect.localScale.y * scaleFactor,
                                                     childRect.localScale.z);
                }
            }

            // Move, scale, and rotate child objects of duplicated canvas
            foreach (Transform child in duplicatedCanvas.transform)
            {
                RectTransform childRect = child.GetComponent<RectTransform>();
                if (childRect != null)
                {

                    float Ycorrection = (float)(childRect.anchoredPosition.y * 2.0);

                    childRect.anchoredPosition = new Vector2(childRect.anchoredPosition.x,
                                                             childRect.anchoredPosition.y + canvasHeightOffset - Ycorrection);
                    childRect.localScale = new Vector3(childRect.localScale.x,
                                                       childRect.localScale.y * scaleFactor,
                                                       childRect.localScale.z);

                    // Apply 180 degrees rotation around the X-axis
                    childRect.localRotation = Quaternion.Euler(0f, 0f, 180f);
                }
            }

            ConfigureMask(originalCanvas);
            ConfigureMask(duplicatedCanvas);

            // Force UI update
            Canvas.ForceUpdateCanvases();
        }
    }

    private void ConfigureDisplay(Camera camera1, Camera camera2)
    {
        camera1.rect = new Rect(0f, 0f, 1f, visibleHeightPercentatge);
        camera2.rect = new Rect(0f, 0f, 1f, visibleHeightPercentatge);
    }

    private void ConfigureMask(Canvas canvas)
    {
        // Create Masking Object
        GameObject maskGO = new GameObject("CanvasMask");
        maskGO.transform.SetParent(canvas.transform);

        // Add RectMask2D component
        RectMask2D rectMask = maskGO.AddComponent<RectMask2D>();

        // Adjust RectTransform to mask the top 20% and show the bottom 80%
        RectTransform maskRect = maskGO.GetComponent<RectTransform>();
        maskRect.anchorMin = new Vector2(0, visibleHeightPercentatge); 
        maskRect.anchorMax = new Vector2(1, 1f);
        maskRect.offsetMin = Vector2.zero;
        maskRect.offsetMax = Vector2.zero;

        // Add an Image component to visualize the mask area
        Image maskImage = maskGO.AddComponent<Image>(); 
        maskImage.color = new Color(0, 0, 0, 1f); // Semi-transparent black for visualization
    }
}
