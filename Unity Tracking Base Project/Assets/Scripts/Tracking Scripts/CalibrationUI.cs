using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CalibrationUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI baseStationNumber;

    [SerializeField] private TextMeshProUGUI playerPos;
    [SerializeField] private TextMeshProUGUI playerRot;

    [SerializeField] private TextMeshProUGUI calibrationFileStatus;

    [SerializeField] private TextMeshProUGUI center;
    [SerializeField] private TextMeshProUGUI physicalWorldSize;
    [SerializeField] private TextMeshProUGUI rotationOffset;

    [SerializeField] private List<TextMeshProUGUI> playersPositions;
    [SerializeField] private List<TextMeshProUGUI> playersRotations;

    private void start()
    {
        center.text = "Uncalibrated";
        physicalWorldSize.text = "Uncalibrated";
        rotationOffset.text = "Uncalibrated";
    }

    public void SetNumberOfBaseStations(string numBaseStations) { baseStationNumber.text = numBaseStations; }
    public void SetNumberOfBaseStations(int numBaseStations) { baseStationNumber.text = numBaseStations.ToString(); }

    public void SetPlayerXPos(int x, Vector3 pos)
    {
        if ( x == 0)
            SetPlayerPos(playerPos, pos);

        if(playersPositions.Count > 0)
            SetPlayerPos(playersPositions[x], pos);
    }
    public void SetPlayerPos(TextMeshProUGUI text, Vector3 pos) { text.text = Utils.Vector3ToString(pos); }

    public void SetPlayerXRot(int x, Quaternion rot)
    {
        if (x == 0)
            SetPlayerRot(playerRot, rot);

        if(playersRotations.Count > 0)
            SetPlayerRot(playersRotations[x], rot);
    }
    public void SetPlayerRot(TextMeshProUGUI text, Quaternion rot) { text.text = Utils.QuaternionToString(rot); }

    public void SetCalibrationFileStatus(string fileStatus) { calibrationFileStatus.text = fileStatus; }

    public void SetCenter(Vector3 c) { center.text = Utils.Vector3ToString(c); }
    public void SetPhysicalWorldSize(Vector3 size) { physicalWorldSize.text = Utils.Vector3ToString(size); }
    public void SetRotationOffset(Quaternion RotOff) { rotationOffset.text = Utils.QuaternionToString(RotOff); }
}
