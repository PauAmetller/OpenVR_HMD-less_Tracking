using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Setter for position
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    // Getter for position
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    // Setter for rotation
    public void SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
    }

    // Getter for rotation
    public Quaternion GetRotation()
    {
        return transform.rotation;
    }
}
