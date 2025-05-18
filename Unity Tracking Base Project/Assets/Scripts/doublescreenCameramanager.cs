using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine;
using System.Linq;
using System;
using Newtonsoft.Json;

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
    private Canvas duplicatedCanvas;

    private CanvasSnapshot originalCanvasSnapshot;

    [Header("Use percentatge of overlap from the overlap file")]
    [SerializeField] private bool UseOverlapFile;
    [SerializeField, Range(0f, 100f)]
    private float percentatgeOfOverlap;
    private float visibleHeightPercentatge;

    [SerializeField] private CalibrationUIManager calibrationUIManager;

    [SerializeField] private string overlapSaveFilePath;
    private string overlapSaveFileName = "overlapCalibration";
    private string fullOverlapSaveFilePath;

    public class OverlapData { public float percentatgeOfOverlap; }

    /// <summary>
    /// This script just open the two screens on the same allication
    /// </summary>
    private void Awake()
    {

        //assign calibration save File
        if (string.IsNullOrEmpty(overlapSaveFilePath))
        {
            overlapSaveFilePath = Application.persistentDataPath;
        }
        fullOverlapSaveFilePath = overlapSaveFilePath + "/" + overlapSaveFileName + ".json";

        if (UseOverlapFile)
        {
            //load calibration if saved
            LoadOverlapJson();
        }


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

        SaveOriginalCanvasInfo();

        ConfigurateCamerasSetup();
    }

    private void LoadOverlapJson()
    {
        Debug.Log("Fetching file at: " + fullOverlapSaveFilePath);

        try
        {
            string jsonString = File.ReadAllText(fullOverlapSaveFilePath);
            OverlapData OverlapData = JsonConvert.DeserializeObject<OverlapData>(jsonString);
            percentatgeOfOverlap = OverlapData.percentatgeOfOverlap;
        }
        catch (Exception)
        {
            Debug.Log("Overlap file not found");
        }
    }

    public void OnSaveOverlapJson()
    {
        OverlapData OverlapData = new OverlapData();
        OverlapData.percentatgeOfOverlap = percentatgeOfOverlap;
        string jsonString = JsonUtility.ToJson(OverlapData);
        File.WriteAllText(fullOverlapSaveFilePath, jsonString);
    }

    private void Update()
    {
        float step = 5f * Time.deltaTime; // Adjust speed here (20 units per second)

        if (Input.GetKey(KeyCode.UpArrow))
        {
            percentatgeOfOverlap -= step;
            ConfigurateCamerasSetup();
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            percentatgeOfOverlap += step;
            ConfigurateCamerasSetup();
        }

        // Optional: Clamp between 0 and 100
        percentatgeOfOverlap = Mathf.Clamp(percentatgeOfOverlap, 0f, 100f);
    }

    private void ConfigurateCamerasSetup()
    {
        visibleHeightPercentatge = 1 / (1 + (percentatgeOfOverlap / 100));

        // Get cameras
        Camera camera1 = transform.GetChild(0).GetComponent<Camera>();
        Camera camera2 = transform.GetChild(1).GetComponent<Camera>();

        if (camera1 == null || camera2 == null)
        {
            Debug.LogError("Cameras not found on children of this GameObject.");
            return;
        }

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

    private void SaveOriginalCanvasInfo()
    {
        RectTransform originalRect = originalCanvas.GetComponent<RectTransform>();
        originalCanvasSnapshot = new CanvasSnapshot(originalRect, originalCanvas);
    }

    private void ConfigureSplitCanvas()
    {
        if (originalCanvas != null)
        {
            if (duplicatedCanvas == null)
            {
                GameObject duplicatedCanvasGO = Instantiate(originalCanvas.gameObject);
                duplicatedCanvas = duplicatedCanvasGO.GetComponent<Canvas>();

                // Set Render Mode to Overlay
                originalCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                duplicatedCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

                duplicatedCanvasGO.transform.SetParent(originalCanvas.transform.parent);
                duplicatedCanvasGO.transform.SetSiblingIndex(originalCanvas.transform.GetSiblingIndex() + 1);

                originalCanvas.targetDisplay = 0; // Display 1
                duplicatedCanvas.targetDisplay = 1; // Display 2
            }

            RectTransform originalRect = originalCanvas.GetComponent<RectTransform>();
            RectTransform duplicatedRect = duplicatedCanvas.GetComponent<RectTransform>();


            // Calculate half canvas height
            float halfCanvasHeight = originalCanvasSnapshot.sizeDelta.y * 0.5f;
            float extraCanvasHeightOffset = originalCanvasSnapshot.sizeDelta.y * (1.0f - visibleHeightPercentatge) ;
            float halfCanvasWidth = originalCanvasSnapshot.sizeDelta.x * 0.5f;

            float scaleFactor = ((halfCanvasHeight * visibleHeightPercentatge) / halfCanvasWidth) * 2;

            float canvasHeightOffset = halfCanvasHeight - extraCanvasHeightOffset;

            // Move and scale child objects of original canvas
            foreach (Transform child in originalCanvas.transform)
            {
                RectTransform childRect = child.GetComponent<RectTransform>();
                if (childRect == null) continue;

                // Try to find matching saved snapshot by name
                var matchingSnapshot = originalCanvasSnapshot.children
                    .FirstOrDefault(snap => snap.name == child.name);

                if (matchingSnapshot != null)
                {
                    childRect.anchoredPosition = new Vector2(
                        matchingSnapshot.originalAnchoredPosition.x,
                        matchingSnapshot.originalAnchoredPosition.y + canvasHeightOffset
                    );

                    childRect.localScale = new Vector3(
                        matchingSnapshot.originalLocalScale.x,
                        matchingSnapshot.originalLocalScale.y * scaleFactor,
                        matchingSnapshot.originalLocalScale.z
                    );
                }
            }


            foreach (Transform child in duplicatedCanvas.transform)
            {
                RectTransform childRect = child.GetComponent<RectTransform>();
                if (childRect == null) continue;

                // Find matching snapshot by name from the original canvas
                var matchingSnapshot = originalCanvasSnapshot.children
                    .FirstOrDefault(snap => snap.name == child.name);

                if (matchingSnapshot != null)
                {
                    float yCorrection = matchingSnapshot.originalAnchoredPosition.y * 2f;

                    childRect.anchoredPosition = new Vector2(
                        matchingSnapshot.originalAnchoredPosition.x,
                        matchingSnapshot.originalAnchoredPosition.y + canvasHeightOffset - yCorrection
                    );

                    childRect.localScale = new Vector3(
                        matchingSnapshot.originalLocalScale.x,
                        matchingSnapshot.originalLocalScale.y * scaleFactor,
                        matchingSnapshot.originalLocalScale.z
                    );

                    // Rotate 180° on Z axis (visually flips it vertically)
                    childRect.localRotation = Quaternion.Euler(0f, 0f, 180f);
                }
            }


            ConfigureMask(originalCanvas);
            ConfigureMask(duplicatedCanvas);

            // Force UI update
            Canvas.ForceUpdateCanvases();
        }
    }

    private void ConfigureMask(Canvas canvas)
    {
        // Try to find existing mask by name
        Transform existingMask = canvas.transform.Find("CanvasMask");
        GameObject maskGO;

        if (existingMask != null)
        {
            // Mask already exists; reuse it
            maskGO = existingMask.gameObject;
        }
        else
        {
            // Create masking GameObject
            maskGO = new GameObject("CanvasMask");
            maskGO.transform.SetParent(canvas.transform, false); // 'false' keeps local transform unchanged

            // Add RectMask2D component
            maskGO.AddComponent<RectMask2D>();

            // Add an Image component to visualize the mask area
            Image maskImage = maskGO.AddComponent<Image>();
            maskImage.color = new Color(0, 0, 0, 1f); // Fully opaque black for visualization (adjust as needed)
        }

        // Adjust RectTransform to mask the top (1 - visibleHeightPercentatge)
        RectTransform maskRect = maskGO.GetComponent<RectTransform>();
        if (maskRect == null)
            maskRect = maskGO.AddComponent<RectTransform>();

        maskRect.anchorMin = new Vector2(0, visibleHeightPercentatge);
        maskRect.anchorMax = new Vector2(1, 1f);
        maskRect.offsetMin = Vector2.zero;
        maskRect.offsetMax = Vector2.zero;
    }



    private void ConfigureDisplay(Camera camera1, Camera camera2)
    {
        camera1.rect = new Rect(0f, 0f, 1f, visibleHeightPercentatge);
        camera2.rect = new Rect(0f, 0f, 1f, visibleHeightPercentatge);
    }


    private class CanvasSnapshot
    {
        public Vector2 sizeDelta;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public float renderScale;

        public List<ChildSnapshot> children = new();

        public CanvasSnapshot(RectTransform rectTransform, Canvas canvas)
        {
            sizeDelta = rectTransform.sizeDelta;
            position = rectTransform.position;
            rotation = rectTransform.rotation;
            scale = rectTransform.localScale;
            renderScale = canvas.scaleFactor;

            foreach (Transform child in rectTransform)
            {
                RectTransform childRect = child.GetComponent<RectTransform>();
                if (childRect != null)
                    children.Add(new ChildSnapshot(childRect));
            }
        }
    }
    private class ChildSnapshot
    {
        public string name;
        public RectTransform rect;
        public Vector2 originalAnchoredPosition;
        public Vector3 originalLocalScale;

        public ChildSnapshot(RectTransform rectTransform)
        {
            rect = rectTransform;
            name = rectTransform.name;
            originalAnchoredPosition = rectTransform.anchoredPosition;
            originalLocalScale = rectTransform.localScale;
        }
    }


}
