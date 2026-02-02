using UnityEngine;

public class DuctFan : MonoBehaviour
{
    public float radius = 10f;
    public float arc = 0.7f;
    public float strength = 80f;

    private void FixedUpdate()
    {
        Vector3 force = ComputeConicForce();
        Movement.instance.marbleVelocity += force * Time.fixedDeltaTime;
    }

    private Vector3 ComputeConicForce()
    {
        Vector3 marblePos = Movement.instance.transform.position;
        Vector3 coneTip = transform.position - transform.up * 0.7f;

        // Sphere check (collider equivalent)
        Vector3 toMarble = marblePos - coneTip;
        float distance = toMarble.magnitude;

        if (distance <= Mathf.Epsilon || distance > radius)
            return Vector3.zero;

        // Distance falloff
        float distanceFactor = 1f - (distance / radius);
        float finalStrength = distanceFactor * strength;

        // Normalize direction
        toMarble /= distance;

        // Cone check (force filter)
        // Torque uses the object's Y axis as force direction
        Vector3 axis = transform.up;

        float dot = Vector3.Dot(axis, toMarble);

        // Outside cone
        if (dot <= arc)
            return Vector3.zero;

        // Smooth cone falloff
        float coneFactor = (dot - arc) / (1f - arc);

        return toMarble * finalStrength * coneFactor;
    }
}
