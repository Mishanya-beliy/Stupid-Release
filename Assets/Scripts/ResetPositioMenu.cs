using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositioMenu : MonoBehaviour
{
    public void Placing()
    {
        Transform device = GameObject.FindGameObjectWithTag("MainCamera").transform;
        gameObject.transform.SetPositionAndRotation(device.position, device.rotation);
    }
}
