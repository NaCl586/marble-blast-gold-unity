using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuctFan : MonoBehaviour
{
    public float force = 50f;
    [HideInInspector] public bool playerIsColliding = false;

    private void FixedUpdate()
    {
        if (playerIsColliding)
            Movement.instance.marbleVelocity += transform.rotation * Vector3.up * force * Time.fixedDeltaTime;
    }
}
