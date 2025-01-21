using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class CalibrationManager : MonoBehaviour
{

    //////////////////////////////////////////
    //All the objects sounds and visual resources that this class controls
    //////////////////////////////////////////
    [Header("Dependencies")]
    [SerializeField] private CirclePositionManager circlePositionManager;
    [SerializeField] private CanvasGroupManager canvasGroupManager;

    //[Header("Calibration Points Visuals")]
    //[SerializeField] private List<GameObject> pointsVisuals;

    private List<Vector3> calibrationPoints = new List<Vector3>();


    /// <summary>
    /// Instructions for one player calibration.
    /// </summary>
    public void ShowInstructions(int step)
    {
        circlePositionManager.MoveCircles(step);
        if (step == 0)
        {
            canvasGroupManager.SwitchCanvas("Tracking Step 1");
        }
        else if (step == 1)
        {
            canvasGroupManager.SwitchCanvas("Tracking Step 2");
        }
        else if (step == 2)
        {
            canvasGroupManager.SwitchCanvas("Tracking Step 3");
        }
        else if (step == 3)
        {
            canvasGroupManager.SwitchCanvas("Tracking Step 4");
        }
        else if (step == 4)
        {
            canvasGroupManager.SwitchCanvas("Tracking Step 5");
        }
        else if (step == 5)
        {
            canvasGroupManager.SwitchCanvas("Tracking Menú");
        }

        GiveInstructions(step);
    }

    ///// <summary>
    ///// Activates the visual representation of a calibration point.
    ///// </summary>
    //private void ShowPoint(int pointIndex)
    //{
    //    if (pointIndex < pointsVisuals.Count && pointsVisuals[pointIndex] != null)
    //    {
    //        pointsVisuals[pointIndex].SetActive(true); // Activate the point's visual
    //    }
    //    else
    //    {
    //        Debug.LogWarning($"Point index {pointIndex} is out of range or null.");
    //    }
    //}

    ///// <summary>
    ///// Activates the visual representation of a calibration point.
    ///// </summary>
    //private void HidePoint(int pointIndex)
    //{
    //    if (pointIndex < pointsVisuals.Count && pointsVisuals[pointIndex] != null)
    //    {
    //        pointsVisuals[pointIndex].SetActive(false); // Activate the point's visual
    //    }
    //    else
    //    {
    //        Debug.LogWarning($"Point index {pointIndex} is out of range or null.");
    //    }
    //}

    /// <summary>
    /// Provides instructions for the current step of calibration.
    /// </summary>
    private void GiveInstructions(int step)
    {

        // You can integrate audio or UI feedback here
        // Example: Show a message in the UI or play an audio instruction
        // uiManager.ShowMessage($"Track {NumOfPointsTrackedInThisStep} points.");
        // audioManager.PlayInstructionClip("track_points");
    }

    ///// <summary>
    ///// Hides all visual elements used for the calibration process.
    ///// </summary>
    //public void HideCalibrateElements()
    //{
    //    for (int pointIndex = 0; pointIndex < pointsVisuals.Count; pointIndex++)
    //    {
    //        HidePoint(pointIndex);
    //    }

    //    //Hide any other member that there could be
    //}


    /// <summary>
    /// Saves the calibration points.
    /// </summary>
    public void SaveCalibrationPoints(Vector3 trackerPosition)
    {
        calibrationPoints.Add(trackerPosition);
    }

    /// <summary>
    /// Checks the consistency of the calibration points and the geometry between them.
    /// </summary>
    public bool CheckConsistenceOfCalibrationPoints()
    {
        return CalibrationPointsUtils.CheckConsistenceOfCalibrationPoints(calibrationPoints.Take(5).ToArray());
    }

    /// <summary>
    /// Fills and returns the calculated calibration data.
    /// </summary>
    public Calibration CalculateCalibrationData(Vector3 virtualWorldSpace)
    {
        return CalibrationUtils.CalculateCalibrationData(calibrationPoints.Take(5).ToArray(), virtualWorldSpace);
    }
}
