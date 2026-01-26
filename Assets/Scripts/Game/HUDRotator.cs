using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDRotator : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.rotation = Quaternion.AngleAxis(Time.fixedDeltaTime * 120f, transform.rotation * Vector3.up) * transform.rotation;
    }
}
