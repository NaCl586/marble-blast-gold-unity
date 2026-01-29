using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifChunkCulling : MonoBehaviour
{
    public Transform marble;
    public float activeRadius = 60f;

    MeshCollider[] colliders;

    void Start()
    {
        marble = Marble.instance.transform;
        colliders = GetComponentsInChildren<MeshCollider>();
    }

    void FixedUpdate()
    {
        Vector3 pos = marble.position;
        float r2 = activeRadius * activeRadius;

        foreach (var c in colliders)
        {
            bool active =
                (c.bounds.center - pos).sqrMagnitude <= r2;

            if (c.enabled != active)
                c.enabled = active;
        }
    }
}

