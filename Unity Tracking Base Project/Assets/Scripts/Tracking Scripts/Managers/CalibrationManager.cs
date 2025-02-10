using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;


public class CalibrationManager : MonoBehaviour
{

    //////////////////////////////////////////
    //All the objects sounds and visual resources that this class controls
    //////////////////////////////////////////
    [Header("Dependencies")]
    [SerializeField] private CirclePositionManager circlePositionManager;
    [SerializeField] private CalibrationQuadManager calibrationQuadManager;
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

        GiveInstructions(step);
    }

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
    public void HideCalibrateElements()
    {
        canvasGroupManager.SwitchCanvas("Tracking Menú");
        circlePositionManager.DeactivatePulsatingCircle();
    }


    /// <summary>
    /// Saves the calibration points.
    /// </summary>
    public void SaveCalibrationPoints(Vector3 trackerPosition)
    {
        calibrationPoints.Add(trackerPosition);
        calibrationQuadManager.CreateSphere(trackerPosition);
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
