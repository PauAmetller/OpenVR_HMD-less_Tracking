using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationUIManager : MonoBehaviour
{
    [SerializeField] private GameObject UIs;

    private CalibrationUI calibrationUI_1;
    private CalibrationUI calibrationUI_2;

    public void GetUIsReferences()
    {
        // Get all CalibrationUI scripts in the children of UIs GameObject
        CalibrationUI[] calibrationUIs = UIs.GetComponentsInChildren<CalibrationUI>();

        // Ensure there are at least two CalibrationUI components found
        if (calibrationUIs.Length >= 2)
        {
            calibrationUI_1 = calibrationUIs[0];
            calibrationUI_2 = calibrationUIs[1];
        }
        else
        {
            Debug.LogWarning("Less than 2 CalibrationUI components found in children.");
        }
    }

    private void CheckReferences()
    {
        if (calibrationUI_1 == null || calibrationUI_2 == null)
            GetUIsReferences();
    }
    public void SetNumberOfBaseStations(string numBaseStations)
    {
        CheckReferences();
        calibrationUI_1.SetNumberOfBaseStations(numBaseStations);
        calibrationUI_2.SetNumberOfBaseStations(numBaseStations);
    }
    public void SetNumberOfBaseStations(int numBaseStations)
    {
        CheckReferences();
        calibrationUI_1.SetNumberOfBaseStations(numBaseStations);
        calibrationUI_2.SetNumberOfBaseStations(numBaseStations);
    }

    public void SetPlayerXPos(int x, Vector3 pos)
    {
        CheckReferences();
        calibrationUI_1.SetPlayerXPos(x, pos);
        calibrationUI_2.SetPlayerXPos(x, pos);
    }

    public void SetPlayerXRot(int x, Quaternion rot)
    {
        CheckReferences();
        calibrationUI_1.SetPlayerXRot(x, rot);
        calibrationUI_2.SetPlayerXRot(x, rot);
    }

    public void SetCalibrationFileStatus(string fileStatus)
    {
        CheckReferences();
        calibrationUI_1.SetCalibrationFileStatus(fileStatus);
        calibrationUI_2.SetCalibrationFileStatus(fileStatus);
    }

    public void SetCenter(Vector3 c)
    {
        CheckReferences();
        calibrationUI_1.SetCenter(c);
        calibrationUI_2.SetCenter(c);
    }
    public void SetPhysicalWorldSize(Vector3 size)
    {
        CheckReferences();
        calibrationUI_1.SetPhysicalWorldSize(size);
        calibrationUI_2.SetPhysicalWorldSize(size);
    }
    public void SetRotationOffset(Quaternion RotOff)
    {
        CheckReferences();
        calibrationUI_1.SetRotationOffset(RotOff);
        calibrationUI_2.SetRotationOffset(RotOff);
    }

    public void SetPointPos(int x, Vector3 pos)
    {
        CheckReferences();
        calibrationUI_1.SetPointPos(x, pos);
        calibrationUI_2.SetPointPos(x, pos);
    }
}
