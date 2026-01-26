using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    public float force = 50f;
    [HideInInspector] public bool playerIsColliding = false;

    private void FixedUpdate()
    {
        if (playerIsColliding)
        {
            Vector3 directionToCenter = transform.position - Marble.instance.transform.position; // Direction from the object to the center of the sphere
            Movement.instance.marbleVelocity += (directionToCenter.normalized * force) * Time.fixedDeltaTime;
        }
    }
}
