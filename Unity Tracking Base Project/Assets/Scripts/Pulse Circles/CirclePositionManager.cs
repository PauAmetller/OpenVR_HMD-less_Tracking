using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclePositionManager : MonoBehaviour
{

    public PulsatingCircle pulsatingCircleScript;

    public void MoveCircles(int index_pos)
    {
        pulsatingCircleScript.active = true;
        if (index_pos == 0)
            pulsatingCircleScript.circleCenter = new Vector3(-22.5f, 0, -22.5f);
        if (index_pos == 1)
            pulsatingCircleScript.circleCenter = new Vector3(22.5f, 0, -22.5f);
        if (index_pos == 2)
            pulsatingCircleScript.circleCenter = new Vector3(22.5f, 0, 22.5f);
        if (index_pos == 3)
            pulsatingCircleScript.circleCenter = new Vector3(-22.5f, 0, 22.5f);
        if (index_pos == 4)
            pulsatingCircleScript.circleCenter = new Vector3(0, 0, 0);
    }

    public void DeactivatePulsatingCircle()
    {
        pulsatingCircleScript.active = false;
    }
}
