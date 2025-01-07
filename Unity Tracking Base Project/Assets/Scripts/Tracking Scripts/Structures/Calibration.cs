using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;

//Data can only be modified by setters
public struct Calibration
{
    public Vector3 CalibrationCenter { get; private set; }  //Center of the scene
    public Vector3 CalibrationScale { get; private set; }  //Scale comparing real world and 1, so final scale will be application size / scale
    public Quaternion CalibrationRotation { get; private set; } // Rotation diference in a quaternion

    public Calibration(Vector3 center, Vector3 scale, Quaternion matTransform)
    {
        CalibrationCenter = center;
        CalibrationScale = scale;
        CalibrationRotation = matTransform;
    }

    public void Clear()
    {
        CalibrationCenter = Vector3.zero;
        CalibrationScale = Vector3.zero;
        CalibrationRotation = Quaternion.identity;
    }

    public void SetCalibrationCenter(Vector3 vec)
    {
        CalibrationCenter = vec;
    }

    public void SetCalibrationCenter(Vector3 vec, float yOffset)
    {
        CalibrationCenter = new Vector3(vec.x, vec.y + yOffset, vec.z);
    }

    public void SetCalibrationScale(Vector3 scale)
    {
        CalibrationScale = scale;
    }

    public void SetCalibrationRotation(Quaternion mat)
    {
        CalibrationRotation = mat;
    }

    public Vector3 GetCalibrationCenter()
    {
        return CalibrationCenter;
    }

    public Vector3 GetCalibrationScale()
    {
        return CalibrationScale;
    }

    public Quaternion GetCalibrationRotation()
    {
        return CalibrationRotation;
    }
}
