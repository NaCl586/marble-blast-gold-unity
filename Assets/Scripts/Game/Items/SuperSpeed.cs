using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SuperSpeed : Powerups
{
    [SerializeField] float superSpeedMultiplier = 25f;
    public class OnUseSuperSpeed : UnityEvent { };
    public static OnUseSuperSpeed onUseSuperSpeed = new OnUseSuperSpeed();
    public static bool alreadyListened = false;

    public void Start()
    {
        if (!alreadyListened)
        {
            alreadyListened = true;
            onUseSuperSpeed.AddListener(UsePowerup);
        }
    }

    protected override void UsePowerup()
    {
        GameManager.instance.PlayAudioClip(useSound);

        Vector3 normal = Marble.instance.GetComponent<CheckCollision>().normal;

        Vector3 cameraRot = (Marble.instance.transform.position - Camera.main.transform.position);
        float normalInfluence = Vector3.Dot(cameraRot, normal);
        Vector3 direction = cameraRot - (normalInfluence * normal);
        if (direction.magnitude == 0f)
            direction = (new Vector3(cameraRot.x, 0f, cameraRot.z)).normalized;
        else
            direction = (new Vector3(cameraRot.x, direction.y, cameraRot.z)).normalized;

        direction *= superSpeedMultiplier;
        Movement.instance.marbleVelocity += direction;
    }
}
